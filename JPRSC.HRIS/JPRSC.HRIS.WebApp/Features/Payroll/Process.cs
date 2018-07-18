using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace JPRSC.HRIS.WebApp.Features.Payroll
{
    public class Process
    {
        public class Command : IRequest<CommandResult>
        {
            public int? ClientId { get; set; }
            public DateTime? PayrollPeriodFrom { get; set; }
            public DateTime? PayrollPeriodTo { get; set; }
            public int? PayrollPeriod { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.ClientId)
                    .NotEmpty();

                RuleFor(c => c.PayrollPeriodFrom)
                    .NotEmpty();

                RuleFor(c => c.PayrollPeriodTo)
                    .NotEmpty();

                RuleFor(c => c.PayrollPeriod)
                    .NotEmpty();
            }
        }

        public class CommandResult
        {
        }

        public class CommandHandler : IRequestHandler<Command, CommandResult>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<CommandResult> Handle(Command command, CancellationToken cancellationToken)
            {
                var now = DateTime.UtcNow;

                var client = await _db.Clients
                    .SingleAsync(c => c.Id == command.ClientId);

                var clientEmployees = await _db.Employees
                    .Where(e => !e.DeletedOn.HasValue && e.ClientId == command.ClientId && e.DailyRate.HasValue)
                    .ToListAsync();

                var clientEmployeeIds = clientEmployees.Select(e => e.Id).ToList();

                var dailyTimeRecords = await _db.DailyTimeRecords
                    .Where(dtr => !dtr.DeletedOn.HasValue && dtr.EmployeeId.HasValue && clientEmployeeIds.Contains(dtr.EmployeeId.Value) && dtr.PayrollPeriodFrom == command.PayrollPeriodFrom && dtr.PayrollPeriodTo == command.PayrollPeriodTo)
                    .ToListAsync();

                var overtimes = await _db.Overtimes
                    .Where(ot => !ot.DeletedOn.HasValue && ot.EmployeeId.HasValue && clientEmployeeIds.Contains(ot.EmployeeId.Value) && ot.PayrollPeriodFrom == command.PayrollPeriodFrom && ot.PayrollPeriodTo == command.PayrollPeriodTo)
                    .ToListAsync();

                var earningDeductions = await _db.EarningDeductionRecords
                    .Where(edr => !edr.DeletedOn.HasValue && edr.EmployeeId.HasValue && clientEmployeeIds.Contains(edr.EmployeeId.Value) && edr.PayrollPeriodFrom == command.PayrollPeriodFrom && edr.PayrollPeriodTo == command.PayrollPeriodTo)
                    .Include(edr => edr.EarningDeduction)
                    .ToListAsync();

                var loans = await _db.Loans
                    .Where(l => !l.DeletedOn.HasValue && clientEmployeeIds.Contains(l.EmployeeId.Value) && l.PayrollPeriod == command.PayrollPeriod && l.RemainingBalance > 0)
                    .ToListAsync();

                var sssRecords = await _db.SSSRecords
                    .ToListAsync();

                var shouldDeductSSS = client.SSSPayrollPeriods.Contains(command.PayrollPeriod.Value);
                var shouldDeductPHIC = client.PHICPayrollPeriods.Contains(command.PayrollPeriod.Value);
                var shouldDeductPagIbig = client.PagIbigPayrollPeriods.Contains(command.PayrollPeriod.Value);
                var shouldDeductTax = client.TaxPayrollPeriods.Contains(command.PayrollPeriod.Value);

                var existingPayrollProcessBatch = await _db.PayrollProcessBatches
                    .FirstOrDefaultAsync(ppb => ppb.ClientId == command.ClientId && ppb.PayrollPeriod == command.PayrollPeriod && ppb.PayrollPeriodFrom == command.PayrollPeriodFrom && ppb.PayrollPeriodTo == command.PayrollPeriodTo);

                if (existingPayrollProcessBatch != null)
                {
                    existingPayrollProcessBatch.DateOverwritten = now;
                }

                var payrollProcessBatch = new PayrollProcessBatch
                {
                    AddedOn = now,
                    ClientId = command.ClientId,
                    DeductedPagIbig = shouldDeductPagIbig,
                    DeductedPHIC = shouldDeductPHIC,
                    DeductedSSS = shouldDeductSSS,
                    DeductedTax = shouldDeductTax,
                    PayrollPeriod = command.PayrollPeriod,
                    PayrollPeriodFrom = command.PayrollPeriodFrom,
                    PayrollPeriodTo = command.PayrollPeriodTo
                };

                foreach (var employee in clientEmployees)
                {
                    var employeeDtrs = dailyTimeRecords.Where(dtr => dtr.EmployeeId == employee.Id);
                    var employeeOts = overtimes.Where(ot => ot.EmployeeId == employee.Id);
                    var employeeEds = earningDeductions.Where(ed => ed.EmployeeId == employee.Id);
                    var employeeLoans = loans.Where(l => l.EmployeeId == employee.Id);

                    var payrollRecord = new PayrollRecord
                    {
                        AddedOn = now,
                        COLADailyValue = employeeDtrs.Sum(dtr => dtr.COLADailyValue),
                        DaysWorkedValue = employeeDtrs.Sum(dtr => dtr.DaysWorkedValue),
                        
                        DeductionsValue = employeeEds.Where(ed => ed.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(ed => ed.Amount),
                        EarningsValue = employeeEds.Where(ed => ed.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(ed => ed.Amount),
                        EmployeeId = employee.Id,
                        HoursLateValue = employeeDtrs.Sum(dtr => dtr.HoursLateValue),
                        HoursUndertimeValue = employeeDtrs.Sum(dtr => dtr.HoursUndertimeValue),
                        HoursWorkedValue = employeeDtrs.Sum(dtr => dtr.HoursWorkedValue),
                        LoanPaymentValue = employeeLoans.Any() ? loans.Sum(l => l.RemainingBalance > l.DeductionAmount ? l.DeductionAmount : l.RemainingBalance) : null,
                        OvertimeValue = employeeOts.Sum(ot => ot.NumberOfHoursValue)
                    };

                    if (shouldDeductSSS)
                    {
                        payrollRecord.SSSValueEmployee = ComputeSSSEmployee(employee, sssRecords);
                        payrollRecord.SSSValueEmployer = ComputeSSSEmployer(employee, sssRecords);
                    }

                    if (shouldDeductPHIC)
                    {
                        payrollRecord.PHICValueEmployee = ComputePHICEmployee(employee);
                        payrollRecord.PHICValueEmployer = ComputePHICEmployer(employee);
                    }

                    if (shouldDeductPagIbig) payrollRecord.PagIbigValue = ComputePagIbig(employee, client);
                    if (shouldDeductTax) payrollRecord.TaxValue = ComputeTax(employee, client);

                    payrollProcessBatch.PayrollRecords.Add(payrollRecord);
                }

                foreach (var loan in loans)
                {
                    loan.RemainingBalance -= loan.RemainingBalance > loan.DeductionAmount ? loan.DeductionAmount : loan.RemainingBalance;
                }

                _db.PayrollProcessBatches.Add(payrollProcessBatch);

                await _db.SaveChangesAsync();

                return new CommandResult();
            }

            private decimal? ComputeSSSEmployee(Employee employee, IEnumerable<SSSRecord> sssRecords)
            {
                var basicSalary = employee.DailyRate * 20;

                SSSRecord matchingRange = null;

                try
                {
                    matchingRange = sssRecords
                        .OrderBy(s => s.Range1)
                        .First(s => s.Range1 > basicSalary);
                }
                catch
                {
                    throw new Exception($"Matching SSS range not found for salary {basicSalary}");
                }

                return matchingRange.Employee;
            }

            private decimal? ComputeSSSEmployer(Employee employee, IEnumerable<SSSRecord> sssRecords)
            {
                var basicSalary = employee.DailyRate * 20;

                SSSRecord matchingRange = null;

                try
                {
                    matchingRange = sssRecords
                        .OrderBy(s => s.Range1)
                        .First(s => s.Range1 > basicSalary);
                }
                catch
                {
                    throw new Exception($"Matching SSS range not found for salary {basicSalary}");
                }

                return matchingRange.Employer;
            }

            private decimal? ComputePHICEmployee(Employee employee)
            {
                decimal? phic = null;

                var basicSalary = employee.DailyRate * 20;

                phic = (decimal)(0.0275 / 2) * basicSalary;

                return phic;
            }

            private decimal? ComputePHICEmployer(Employee employee)
            {
                decimal? phic = null;

                var basicSalary = employee.DailyRate * 20;

                phic = (decimal)(0.0275 / 2) * basicSalary;

                return phic;
            }

            private decimal? ComputePagIbig(Employee employee, Client client)
            {
                return 100;
            }

            private decimal? ComputeTax(Employee employee, Client client)
            {
                return null;
            }
        }
    }
}
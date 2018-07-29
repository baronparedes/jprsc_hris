using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Dependency;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            private readonly ApplicationDbContext _db = DependencyConfig.Instance.Container.GetInstance<ApplicationDbContext>();

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

                When(c => c.PayrollPeriodFrom.HasValue && c.PayrollPeriodTo.HasValue && c.PayrollPeriod.HasValue, () =>
                {
                    RuleFor(c => c.ClientId)
                        .Must(NotHavePendingRecordsForEndProcess)
                        .WithMessage("There are pending records for end process for this client.");
                });
            }

            private bool NotHavePendingRecordsForEndProcess(Command commnad, int? clientId)
            {
                return !_db
                    .PayrollProcessBatches
                    .Any(ppb => !ppb.DeletedOn.HasValue && !ppb.DateOverwritten.HasValue && !ppb.EndProcessedOn.HasValue &&
                                ppb.PayrollPeriodFrom == commnad.PayrollPeriodFrom && ppb.PayrollPeriodTo == commnad.PayrollPeriodTo && ppb.PayrollPeriod == commnad.PayrollPeriod);
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
                    .Where(l => !l.DeletedOn.HasValue && clientEmployeeIds.Contains(l.EmployeeId.Value) && l.LoanPayrollPeriods.ToList().Contains(command.PayrollPeriod.Value) && l.RemainingBalance > 0 && l.ZeroedOutOn.HasValue && DbFunctions.TruncateTime(l.StartDeductionDate) >= DbFunctions.TruncateTime(command.PayrollPeriodFrom))
                    .ToListAsync();

                var sssRecords = await _db.SSSRecords
                    .ToListAsync();

                var shouldDeductSSS = client.SSSPayrollPeriods.Contains(command.PayrollPeriod.Value);
                var shouldDeductPHIC = client.PHICPayrollPeriods.Contains(command.PayrollPeriod.Value);
                var shouldDeductPagIbig = client.PagIbigPayrollPeriods.Contains(command.PayrollPeriod.Value);
                var shouldDeductTax = client.TaxPayrollPeriods.Contains(command.PayrollPeriod.Value);

                var existingPayrollProcessBatch = await _db.PayrollProcessBatches
                    .FirstOrDefaultAsync(ppb => !ppb.DeletedOn.HasValue && !ppb.DateOverwritten.HasValue && !ppb.EndProcessedOn.HasValue && ppb.ClientId == command.ClientId && ppb.PayrollPeriod == command.PayrollPeriod && ppb.PayrollPeriodFrom == command.PayrollPeriodFrom && ppb.PayrollPeriodTo == command.PayrollPeriodTo);

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
                    PayrollPeriodMonth = client.PayrollPeriodMonth,
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

                    if (!employeeDtrs.Any()) continue;

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
                        payrollRecord.SSSValueEmployee = ComputeSSSEmployee(employee, client, employeeDtrs, employeeOts, employeeEds, sssRecords);
                        payrollRecord.SSSValueEmployer = ComputeSSSEmployer(employee, client, employeeDtrs, employeeOts, employeeEds, sssRecords);
                    }

                    if (shouldDeductPHIC)
                    {
                        payrollRecord.PHICValueEmployee = ComputePHICEmployee(employee, client, employeeDtrs, employeeOts, employeeEds);
                        payrollRecord.PHICValueEmployer = ComputePHICEmployer(employee, client, employeeDtrs, employeeOts, employeeEds);
                    }

                    if (shouldDeductPagIbig) payrollRecord.PagIbigValue = ComputePagIbig(employee, client, employeeDtrs, employeeOts, employeeEds);
                    if (shouldDeductTax) payrollRecord.TaxValue = ComputeTax(employee, client, employeeDtrs, employeeOts, employeeEds);

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

            private decimal? ComputeSSSEmployee(Employee employee, Client client, IEnumerable<DailyTimeRecord> employeeDtrs, IEnumerable<Overtime> employeeOts, IEnumerable<EarningDeductionRecord> employeeEds, IEnumerable<SSSRecord> sssRecords)
            {
                var computationBasis = 0m;
                var basicSalary = employee.DailyRate.GetValueOrDefault() * 20;

                if (client.SSSBasic == true) computationBasis += basicSalary;
                if (client.SSSCola == true) computationBasis += employeeDtrs.Sum(dtr => dtr.COLADailyValue).GetValueOrDefault();
                if (client.SSSOvertime == true) computationBasis += employeeOts.Sum(ot => ot.NumberOfHoursValue).GetValueOrDefault();
                if (client.SSSEarnings == true) computationBasis += employeeEds.Where(ed => ed.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(ed => ed.Amount).GetValueOrDefault();
                if (client.SSSDeductions == true) computationBasis -= employeeEds.Where(ed => ed.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(ed => ed.Amount).GetValueOrDefault();
                if (client.SSSUndertime == true) computationBasis -= employeeDtrs.Sum(dtr => dtr.HoursUndertimeValue).GetValueOrDefault();

                SSSRecord matchingRange = null;

                try
                {
                    matchingRange = sssRecords
                        .OrderBy(s => s.Range1)
                        .First(s => s.Range1 > computationBasis);
                }
                catch
                {
                    throw new Exception($"Matching SSS range not found for salary {basicSalary}");
                }

                return matchingRange.Employee;
            }

            private decimal? ComputeSSSEmployer(Employee employee, Client client, IEnumerable<DailyTimeRecord> employeeDtrs, IEnumerable<Overtime> employeeOts, IEnumerable<EarningDeductionRecord> employeeEds, IEnumerable<SSSRecord> sssRecords)
            {
                var computationBasis = 0m;
                var basicSalary = employee.DailyRate.GetValueOrDefault() * 20;

                if (client.SSSBasic == true) computationBasis += basicSalary;
                if (client.SSSCola == true) computationBasis += employeeDtrs.Sum(dtr => dtr.COLADailyValue).GetValueOrDefault();
                if (client.SSSOvertime == true) computationBasis += employeeOts.Sum(ot => ot.NumberOfHoursValue).GetValueOrDefault();
                if (client.SSSEarnings == true) computationBasis += employeeEds.Where(ed => ed.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(ed => ed.Amount).GetValueOrDefault();
                if (client.SSSDeductions == true) computationBasis -= employeeEds.Where(ed => ed.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(ed => ed.Amount).GetValueOrDefault();
                if (client.SSSUndertime == true) computationBasis -= employeeDtrs.Sum(dtr => dtr.HoursUndertimeValue).GetValueOrDefault();

                SSSRecord matchingRange = null;

                try
                {
                    matchingRange = sssRecords
                        .OrderBy(s => s.Range1)
                        .First(s => s.Range1 > computationBasis);
                }
                catch
                {
                    throw new Exception($"Matching SSS range not found for salary {basicSalary}");
                }

                return matchingRange.Employer;
            }

            private decimal? ComputePHICEmployee(Employee employee, Client client, IEnumerable<DailyTimeRecord> employeeDtrs, IEnumerable<Overtime> employeeOts, IEnumerable<EarningDeductionRecord> employeeEds)
            {
                decimal? phic = null;

                var computationBasis = 0m;
                var basicSalary = employee.DailyRate.GetValueOrDefault() * 20;

                if (client.PHICBasic == true) computationBasis += basicSalary;
                if (client.PHICCola == true) computationBasis += employeeDtrs.Sum(dtr => dtr.COLADailyValue).GetValueOrDefault();
                if (client.PHICOvertime == true) computationBasis += employeeOts.Sum(ot => ot.NumberOfHoursValue).GetValueOrDefault();
                if (client.PHICEarnings == true) computationBasis += employeeEds.Where(ed => ed.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(ed => ed.Amount).GetValueOrDefault();
                if (client.PHICDeductions == true) computationBasis -= employeeEds.Where(ed => ed.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(ed => ed.Amount).GetValueOrDefault();
                if (client.PHICUndertime == true) computationBasis -= employeeDtrs.Sum(dtr => dtr.HoursUndertimeValue).GetValueOrDefault();

                phic = (decimal)(0.0275 / 2) * computationBasis;

                return phic;
            }

            private decimal? ComputePHICEmployer(Employee employee, Client client, IEnumerable<DailyTimeRecord> employeeDtrs, IEnumerable<Overtime> employeeOts, IEnumerable<EarningDeductionRecord> employeeEds)
            {
                decimal? phic = null;

                var computationBasis = 0m;
                var basicSalary = employee.DailyRate.GetValueOrDefault() * 20;

                if (client.PHICBasic == true) computationBasis += basicSalary;
                if (client.PHICCola == true) computationBasis += employeeDtrs.Sum(dtr => dtr.COLADailyValue).GetValueOrDefault();
                if (client.PHICOvertime == true) computationBasis += employeeOts.Sum(ot => ot.NumberOfHoursValue).GetValueOrDefault();
                if (client.PHICEarnings == true) computationBasis += employeeEds.Where(ed => ed.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(ed => ed.Amount).GetValueOrDefault();
                if (client.PHICDeductions == true) computationBasis -= employeeEds.Where(ed => ed.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(ed => ed.Amount).GetValueOrDefault();
                if (client.PHICUndertime == true) computationBasis -= employeeDtrs.Sum(dtr => dtr.HoursUndertimeValue).GetValueOrDefault();

                phic = (decimal)(0.0275 / 2) * computationBasis;

                return phic;
            }

            private decimal? ComputePagIbig(Employee employee, Client client, IEnumerable<DailyTimeRecord> employeeDtrs, IEnumerable<Overtime> employeeOts, IEnumerable<EarningDeductionRecord> employeeEds)
            {
                return 100;
            }

            private decimal? ComputeTax(Employee employee, Client client, IEnumerable<DailyTimeRecord> employeeDtrs, IEnumerable<Overtime> employeeOts, IEnumerable<EarningDeductionRecord> employeeEds)
            {
                return null;
            }
        }
    }
}
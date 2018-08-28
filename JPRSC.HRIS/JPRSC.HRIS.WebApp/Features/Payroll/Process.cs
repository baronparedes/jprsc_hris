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
                    .Any(ppb => !ppb.DeletedOn.HasValue && !ppb.DateOverwritten.HasValue && !ppb.EndProcessedOn.HasValue && ppb.ClientId == clientId.Value &&
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

                var dailyTimeRecordsForPayrollPeriod = await _db.DailyTimeRecords
                    .Where(dtr => !dtr.DeletedOn.HasValue && dtr.EmployeeId.HasValue && clientEmployeeIds.Contains(dtr.EmployeeId.Value) && dtr.PayrollPeriodFrom == command.PayrollPeriodFrom && dtr.PayrollPeriodTo == command.PayrollPeriodTo)
                    .ToListAsync();

                var overtimesForPayrollPeriod = await _db.Overtimes
                    .Where(ot => !ot.DeletedOn.HasValue && ot.EmployeeId.HasValue && clientEmployeeIds.Contains(ot.EmployeeId.Value) && ot.PayrollPeriodFrom == command.PayrollPeriodFrom && ot.PayrollPeriodTo == command.PayrollPeriodTo)
                    .ToListAsync();

                var earningDeductionRecordsForPayrollPeriod = await _db.EarningDeductionRecords
                    .Where(edr => !edr.DeletedOn.HasValue && edr.EmployeeId.HasValue && clientEmployeeIds.Contains(edr.EmployeeId.Value) && edr.PayrollPeriodFrom == command.PayrollPeriodFrom && edr.PayrollPeriodTo == command.PayrollPeriodTo)
                    .Include(edr => edr.EarningDeduction)
                    .ToListAsync();

                var loans = await _db.Loans
                    .Where(l => !l.DeletedOn.HasValue && clientEmployeeIds.Contains(l.EmployeeId.Value) && l.RemainingBalance > 0 && !l.ZeroedOutOn.HasValue && DbFunctions.TruncateTime(l.StartDeductionDate) <= DbFunctions.TruncateTime(command.PayrollPeriodTo))
                    .ToListAsync();

                loans = loans.Where(l => l.LoanPayrollPeriods.Contains(command.PayrollPeriod.Value)).ToList();

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

                var previousPayrollProcessBatchesInMonth = await _db.PayrollProcessBatches
                    .Where(ppb => ppb.ClientId == client.Id && ppb.PayrollPeriodMonth == client.PayrollPeriodMonth && ppb.PayrollPeriodFrom.Value.Year == command.PayrollPeriodFrom.Value.Year && ppb.PayrollPeriod < command.PayrollPeriod)
                    .ToListAsync();

                foreach (var employee in clientEmployees)
                {
                    var employeeDtrsForPayrollPeriod = dailyTimeRecordsForPayrollPeriod.Where(dtr => dtr.EmployeeId == employee.Id);
                    var employeeOtsForPayrollPeriod = overtimesForPayrollPeriod.Where(ot => ot.EmployeeId == employee.Id);
                    var employeeEdrsForPayrollPeriod = earningDeductionRecordsForPayrollPeriod.Where(ed => ed.EmployeeId == employee.Id);
                    var employeeLoans = loans.Where(l => l.EmployeeId == employee.Id);

                    if (!employeeDtrsForPayrollPeriod.Any()) continue;

                    var payrollRecord = new PayrollRecord
                    {
                        AddedOn = now,
                        COLADailyValue = employeeDtrsForPayrollPeriod.Sum(dtr => dtr.COLADailyValue),
                        COLAHourlyValue = employeeDtrsForPayrollPeriod.Sum(dtr => dtr.COLAHourlyValue + dtr.COLAHourlyOTValue),
                        DaysWorkedValue = employeeDtrsForPayrollPeriod.Sum(dtr => dtr.DaysWorkedValue),
                        DeductionsValue = employeeEdrsForPayrollPeriod.Where(ed => ed.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(ed => ed.Amount),
                        EarningsValue = employeeEdrsForPayrollPeriod.Where(ed => ed.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(ed => ed.Amount),
                        EmployeeId = employee.Id,
                        HoursLateValue = employeeDtrsForPayrollPeriod.Sum(dtr => dtr.HoursLateValue),
                        HoursUndertimeValue = employeeDtrsForPayrollPeriod.Sum(dtr => dtr.HoursUndertimeValue),
                        HoursWorkedValue = employeeDtrsForPayrollPeriod.Sum(dtr => dtr.HoursWorkedValue),
                        LoanPaymentValue = employeeLoans.Any() ? employeeLoans.Sum(l => l.RemainingBalance > l.DeductionAmount ? l.DeductionAmount : l.RemainingBalance) : null,
                        OvertimeValue = employeeOtsForPayrollPeriod.Sum(ot => ot.NumberOfHoursValue)
                    };

                    var dailyTimeRecordsForPreviousPayrollProcessBatches = new List<DailyTimeRecord>();
                    var overtimesForPreviousPayrollProcessBatches = new List<Overtime>();
                    var earningDeductionRecordsForPreviousPayrollProcessBatches = new List<EarningDeductionRecord>();
                    if (shouldDeductSSS || shouldDeductPHIC)
                    {
                        dailyTimeRecordsForPreviousPayrollProcessBatches = await GetDailyTimeRecordsForPreviousPayrollProcessBatches(employee, previousPayrollProcessBatchesInMonth);
                        overtimesForPreviousPayrollProcessBatches = await GetOvertimesForPreviousPayrollProcessBatches(employee, previousPayrollProcessBatchesInMonth);
                        earningDeductionRecordsForPreviousPayrollProcessBatches = await GetEarningDeductionRecordsForPreviousPayrollProcessBatches(employee, previousPayrollProcessBatchesInMonth);
                    }

                    if (shouldDeductSSS)
                    {
                        var sssDeductionBasis = GetSSSDeductionBasis(employee, client, dailyTimeRecordsForPreviousPayrollProcessBatches, employeeDtrsForPayrollPeriod, overtimesForPreviousPayrollProcessBatches, overtimesForPayrollPeriod, earningDeductionRecordsForPreviousPayrollProcessBatches, employeeEdrsForPayrollPeriod);
                        payrollRecord.SSSValueEmployee = ComputeSSSEmployee(sssDeductionBasis, sssRecords);
                        payrollRecord.SSSValueEmployer = ComputeSSSEmployer(sssDeductionBasis, sssRecords);
                    }

                    if (shouldDeductPHIC)
                    {
                        var phicDeductionBasis = GetPHICDeductionBasis(employee, client, dailyTimeRecordsForPreviousPayrollProcessBatches, employeeDtrsForPayrollPeriod, overtimesForPreviousPayrollProcessBatches, overtimesForPayrollPeriod, earningDeductionRecordsForPreviousPayrollProcessBatches, employeeEdrsForPayrollPeriod);
                        payrollRecord.PHICValueEmployee = ComputePHICEmployee(phicDeductionBasis);
                        payrollRecord.PHICValueEmployer = ComputePHICEmployer(phicDeductionBasis);
                    }

                    if (shouldDeductPagIbig) payrollRecord.PagIbigValue = ComputePagIbig(employee, client, employeeDtrsForPayrollPeriod, employeeOtsForPayrollPeriod, employeeEdrsForPayrollPeriod);
                    if (shouldDeductTax) payrollRecord.TaxValue = ComputeTax(employee, client, employeeDtrsForPayrollPeriod, employeeOtsForPayrollPeriod, employeeEdrsForPayrollPeriod);

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

            private async Task<List<DailyTimeRecord>> GetDailyTimeRecordsForPreviousPayrollProcessBatches(Employee employee, IEnumerable<PayrollProcessBatch> previousPayrollProcessBatchesInMonth)
            {
                var dailyTimeRecordsForPreviousBatches = new List<DailyTimeRecord>();

                foreach (var payrollProcessBatch in previousPayrollProcessBatchesInMonth)
                {
                    var dailyTimeRecordsForBatch = await _db
                        .DailyTimeRecords
                        .Where(dtr => dtr.EmployeeId == employee.Id && dtr.PayrollPeriodFrom == payrollProcessBatch.PayrollPeriodFrom && dtr.PayrollPeriodTo == payrollProcessBatch.PayrollPeriodTo)
                        .ToListAsync();

                    dailyTimeRecordsForPreviousBatches.AddRange(dailyTimeRecordsForBatch);
                }

                return dailyTimeRecordsForPreviousBatches;
            }

            private async Task<List<Overtime>> GetOvertimesForPreviousPayrollProcessBatches(Employee employee, List<PayrollProcessBatch> previousPayrollProcessBatchesInMonth)
            {
                var overtimesForPreviousBatches = new List<Overtime>();

                foreach (var payrollProcessBatch in previousPayrollProcessBatchesInMonth)
                {
                    var dailyTimeRecordsForBatch = await _db
                        .Overtimes
                        .Where(ot => ot.EmployeeId == employee.Id && ot.PayrollPeriodFrom == payrollProcessBatch.PayrollPeriodFrom && ot.PayrollPeriodTo == payrollProcessBatch.PayrollPeriodTo)
                        .ToListAsync();

                    overtimesForPreviousBatches.AddRange(dailyTimeRecordsForBatch);
                }

                return overtimesForPreviousBatches;
            }

            private async Task<List<EarningDeductionRecord>> GetEarningDeductionRecordsForPreviousPayrollProcessBatches(Employee employee, List<PayrollProcessBatch> previousPayrollProcessBatchesInMonth)
            {
                var earningDeductionRecordsForPreviousBatches = new List<EarningDeductionRecord>();

                foreach (var payrollProcessBatch in previousPayrollProcessBatchesInMonth)
                {
                    var dailyTimeRecordsForBatch = await _db
                        .EarningDeductionRecords
                        .Include(edr => edr.EarningDeduction)
                        .Where(edr => edr.EmployeeId == employee.Id && edr.PayrollPeriodFrom == payrollProcessBatch.PayrollPeriodFrom && edr.PayrollPeriodTo == payrollProcessBatch.PayrollPeriodTo)
                        .ToListAsync();

                    earningDeductionRecordsForPreviousBatches.AddRange(dailyTimeRecordsForBatch);
                }

                return earningDeductionRecordsForPreviousBatches;
            }

            private decimal GetSSSDeductionBasis(Employee employee, Client client, List<DailyTimeRecord> dailyTimeRecordsForPreviousPayrollProcessBatches, IEnumerable<DailyTimeRecord> employeeDtrsForPayrollPeriod, List<Overtime> overtimesForPreviousPayrollProcessBatches, List<Overtime> overtimesForPayrollPeriod, List<EarningDeductionRecord> earningDeductionRecordsForPreviousPayrollProcessBatches, IEnumerable<EarningDeductionRecord> employeeEdrsForPayrollPeriod)
            {
                var deductionBasis = 0m;

                if (client.SSSBasic == true) deductionBasis += dailyTimeRecordsForPreviousPayrollProcessBatches.Sum(dtr => dtr.DaysWorkedValue.GetValueOrDefault() + dtr.HoursWorkedValue.GetValueOrDefault()) + employeeDtrsForPayrollPeriod.Sum(dtr => dtr.DaysWorkedValue.GetValueOrDefault() + dtr.HoursWorkedValue.GetValueOrDefault());
                if (client.SSSCola == true) deductionBasis += dailyTimeRecordsForPreviousPayrollProcessBatches.Sum(dtr => dtr.COLADailyValue.GetValueOrDefault() + dtr.COLAHourlyValue.GetValueOrDefault() + dtr.COLAHourlyOTValue.GetValueOrDefault()) + employeeDtrsForPayrollPeriod.Sum(dtr => dtr.COLADailyValue.GetValueOrDefault() + dtr.COLAHourlyValue.GetValueOrDefault() + dtr.COLAHourlyOTValue.GetValueOrDefault());
                if (client.SSSOvertime == true) deductionBasis += overtimesForPreviousPayrollProcessBatches.Sum(ot => ot.NumberOfHoursValue.GetValueOrDefault()) + overtimesForPayrollPeriod.Sum(ot => ot.NumberOfHoursValue.GetValueOrDefault());
                if (client.SSSEarnings == true) deductionBasis += earningDeductionRecordsForPreviousPayrollProcessBatches.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(edr => edr.Amount.GetValueOrDefault()) + employeeEdrsForPayrollPeriod.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(edr => edr.Amount.GetValueOrDefault());
                if (client.SSSDeductions == true) deductionBasis -= earningDeductionRecordsForPreviousPayrollProcessBatches.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(edr => edr.Amount.GetValueOrDefault()) + employeeEdrsForPayrollPeriod.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(edr => edr.Amount.GetValueOrDefault());
                if (client.SSSUndertime == true) deductionBasis -= dailyTimeRecordsForPreviousPayrollProcessBatches.Sum(dtr => dtr.HoursLateValue.GetValueOrDefault() + dtr.HoursUndertimeValue.GetValueOrDefault()) + employeeDtrsForPayrollPeriod.Sum(dtr => dtr.HoursLateValue.GetValueOrDefault() + dtr.HoursUndertimeValue.GetValueOrDefault());

                return deductionBasis;
            }

            private decimal GetPHICDeductionBasis(Employee employee, Client client, List<DailyTimeRecord> dailyTimeRecordsForPreviousPayrollProcessBatches, IEnumerable<DailyTimeRecord> employeeDtrsForPayrollPeriod, List<Overtime> overtimesForPreviousPayrollProcessBatches, List<Overtime> overtimesForPayrollPeriod, List<EarningDeductionRecord> earningDeductionRecordsForPreviousPayrollProcessBatches, IEnumerable<EarningDeductionRecord> employeeEdrsForPayrollPeriod)
            {
                var deductionBasis = 0m;

                if (client.PHICBasic == true) deductionBasis += dailyTimeRecordsForPreviousPayrollProcessBatches.Sum(dtr => dtr.DaysWorkedValue.GetValueOrDefault() + dtr.HoursWorkedValue.GetValueOrDefault()) + employeeDtrsForPayrollPeriod.Sum(dtr => dtr.DaysWorkedValue.GetValueOrDefault() + dtr.HoursWorkedValue.GetValueOrDefault());
                if (client.PHICCola == true) deductionBasis += dailyTimeRecordsForPreviousPayrollProcessBatches.Sum(dtr => dtr.COLADailyValue.GetValueOrDefault() + dtr.COLAHourlyValue.GetValueOrDefault() + dtr.COLAHourlyOTValue.GetValueOrDefault()) + employeeDtrsForPayrollPeriod.Sum(dtr => dtr.COLADailyValue.GetValueOrDefault() + dtr.COLAHourlyValue.GetValueOrDefault() + dtr.COLAHourlyOTValue.GetValueOrDefault());
                if (client.PHICOvertime == true) deductionBasis += overtimesForPreviousPayrollProcessBatches.Sum(ot => ot.NumberOfHoursValue.GetValueOrDefault()) + overtimesForPayrollPeriod.Sum(ot => ot.NumberOfHoursValue.GetValueOrDefault());
                if (client.PHICEarnings == true) deductionBasis += earningDeductionRecordsForPreviousPayrollProcessBatches.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(edr => edr.Amount.GetValueOrDefault()) + employeeEdrsForPayrollPeriod.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(edr => edr.Amount.GetValueOrDefault());
                if (client.PHICDeductions == true) deductionBasis -= earningDeductionRecordsForPreviousPayrollProcessBatches.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(edr => edr.Amount.GetValueOrDefault()) + employeeEdrsForPayrollPeriod.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(edr => edr.Amount.GetValueOrDefault());
                if (client.PHICUndertime == true) deductionBasis -= dailyTimeRecordsForPreviousPayrollProcessBatches.Sum(dtr => dtr.HoursLateValue.GetValueOrDefault() + dtr.HoursUndertimeValue.GetValueOrDefault()) + employeeDtrsForPayrollPeriod.Sum(dtr => dtr.HoursLateValue.GetValueOrDefault() + dtr.HoursUndertimeValue.GetValueOrDefault());

                return deductionBasis;
            }

            private decimal? ComputeSSSEmployee(decimal deductionBasis, IEnumerable<SSSRecord> sssRecords)
            {
                SSSRecord matchingRange = null;

                try
                {
                    matchingRange = sssRecords
                        .OrderBy(s => s.Range1)
                        .First(s => s.Range1 > deductionBasis);
                }
                catch
                {
                    throw new Exception($"Matching SSS range not found for amount {deductionBasis}");
                }

                return matchingRange.Employee;
            }

            private decimal? ComputeSSSEmployer(decimal deductionBasis, IEnumerable<SSSRecord> sssRecords)
            {
                SSSRecord matchingRange = null;

                try
                {
                    matchingRange = sssRecords
                        .OrderBy(s => s.Range1)
                        .First(s => s.Range1 > deductionBasis);
                }
                catch
                {
                    throw new Exception($"Matching SSS range not found for amount {deductionBasis}");
                }

                return matchingRange.Employer;
            }

            private decimal ComputePHICEmployee(decimal deductionBasis) => (decimal)(0.0275 / 2) * deductionBasis;
            private decimal ComputePHICEmployer(decimal deductionBasis) => (decimal)(0.0275 / 2) * deductionBasis;

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
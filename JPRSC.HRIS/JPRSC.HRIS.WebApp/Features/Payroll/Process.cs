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
            public Month? PayrollPeriodMonth { get; set; }
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

                RuleFor(c => c.PayrollPeriodMonth)
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

                var phicSettings = await _db.PhicRecords
                    .SingleOrDefaultAsync();

                if (!phicSettings.Percentage.HasValue) throw new Exception("No PHIC percentage set.");
                if (!phicSettings.MaximumDeduction.HasValue) throw new Exception("No PHIC maximum deduction set.");
                if (!phicSettings.MinimumDeduction.HasValue) throw new Exception("No PHIC minimum deduction set.");
                if (!phicSettings.EmployeePercentageShare.HasValue || !phicSettings.EmployerPercentageShare.HasValue) throw new Exception("No PHIC share percentages set.");

                var clientEmployees = await _db.Employees
                    .Where(e => !e.DeletedOn.HasValue && e.ClientId == command.ClientId && e.DailyRate.HasValue)
                    .Include(e => e.PagIbigRecord)
                    .ToListAsync();

                var clientEmployeeIds = clientEmployees.Select(e => e.Id).ToList();

                var dailyTimeRecordsForPayrollPeriod = await _db.DailyTimeRecords
                    .Include(dtr => dtr.PayrollProcessBatch)
                    .Where(dtr => !dtr.DeletedOn.HasValue && dtr.EmployeeId.HasValue && clientEmployeeIds.Contains(dtr.EmployeeId.Value) && dtr.PayrollPeriodFrom == command.PayrollPeriodFrom && dtr.PayrollPeriodTo == command.PayrollPeriodTo && dtr.PayrollPeriodMonth == command.PayrollPeriodMonth && (!dtr.PayrollProcessBatchId.HasValue || !dtr.PayrollProcessBatch.EndProcessedOn.HasValue))
                    .ToListAsync();

                var overtimesForPayrollPeriod = await _db.Overtimes
                    .Where(ot => !ot.DeletedOn.HasValue && ot.EmployeeId.HasValue && clientEmployeeIds.Contains(ot.EmployeeId.Value) && ot.PayrollPeriodFrom == command.PayrollPeriodFrom && ot.PayrollPeriodTo == command.PayrollPeriodTo && ot.PayrollPeriodMonth == command.PayrollPeriodMonth)
                    .ToListAsync();

                var earningDeductionRecordsForPayrollPeriod = await _db.EarningDeductionRecords
                    .Where(edr => !edr.DeletedOn.HasValue && edr.EmployeeId.HasValue && clientEmployeeIds.Contains(edr.EmployeeId.Value) && edr.PayrollPeriodFrom == command.PayrollPeriodFrom && edr.PayrollPeriodTo == command.PayrollPeriodTo && edr.PayrollPeriodMonth == command.PayrollPeriodMonth)
                    .Include(edr => edr.EarningDeduction)
                    .ToListAsync();

                var loans = await _db.Loans
                    .Where(l => !l.DeletedOn.HasValue && clientEmployeeIds.Contains(l.EmployeeId.Value) && l.RemainingBalance > 0 && !l.ZeroedOutOn.HasValue && DbFunctions.TruncateTime(l.StartDeductionDate) <= DbFunctions.TruncateTime(command.PayrollPeriodTo))
                    .ToListAsync();

                loans = loans.Where(l => l.LoanPayrollPeriods.Contains(command.PayrollPeriod.Value)).ToList();

                var sssRecords = await _db.SSSRecords
                    .ToListAsync();

                var shouldDeductSSS = client.SSSPayrollPeriods.Contains(command.PayrollPeriod.Value);
                var shouldIncludeFifthPayrollPeriodFromPreviousMonthSSS = shouldDeductSSS && client.NumberOfPayrollPeriodsAMonth == 5 && command.PayrollPeriod == 4;

                var shouldDeductPHIC = client.PHICPayrollPeriods.Contains(command.PayrollPeriod.Value);
                var shouldIncludeFifthPayrollPeriodFromPreviousMonthPHIC = shouldDeductPHIC && client.NumberOfPayrollPeriodsAMonth == 5 && command.PayrollPeriod == 4;

                var shouldDeductPagIbig = client.PagIbigPayrollPeriods.Contains(command.PayrollPeriod.Value);
                var shouldIncludeFifthPayrollPeriodFromPreviousMonthPagIbig = shouldDeductPagIbig && client.NumberOfPayrollPeriodsAMonth == 5 && command.PayrollPeriod == 4;

                var shouldDeductTax = client.TaxPayrollPeriods.Contains(command.PayrollPeriod.Value);

                var existingPayrollProcessBatch = await _db.PayrollProcessBatches
                    .FirstOrDefaultAsync(ppb => !ppb.DeletedOn.HasValue && !ppb.DateOverwritten.HasValue && !ppb.EndProcessedOn.HasValue && ppb.ClientId == command.ClientId && ppb.PayrollPeriod == command.PayrollPeriod && ppb.PayrollPeriodFrom == command.PayrollPeriodFrom && ppb.PayrollPeriodTo == command.PayrollPeriodTo && ppb.PayrollPeriodMonth == command.PayrollPeriodMonth);

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
                    PayrollPeriodMonth = command.PayrollPeriodMonth,
                    PayrollPeriodTo = command.PayrollPeriodTo
                };

                var previousPayrollProcessBatchesInMonth = new List<PayrollProcessBatch>();
                var payrollProcessBatchesFromFifthPayrollPeriodOfPreviousMonth = new List<PayrollProcessBatch>();

                if (command.PayrollPeriodMonth != Month.January)
                {
                    previousPayrollProcessBatchesInMonth = await _db.PayrollProcessBatches
                        .Where(ppb => ppb.ClientId == client.Id && ppb.PayrollPeriodFrom.Value.Year == command.PayrollPeriodFrom.Value.Year && ppb.PayrollPeriod < command.PayrollPeriod && ppb.PayrollPeriodMonth == command.PayrollPeriodMonth)
                        .OrderByDescending(ppb => ppb.PayrollPeriodFrom)
                        .ToListAsync();

                    var previousPayrollPeriodMonth = (Month)Enum.Parse(typeof(Month), ((int)command.PayrollPeriodMonth - 1).ToString());

                    payrollProcessBatchesFromFifthPayrollPeriodOfPreviousMonth = await _db.PayrollProcessBatches
                        .Where(ppb => ppb.ClientId == client.Id && ppb.PayrollPeriodFrom.Value.Year == command.PayrollPeriodFrom.Value.Year && ppb.PayrollPeriod == 5 && ppb.PayrollPeriodMonth == previousPayrollPeriodMonth)
                        .OrderByDescending(ppb => ppb.PayrollPeriodFrom)
                        .ToListAsync();
                }
                else
                {
                    previousPayrollProcessBatchesInMonth = await _db.PayrollProcessBatches
                        .Where(ppb => ppb.ClientId == client.Id && (ppb.PayrollPeriodFrom.Value.Year == command.PayrollPeriodFrom.Value.Year || (ppb.PayrollPeriodFrom.Value.Year == command.PayrollPeriodFrom.Value.Year - 1 && ppb.PayrollPeriodFrom.Value.Month == 12)) && ppb.PayrollPeriod < command.PayrollPeriod && ppb.PayrollPeriodMonth == command.PayrollPeriodMonth)
                        .OrderByDescending(ppb => ppb.PayrollPeriodFrom)
                        .ToListAsync();

                    payrollProcessBatchesFromFifthPayrollPeriodOfPreviousMonth = await _db.PayrollProcessBatches
                        .Where(ppb => ppb.ClientId == client.Id && (ppb.PayrollPeriodFrom.Value.Year == command.PayrollPeriodFrom.Value.Year - 1 && ppb.PayrollPeriodFrom.Value.Month == 12) && ppb.PayrollPeriod == 5 && ppb.PayrollPeriodMonth == Month.December)
                        .OrderByDescending(ppb => ppb.PayrollPeriodFrom)
                        .ToListAsync();
                }                

                var systemSettings = await _db.SystemSettings.SingleAsync();

                foreach (var employee in clientEmployees)
                {
                    var employeeDtrsForPayrollPeriod = dailyTimeRecordsForPayrollPeriod.Where(dtr => dtr.EmployeeId == employee.Id).ToList();
                    var employeeOtsForPayrollPeriod = overtimesForPayrollPeriod.Where(ot => ot.EmployeeId == employee.Id).ToList();
                    var employeeEdrsForPayrollPeriod = earningDeductionRecordsForPayrollPeriod.Where(ed => ed.EmployeeId == employee.Id).ToList();
                    var employeeLoans = loans.Where(l => l.EmployeeId == employee.Id).ToList();

                    if (!employeeDtrsForPayrollPeriod.Any()) continue;

                    var payrollRecord = new PayrollRecord
                    {
                        AddedOn = now,
                        COLADailyValue = employeeDtrsForPayrollPeriod.Sum(dtr => dtr.COLADailyValue.GetValueOrDefault()),
                        COLAHourlyValue = employeeDtrsForPayrollPeriod.Sum(dtr => dtr.COLAHourlyValue.GetValueOrDefault() + dtr.COLAHourlyOTValue.GetValueOrDefault()),
                        COLAMonthlyValue = employeeDtrsForPayrollPeriod.Sum(dtr => dtr.COLAMonthlyValue.GetValueOrDefault()),
                        DaysWorkedValue = employeeDtrsForPayrollPeriod.Sum(dtr => dtr.DaysWorkedValue.GetValueOrDefault()),
                        DeductionsValue = employeeEdrsForPayrollPeriod.Where(ed => ed.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(ed => ed.Amount.GetValueOrDefault()),
                        EarningsValue = employeeEdrsForPayrollPeriod.Where(ed => ed.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(ed => ed.Amount.GetValueOrDefault()),
                        EmployeeId = employee.Id,
                        HoursLateValue = employeeDtrsForPayrollPeriod.Sum(dtr => dtr.HoursLateValue.GetValueOrDefault()),
                        HoursUndertimeValue = employeeDtrsForPayrollPeriod.Sum(dtr => dtr.HoursUndertimeValue.GetValueOrDefault()),
                        HoursWorkedValue = employeeDtrsForPayrollPeriod.Sum(dtr => dtr.HoursWorkedValue.GetValueOrDefault()),
                        OvertimeValue = employeeOtsForPayrollPeriod.Sum(ot => ot.NumberOfHoursValue.GetValueOrDefault())
                    };

                    IList<DailyTimeRecord> dailyTimeRecordsForPreviousPayrollProcessBatches = new List<DailyTimeRecord>();
                    IList<Overtime> overtimesForPreviousPayrollProcessBatches = new List<Overtime>();
                    IList<EarningDeductionRecord> earningDeductionRecordsForPreviousPayrollProcessBatches = new List<EarningDeductionRecord>();
                    if (shouldDeductSSS || shouldDeductPHIC || shouldDeductPagIbig)
                    {
                        dailyTimeRecordsForPreviousPayrollProcessBatches = await GetDailyTimeRecordsOfPayrollProcessBatches(employee, previousPayrollProcessBatchesInMonth);
                        overtimesForPreviousPayrollProcessBatches = await GetOvertimesOfPayrollProcessBatches(employee, previousPayrollProcessBatchesInMonth);
                        earningDeductionRecordsForPreviousPayrollProcessBatches = await GetEarningDeductionRecordsOfPayrollProcessBatches(employee, previousPayrollProcessBatchesInMonth);
                    }

                    IList<DailyTimeRecord> dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonth = new List<DailyTimeRecord>();
                    IList<Overtime> overtimesFromFifthPayrollPeriodOfPreviousMonth = new List<Overtime>();
                    IList<EarningDeductionRecord> earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonth = new List<EarningDeductionRecord>();
                    if (shouldIncludeFifthPayrollPeriodFromPreviousMonthSSS || shouldIncludeFifthPayrollPeriodFromPreviousMonthPHIC || shouldIncludeFifthPayrollPeriodFromPreviousMonthPagIbig)
                    {
                        dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonth = await GetDailyTimeRecordsOfPayrollProcessBatches(employee, payrollProcessBatchesFromFifthPayrollPeriodOfPreviousMonth);
                        overtimesFromFifthPayrollPeriodOfPreviousMonth = await GetOvertimesOfPayrollProcessBatches(employee, payrollProcessBatchesFromFifthPayrollPeriodOfPreviousMonth);
                        earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonth = await GetEarningDeductionRecordsOfPayrollProcessBatches(employee, payrollProcessBatchesFromFifthPayrollPeriodOfPreviousMonth);
                    }

                    if (shouldDeductSSS && client.SSSExempt != true && employee.SSSExempt != true)
                    {
                        var dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonthToInclude = shouldIncludeFifthPayrollPeriodFromPreviousMonthSSS ? dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonth : new List<DailyTimeRecord>();
                        var overtimesFromFifthPayrollPeriodOfPreviousMonthToInclude = shouldIncludeFifthPayrollPeriodFromPreviousMonthSSS ? overtimesFromFifthPayrollPeriodOfPreviousMonth : new List<Overtime>();
                        var earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonthToInclude = shouldIncludeFifthPayrollPeriodFromPreviousMonthSSS ? earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonth : new List<EarningDeductionRecord>();

                        var sssDeductionBasis = GetSSSDeductionBasis(employee, client, dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonthToInclude, dailyTimeRecordsForPreviousPayrollProcessBatches, employeeDtrsForPayrollPeriod, overtimesFromFifthPayrollPeriodOfPreviousMonthToInclude, overtimesForPreviousPayrollProcessBatches, overtimesForPayrollPeriod, earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonthToInclude, earningDeductionRecordsForPreviousPayrollProcessBatches, employeeEdrsForPayrollPeriod);
                        payrollRecord.SSSDeductionBasis = sssDeductionBasis;
                        payrollRecord.SSSValueEmployee = ComputeSSSEmployee(sssDeductionBasis, sssRecords);
                        payrollRecord.SSSValueEmployer = ComputeSSSEmployer(sssDeductionBasis, sssRecords);
                    }

                    if (shouldDeductPHIC && client.PHICExempt != true && employee.PhilHealthExempt != true)
                    {
                        var dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonthToInclude = shouldIncludeFifthPayrollPeriodFromPreviousMonthPHIC ? dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonth : new List<DailyTimeRecord>();
                        var overtimesFromFifthPayrollPeriodOfPreviousMonthToInclude = shouldIncludeFifthPayrollPeriodFromPreviousMonthPHIC ? overtimesFromFifthPayrollPeriodOfPreviousMonth : new List<Overtime>();
                        var earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonthToInclude = shouldIncludeFifthPayrollPeriodFromPreviousMonthPHIC ? earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonth : new List<EarningDeductionRecord>();

                        var phicDeductionBasis = GetPHICDeductionBasis(employee, client, dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonthToInclude, dailyTimeRecordsForPreviousPayrollProcessBatches, employeeDtrsForPayrollPeriod, overtimesFromFifthPayrollPeriodOfPreviousMonthToInclude, overtimesForPreviousPayrollProcessBatches, overtimesForPayrollPeriod, earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonthToInclude, earningDeductionRecordsForPreviousPayrollProcessBatches, employeeEdrsForPayrollPeriod);
                        payrollRecord.PHICDeductionBasis = phicDeductionBasis;
                        payrollRecord.PHICValueEmployee = ComputePHICEmployee(phicDeductionBasis, phicSettings);
                        payrollRecord.PHICValueEmployer = ComputePHICEmployer(phicDeductionBasis, phicSettings);
                    }

                    if (shouldDeductPagIbig && client.PHICExempt != true && employee.PagIbigExempt != true)
                    {
                        var dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonthToInclude = shouldIncludeFifthPayrollPeriodFromPreviousMonthPagIbig ? dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonth : new List<DailyTimeRecord>();
                        var overtimesFromFifthPayrollPeriodOfPreviousMonthToInclude = shouldIncludeFifthPayrollPeriodFromPreviousMonthPagIbig ? overtimesFromFifthPayrollPeriodOfPreviousMonth : new List<Overtime>();
                        var earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonthToInclude = shouldIncludeFifthPayrollPeriodFromPreviousMonthPagIbig ? earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonth : new List<EarningDeductionRecord>();

                        var pagIbigDeductionBasis = GetPagIbigDeductionBasis(employee, client, dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonthToInclude, dailyTimeRecordsForPreviousPayrollProcessBatches, employeeDtrsForPayrollPeriod, overtimesFromFifthPayrollPeriodOfPreviousMonthToInclude, overtimesForPreviousPayrollProcessBatches, overtimesForPayrollPeriod, earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonthToInclude, earningDeductionRecordsForPreviousPayrollProcessBatches, employeeEdrsForPayrollPeriod);
                        payrollRecord.PagIbigDeductionBasis = pagIbigDeductionBasis;
                        payrollRecord.PagIbigValueEmployee = ComputePagIbigEmployee(pagIbigDeductionBasis, employee.PagIbigRecord);
                        payrollRecord.PagIbigValueEmployer = ComputePagIbigEmployer(pagIbigDeductionBasis, employee.PagIbigRecord);
                    }

                    if (shouldDeductTax && client.TaxExempt != true && employee.TaxExempt != true) payrollRecord.TaxValueEmployee = ComputeTax(employee, client, employeeDtrsForPayrollPeriod, employeeOtsForPayrollPeriod, employeeEdrsForPayrollPeriod);
                                        
                    if (client.LoanExempt != true && employee.LoanExempt != true)
                    {
                        var sampleLoanPaymentValue = employeeLoans.Sum(l => l.RemainingBalance.GetValueOrDefault() > l.DeductionAmount.GetValueOrDefault() ? l.DeductionAmount.GetValueOrDefault() : l.RemainingBalance.GetValueOrDefault());

                        payrollRecord.NetPayValue = NetPayHelper.GetNetPay(systemSettings, payrollRecord.BasicPayValue, payrollRecord.TotalEarningsValue, payrollRecord.TotalGovDeductionsValue, payrollRecord.DeductionsValue.GetValueOrDefault(), sampleLoanPaymentValue, out bool govDeductionsDeducted, out bool loansDeducted, out bool anythingDeducted, out decimal deductionBasis);

                        payrollRecord.GovDeductionsDeducted = govDeductionsDeducted;
                        payrollRecord.LoansDeducted = loansDeducted;
                        payrollRecord.AnythingDeducted = anythingDeducted;
                        payrollRecord.DeductionBasis = deductionBasis;

                        payrollProcessBatch.PayrollRecords.Add(payrollRecord);

                        if (loansDeducted)
                        {
                            payrollRecord.LoanPaymentValue = sampleLoanPaymentValue;

                            foreach (var loan in employeeLoans)
                            {
                                var deductionAmount = loan.RemainingBalance.GetValueOrDefault() > loan.DeductionAmount.GetValueOrDefault() ? loan.DeductionAmount.GetValueOrDefault() : loan.RemainingBalance.GetValueOrDefault();

                                loan.RemainingBalance -= deductionAmount;

                                if (!loan.AmountPaid.HasValue) loan.AmountPaid = 0;

                                loan.AmountPaid += deductionAmount;

                                payrollRecord.LoanDeductions.Add(new LoanDeduction
                                {
                                    DeductionAmount = deductionAmount,
                                    LoanId = loan.Id,
                                    NewRemainingBalance = loan.RemainingBalance,
                                    NewAmountPaid = loan.AmountPaid
                                });
                            }
                        }
                    }
                    else
                    {
                        payrollRecord.NetPayValue = NetPayHelper.GetNetPayLoanExempt(systemSettings, payrollRecord.BasicPayValue, payrollRecord.TotalEarningsValue, payrollRecord.TotalGovDeductionsValue, payrollRecord.DeductionsValue.GetValueOrDefault(), out bool govDeductionsDeducted, out bool anythingDeducted, out decimal deductionBasis);

                        payrollRecord.GovDeductionsDeducted = govDeductionsDeducted;
                        payrollRecord.LoansDeducted = false;
                        payrollRecord.AnythingDeducted = anythingDeducted;
                        payrollRecord.DeductionBasis = deductionBasis;

                        payrollProcessBatch.PayrollRecords.Add(payrollRecord);
                    }
                }

                _db.PayrollProcessBatches.Add(payrollProcessBatch);

                await _db.SaveChangesAsync();

                foreach (var dtr in dailyTimeRecordsForPayrollPeriod)
                {
                    dtr.PayrollProcessBatchId = payrollProcessBatch.Id;
                }

                await _db.SaveChangesAsync();

                return new CommandResult();
            }

            private async Task<IList<DailyTimeRecord>> GetDailyTimeRecordsOfPayrollProcessBatches(Employee employee, IList<PayrollProcessBatch> payrollProcessBatches)
            {
                var dailyTimeRecords = new List<DailyTimeRecord>();

                foreach (var payrollProcessBatch in payrollProcessBatches)
                {
                    var dailyTimeRecordsForBatch = await _db
                        .DailyTimeRecords
                        .Where(dtr => dtr.EmployeeId == employee.Id && dtr.PayrollPeriodFrom == payrollProcessBatch.PayrollPeriodFrom && dtr.PayrollPeriodTo == payrollProcessBatch.PayrollPeriodTo)
                        .ToListAsync();

                    dailyTimeRecords.AddRange(dailyTimeRecordsForBatch);
                }

                return dailyTimeRecords;
            }

            private async Task<IList<Overtime>> GetOvertimesOfPayrollProcessBatches(Employee employee, IList<PayrollProcessBatch> payrollProcessBatches)
            {
                var overtimes = new List<Overtime>();

                foreach (var payrollProcessBatch in payrollProcessBatches)
                {
                    var overtimesForBatch = await _db
                        .Overtimes
                        .Where(ot => ot.EmployeeId == employee.Id && ot.PayrollPeriodFrom == payrollProcessBatch.PayrollPeriodFrom && ot.PayrollPeriodTo == payrollProcessBatch.PayrollPeriodTo)
                        .ToListAsync();

                    overtimes.AddRange(overtimesForBatch);
                }

                return overtimes;
            }

            private async Task<IList<EarningDeductionRecord>> GetEarningDeductionRecordsOfPayrollProcessBatches(Employee employee, IList<PayrollProcessBatch> payrollProcessBatches)
            {
                var earningDeductionRecords = new List<EarningDeductionRecord>();

                foreach (var payrollProcessBatch in payrollProcessBatches)
                {
                    var earningDeductionRecordsForBatch = await _db
                        .EarningDeductionRecords
                        .Include(edr => edr.EarningDeduction)
                        .Where(edr => edr.EmployeeId == employee.Id && edr.PayrollPeriodFrom == payrollProcessBatch.PayrollPeriodFrom && edr.PayrollPeriodTo == payrollProcessBatch.PayrollPeriodTo)
                        .ToListAsync();

                    earningDeductionRecords.AddRange(earningDeductionRecordsForBatch);
                }

                return earningDeductionRecords;
            }

            private decimal GetSSSDeductionBasis(Employee employee, Client client, IList<DailyTimeRecord> dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonth, IList<DailyTimeRecord> dailyTimeRecordsForPreviousPayrollProcessBatches, IList<DailyTimeRecord> employeeDtrsForPayrollPeriod, IList<Overtime> overtimesFromFifthPayrollPeriodOfPreviousMonth, IList<Overtime> overtimesForPreviousPayrollProcessBatches, IList<Overtime> overtimesForPayrollPeriod, IList<EarningDeductionRecord> earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonth, IList<EarningDeductionRecord> earningDeductionRecordsForPreviousPayrollProcessBatches, IList<EarningDeductionRecord> employeeEdrsForPayrollPeriod)
            {
                var deductionBasis = 0m;

                if (client.SSSBasic == true) deductionBasis += dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonth.Sum(dtr => dtr.TimeWorkedValue) + dailyTimeRecordsForPreviousPayrollProcessBatches.Sum(dtr => dtr.TimeWorkedValue) + employeeDtrsForPayrollPeriod.Sum(dtr => dtr.TimeWorkedValue);
                if (client.SSSCola == true) deductionBasis += dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonth.Sum(dtr => dtr.COLATotalValue) + dailyTimeRecordsForPreviousPayrollProcessBatches.Sum(dtr => dtr.COLATotalValue) + employeeDtrsForPayrollPeriod.Sum(dtr => dtr.COLATotalValue);
                if (client.SSSOvertime == true) deductionBasis += overtimesFromFifthPayrollPeriodOfPreviousMonth.Sum(ot => ot.NumberOfHoursValue.GetValueOrDefault()) + overtimesForPreviousPayrollProcessBatches.Sum(ot => ot.NumberOfHoursValue.GetValueOrDefault()) + overtimesForPayrollPeriod.Sum(ot => ot.NumberOfHoursValue.GetValueOrDefault());
                if (client.SSSEarnings == true) deductionBasis += earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonth.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(edr => edr.Amount.GetValueOrDefault()) + earningDeductionRecordsForPreviousPayrollProcessBatches.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(edr => edr.Amount.GetValueOrDefault()) + employeeEdrsForPayrollPeriod.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(edr => edr.Amount.GetValueOrDefault());
                if (client.SSSDeductions == true) deductionBasis -= earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonth.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(edr => edr.Amount.GetValueOrDefault()) + earningDeductionRecordsForPreviousPayrollProcessBatches.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(edr => edr.Amount.GetValueOrDefault()) + employeeEdrsForPayrollPeriod.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(edr => edr.Amount.GetValueOrDefault());
                if (client.SSSUndertime == true) deductionBasis -= dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonth.Sum(dtr => dtr.TimeNotWorkedValue) + dailyTimeRecordsForPreviousPayrollProcessBatches.Sum(dtr => dtr.TimeNotWorkedValue) + employeeDtrsForPayrollPeriod.Sum(dtr => dtr.TimeNotWorkedValue);

                return deductionBasis;
            }

            private decimal GetPHICDeductionBasis(Employee employee, Client client, IList<DailyTimeRecord> dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonth, IList<DailyTimeRecord> dailyTimeRecordsForPreviousPayrollProcessBatches, IList<DailyTimeRecord> employeeDtrsForPayrollPeriod, IList<Overtime> overtimesFromFifthPayrollPeriodOfPreviousMonth, IList<Overtime> overtimesForPreviousPayrollProcessBatches, IList<Overtime> overtimesForPayrollPeriod, IList<EarningDeductionRecord> earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonth, IList<EarningDeductionRecord> earningDeductionRecordsForPreviousPayrollProcessBatches, IList<EarningDeductionRecord> employeeEdrsForPayrollPeriod)
            {
                var deductionBasis = 0m;

                if (client.PHICBasic == true) deductionBasis += dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonth.Sum(dtr => dtr.TimeWorkedValue) + dailyTimeRecordsForPreviousPayrollProcessBatches.Sum(dtr => dtr.TimeWorkedValue) + employeeDtrsForPayrollPeriod.Sum(dtr => dtr.TimeWorkedValue);
                if (client.PHICCola == true) deductionBasis += dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonth.Sum(dtr => dtr.COLATotalValue) + dailyTimeRecordsForPreviousPayrollProcessBatches.Sum(dtr => dtr.COLATotalValue) + employeeDtrsForPayrollPeriod.Sum(dtr => dtr.COLATotalValue);
                if (client.PHICOvertime == true) deductionBasis += overtimesFromFifthPayrollPeriodOfPreviousMonth.Sum(ot => ot.NumberOfHoursValue.GetValueOrDefault()) + overtimesForPreviousPayrollProcessBatches.Sum(ot => ot.NumberOfHoursValue.GetValueOrDefault()) + overtimesForPayrollPeriod.Sum(ot => ot.NumberOfHoursValue.GetValueOrDefault());
                if (client.PHICEarnings == true) deductionBasis += earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonth.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(edr => edr.Amount.GetValueOrDefault()) + earningDeductionRecordsForPreviousPayrollProcessBatches.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(edr => edr.Amount.GetValueOrDefault()) + employeeEdrsForPayrollPeriod.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(edr => edr.Amount.GetValueOrDefault());
                if (client.PHICDeductions == true) deductionBasis -= earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonth.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(edr => edr.Amount.GetValueOrDefault()) + earningDeductionRecordsForPreviousPayrollProcessBatches.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(edr => edr.Amount.GetValueOrDefault()) + employeeEdrsForPayrollPeriod.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(edr => edr.Amount.GetValueOrDefault());
                if (client.PHICUndertime == true) deductionBasis -= dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonth.Sum(dtr => dtr.TimeNotWorkedValue) + dailyTimeRecordsForPreviousPayrollProcessBatches.Sum(dtr => dtr.TimeNotWorkedValue) + employeeDtrsForPayrollPeriod.Sum(dtr => dtr.TimeNotWorkedValue);

                return deductionBasis;
            }

            private decimal GetPagIbigDeductionBasis(Employee employee, Client client, IList<DailyTimeRecord> dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonth, IList<DailyTimeRecord> dailyTimeRecordsForPreviousPayrollProcessBatches, IList<DailyTimeRecord> employeeDtrsForPayrollPeriod, IList<Overtime> overtimesFromFifthPayrollPeriodOfPreviousMonth, IList<Overtime> overtimesForPreviousPayrollProcessBatches, IList<Overtime> overtimesForPayrollPeriod, IList<EarningDeductionRecord> earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonth, IList<EarningDeductionRecord> earningDeductionRecordsForPreviousPayrollProcessBatches, IList<EarningDeductionRecord> employeeEdrsForPayrollPeriod)
            {
                var deductionBasis = 0m;

                if (client.PagIbigBasic == true) deductionBasis += dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonth.Sum(dtr => dtr.TimeWorkedValue) + dailyTimeRecordsForPreviousPayrollProcessBatches.Sum(dtr => dtr.TimeWorkedValue) + employeeDtrsForPayrollPeriod.Sum(dtr => dtr.TimeWorkedValue);
                if (client.PagIbigCola == true) deductionBasis += dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonth.Sum(dtr => dtr.COLATotalValue) + dailyTimeRecordsForPreviousPayrollProcessBatches.Sum(dtr => dtr.COLATotalValue) + employeeDtrsForPayrollPeriod.Sum(dtr => dtr.COLATotalValue);
                if (client.PagIbigOvertime == true) deductionBasis += overtimesFromFifthPayrollPeriodOfPreviousMonth.Sum(ot => ot.NumberOfHoursValue.GetValueOrDefault()) + overtimesForPreviousPayrollProcessBatches.Sum(ot => ot.NumberOfHoursValue.GetValueOrDefault()) + overtimesForPayrollPeriod.Sum(ot => ot.NumberOfHoursValue.GetValueOrDefault());
                if (client.PagIbigEarnings == true) deductionBasis += earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonth.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(edr => edr.Amount.GetValueOrDefault()) + earningDeductionRecordsForPreviousPayrollProcessBatches.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(edr => edr.Amount.GetValueOrDefault()) + employeeEdrsForPayrollPeriod.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Earnings).Sum(edr => edr.Amount.GetValueOrDefault());
                if (client.PagIbigDeductions == true) deductionBasis -= earningDeductionRecordsFromFifthPayrollPeriodOfPreviousMonth.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(edr => edr.Amount.GetValueOrDefault()) + earningDeductionRecordsForPreviousPayrollProcessBatches.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(edr => edr.Amount.GetValueOrDefault()) + employeeEdrsForPayrollPeriod.Where(edr => edr.EarningDeduction.EarningDeductionType == EarningDeductionType.Deductions).Sum(edr => edr.Amount.GetValueOrDefault());
                if (client.PagIbigUndertime == true) deductionBasis -= dailyTimeRecordsFromFifthPayrollPeriodOfPreviousMonth.Sum(dtr => dtr.TimeNotWorkedValue) + dailyTimeRecordsForPreviousPayrollProcessBatches.Sum(dtr => dtr.TimeNotWorkedValue) + employeeDtrsForPayrollPeriod.Sum(dtr => dtr.TimeNotWorkedValue);

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

            private decimal ComputePHICEmployee(decimal deductionBasis, PhicRecord phicSettings)
            {
                deductionBasis = Math.Max(deductionBasis, phicSettings.MinimumDeduction.Value);
                deductionBasis = Math.Min(deductionBasis, phicSettings.MaximumDeduction.Value);

                return phicSettings.PercentageForComputation * phicSettings.EmployeePercentageShareForComputation * deductionBasis;
            }
                
            private decimal ComputePHICEmployer(decimal deductionBasis, PhicRecord phicSettings)
            {
                deductionBasis = Math.Max(deductionBasis, phicSettings.MinimumDeduction.Value);
                deductionBasis = Math.Min(deductionBasis, phicSettings.MaximumDeduction.Value);

                return phicSettings.PercentageForComputation * phicSettings.EmployerPercentageShareForComputation * deductionBasis;
            }

            private decimal ComputePagIbigEmployee(decimal deductionBasis, PagIbigRecord pagIbigRecord)
            {
                if (pagIbigRecord == null) return 0;

                return deductionBasis < pagIbigRecord.MinimumDeduction.Value ?
                    deductionBasis * (decimal)pagIbigRecord.EmployeePercentage.Value / 100 :
                    pagIbigRecord.DeductionAmount.Value;
            }

            private decimal ComputePagIbigEmployer(decimal deductionBasis, PagIbigRecord pagIbigRecord)
            {
                if (pagIbigRecord == null) return 0;

                return deductionBasis < pagIbigRecord.MinimumDeduction.Value ?
                    deductionBasis * (decimal)pagIbigRecord.EmployerPercentage.Value / 100 :
                    pagIbigRecord.DeductionAmount.Value;
            }

            private decimal? ComputeTax(Employee employee, Client client, IEnumerable<DailyTimeRecord> employeeDtrs, IEnumerable<Overtime> employeeOts, IEnumerable<EarningDeductionRecord> employeeEds)
            {
                return null;
            }
        }
    }
}
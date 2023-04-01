using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Infrastructure.Logging;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.Payroll
{
    public class PayrollSummaryReport
    {
        public class Query : IRequest<QueryResult>
        {
            public int? PayrollProcessBatchId { get; set; }
            public string DisplayMode { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(q => q.PayrollProcessBatchId)
                    .NotEmpty();
            }
        }

        public class QueryResult
        {
            public int? PayrollProcessBatchId { get; set; }
            public string DisplayMode { get; set; }
            public Models.PayrollProcessBatch PayrollProcessBatchResult { get; set; }
            public IEnumerable<Models.PayPercentage> PayRates { get; set; } = new List<Models.PayPercentage>();
            public IEnumerable<Models.EarningDeduction> EarningDeductions { get; set; } = new List<Models.EarningDeduction>();
            public IEnumerable<Models.LoanType> LoanTypes { get; set; } = new List<Models.LoanType>();

            public IEnumerable<PayrollReportItem> PayrollReportItems { get; set; } = new List<PayrollReportItem>();

            public class PayrollReportItem
            {
                public Models.PayrollRecord PayrollRecord { get; set; }
                public Models.DailyTimeRecord DailyTimeRecord { get; set; }
                public IEnumerable<Models.Overtime> Overtimes { get; set; } = new List<Models.Overtime>();
                public IEnumerable<Models.EarningDeductionRecord> EarningDeductionRecords { get; set; } = new List<Models.EarningDeductionRecord>();
                public IEnumerable<Models.Loan> Loans { get; set; } = new List<Models.Loan>();
            }

            public decimal TotalBasicPay { get; set; }
            public decimal TotalCOLA { get; set; }
            public decimal TotalUndertime { get; set; }
            public IEnumerable<Models.Overtime> TotalOvertimesPerType { get; set; }
            public decimal TotalOvertimes => TotalOvertimesPerType.Sum(ot => ot.NumberOfHoursValue.GetValueOrDefault());
            public IEnumerable<Models.EarningDeductionRecord> TotalEarningsPerType { get; set; }
            public IEnumerable<Models.EarningDeductionRecord> TotalDeductionsPerType { get; set; }
            public decimal TotalSSSEmployer { get; set; }
            public decimal TotalPHICEmployer { get; set; }
            public decimal TotalPagIbigEmployer { get; set; }
            public decimal TotalTaxEmployer { get; set; }
            public decimal TotalEarnings => TotalEarningsPerType.Sum(e => e.Amount.GetValueOrDefault());
            public decimal TotalDeductions => TotalDeductionsPerType.Sum(d => d.Amount.GetValueOrDefault());
            public decimal TotalContributions => TotalSSSEmployee + TotalPHICEmployee + TotalPagIbigEmployee + TotalTaxEmployee;
            public decimal TotalNetPay { get; set; }
            public decimal TotalSSSEmployee { get; set; }
            public decimal TotalPHICEmployee { get; set; }
            public decimal TotalPagIbigEmployee { get; set; }
            public decimal TotalTaxEmployee { get; set; }
            public int TotalEmployees { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, QueryResult>
        {
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<QueryResult> Handle(Query query, CancellationToken cancellationToken)
            {
                var payrollProcessBatch = await _db.PayrollProcessBatches
                    .Include(ppb => ppb.Client)
                    .SingleAsync(ppb => ppb.Id == query.PayrollProcessBatchId);

                var payrollRecords = await _db.PayrollRecords
                    .Include(pr => pr.Employee)
                    .Include(pr => pr.Employee.Department)
                    .Where(pr => pr.PayrollProcessBatchId == query.PayrollProcessBatchId)
                    .OrderBy(pr => pr.Employee.LastName)
                    .ThenBy(pr => pr.Employee.FirstName)
                    .ToListAsync();

                var employeeIds = payrollRecords.Select(pr => pr.EmployeeId.Value).Distinct().ToList();

                var dailyTimeRecords = await _db
                    .DailyTimeRecords
                    .AsNoTracking()
                    .Where(dtr => !dtr.DeletedOn.HasValue && employeeIds.Contains(dtr.EmployeeId.Value) && dtr.PayrollPeriodFrom == payrollProcessBatch.PayrollPeriodFrom && dtr.PayrollPeriodTo == payrollProcessBatch.PayrollPeriodTo)
                    .ToListAsync();

                var earningDeductionRecords = await _db
                    .EarningDeductionRecords
                    .Include(edr => edr.EarningDeduction)
                    .AsNoTracking()
                    .Where(edr => !edr.DeletedOn.HasValue && employeeIds.Contains(edr.EmployeeId.Value) && edr.PayrollPeriodFrom == payrollProcessBatch.PayrollPeriodFrom && edr.PayrollPeriodTo == payrollProcessBatch.PayrollPeriodTo)
                    .ToListAsync();

                var overtimes = await _db
                    .Overtimes
                    .Include(ot => ot.PayPercentage)
                    .AsNoTracking()
                    .Where(ot => !ot.DeletedOn.HasValue && employeeIds.Contains(ot.EmployeeId.Value) && ot.PayrollPeriodFrom == payrollProcessBatch.PayrollPeriodFrom && ot.PayrollPeriodTo == payrollProcessBatch.PayrollPeriodTo)
                    .ToListAsync();

                var loans = await _db
                    .Loans
                    .Include(l => l.LoanType)
                    .AsNoTracking()
                    .Where(l => !l.DeletedOn.HasValue && employeeIds.Contains(l.EmployeeId.Value) && !l.ZeroedOutOn.HasValue && DbFunctions.TruncateTime(l.StartDeductionDate) <= DbFunctions.TruncateTime(payrollProcessBatch.PayrollPeriodTo))
                    .ToListAsync();

                var payrollReportItems = new List<QueryResult.PayrollReportItem>(payrollRecords.Count);

                foreach (var payrollRecord in payrollRecords)
                {
                    var payrollReportItem = new QueryResult.PayrollReportItem();
                    payrollReportItem.PayrollRecord = payrollRecord;

                    var matchingDailyTimeRecords = dailyTimeRecords.Where(dtr => dtr.EmployeeId == payrollRecord.EmployeeId).OrderByDescending(dtr => dtr.AddedOn).ToList();
                    if (matchingDailyTimeRecords.Count > 1)
                    {
                        var logEntry = new LogEntry
                        {
                            Action = "PayrollSummaryReport",
                            Controller = "Payroll",
                            LoggedOn = DateTime.UtcNow,
                            Level = LogLevel.Warn,
                            Message = $"Multiple daily time records for employee {payrollRecord.EmployeeId}, payroll record {payrollRecord.Id}, payroll process batch {query.PayrollProcessBatchId}"
                        };

                        Logger.Log(logEntry);
                    }

                    payrollReportItem.DailyTimeRecord = matchingDailyTimeRecords.FirstOrDefault();
                    payrollReportItem.EarningDeductionRecords = earningDeductionRecords.Where(edr => edr.EmployeeId == payrollRecord.EmployeeId).ToList();
                    payrollReportItem.Overtimes = overtimes.Where(ot => ot.EmployeeId == payrollRecord.EmployeeId).ToList();
                    payrollReportItem.Loans = loans.Where(l => l.EmployeeId == payrollRecord.EmployeeId).ToList();

                    payrollReportItems.Add(payrollReportItem);
                }

                var payRates = await _db.PayPercentages.AsNoTracking().ToListAsync();
                var earningDeductions = await _db.EarningDeductions.AsNoTracking().Where(ed => !ed.DeletedOn.HasValue).ToListAsync();
                var loanTypes = await _db.LoanTypes.AsNoTracking().Where(lt => !lt.DeletedOn.HasValue).ToListAsync();

                var totalBasicPay = payrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.DaysWorkedValue.GetValueOrDefault() + p.HoursWorkedValue.GetValueOrDefault());
                var totalCOLA = payrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.COLADailyValue.GetValueOrDefault() + p.COLAHourlyValue.GetValueOrDefault() + p.COLAMonthlyValue.GetValueOrDefault());
                var totalUndertime = payrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.HoursUndertimeValue.GetValueOrDefault() + p.HoursLateValue.GetValueOrDefault());

                var totalOvertimesPerType = payrollReportItems
                    .SelectMany(p => p.Overtimes)
                    .GroupBy(ot => ot.PayPercentageId)
                    .Select(g => new Models.Overtime
                    {
                        NumberOfHoursValue = g.Sum(item => item.NumberOfHoursValue.GetValueOrDefault()),
                        PayPercentage = g.First().PayPercentage
                    });

                var totalEarningsPerType = payrollReportItems
                    .SelectMany(p => p.EarningDeductionRecords)
                    .Where(edr => edr.EarningDeduction.EarningDeductionType == Models.EarningDeductionType.Earnings)
                    .GroupBy(edr => edr.EarningDeductionId)
                    .Select(g => new Models.EarningDeductionRecord
                    {
                        Amount = g.Sum(item => item.Amount.GetValueOrDefault()),
                        EarningDeduction = g.First().EarningDeduction
                    });

                var totalDeductionsPerType = payrollReportItems
                    .SelectMany(p => p.EarningDeductionRecords)
                    .Where(edr => edr.EarningDeduction.EarningDeductionType == Models.EarningDeductionType.Deductions)
                    .GroupBy(edr => edr.EarningDeductionId)
                    .Select(g => new Models.EarningDeductionRecord
                    {
                        Amount = g.Sum(item => item.Amount.GetValueOrDefault()),
                        EarningDeduction = g.First().EarningDeduction
                    });

                var totalNetPay = payrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.NetPayValue);

                var totalSSSEmployer = payrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.SSSValueEmployer.GetValueOrDefault());
                var totalPHICEmployer = payrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.PHICValueEmployer.GetValueOrDefault());
                var totalPagIbigEmployer = payrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.PagIbigValueEmployer.GetValueOrDefault());
                var totalTaxEmployer = payrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.TaxValueEmployer.GetValueOrDefault());

                var totalSSSEmployee = payrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.SSSValueEmployee.GetValueOrDefault());
                var totalPHICEmployee = payrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.PHICValueEmployee.GetValueOrDefault());
                var totalPagIbigEmployee = payrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.PagIbigValueEmployee.GetValueOrDefault());
                var totalTaxEmployee = payrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.TaxValueEmployee.GetValueOrDefault());

                var totalEmployees = employeeIds.Count;

                return new QueryResult
                {
                    PayrollProcessBatchId = query.PayrollProcessBatchId,
                    DisplayMode = query.DisplayMode,
                    PayrollProcessBatchResult = payrollProcessBatch,
                    PayrollReportItems = payrollReportItems,
                    PayRates = payRates,
                    EarningDeductions = earningDeductions,
                    LoanTypes = loanTypes,
                    TotalBasicPay = totalBasicPay,
                    TotalCOLA = totalCOLA,
                    TotalUndertime = totalUndertime,
                    TotalOvertimesPerType = totalOvertimesPerType,
                    TotalEarningsPerType = totalEarningsPerType,
                    TotalDeductionsPerType = totalDeductionsPerType,
                    TotalNetPay = totalNetPay,
                    TotalSSSEmployer = totalSSSEmployer,
                    TotalPHICEmployer = totalPHICEmployer,
                    TotalPagIbigEmployer = totalPagIbigEmployer,
                    TotalTaxEmployer = totalTaxEmployer,
                    TotalSSSEmployee = totalSSSEmployee,
                    TotalPHICEmployee = totalPHICEmployee,
                    TotalPagIbigEmployee = totalPagIbigEmployee,
                    TotalTaxEmployee = totalTaxEmployee,
                    TotalEmployees = totalEmployees
                };
            }
        }
    }
}
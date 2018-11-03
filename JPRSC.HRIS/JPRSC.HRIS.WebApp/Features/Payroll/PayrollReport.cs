using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Payroll
{
    public class PayrollReport
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

                var employeeIds = payrollRecords.Select(pr => pr.EmployeeId.Value).ToList();

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

                var systemSettings = await _db.SystemSettings.SingleAsync();

                var payrollReportItems = new List<QueryResult.PayrollReportItem>(payrollRecords.Count);

                foreach (var payrollRecord in payrollRecords)
                {
                    if (payrollRecord.AddedOn < new DateTime(2018, 10, 30))
                    {
                        payrollRecord.NetPayValue = NetPayHelper.GetNetPay(systemSettings, payrollRecord.BasicPayValue, payrollRecord.TotalEarningsValue, payrollRecord.TotalGovDeductionsValue, payrollRecord.DeductionsValue.GetValueOrDefault(), payrollRecord.LoanPaymentValue.GetValueOrDefault());
                    }

                    var payrollReportItem = new QueryResult.PayrollReportItem();
                    payrollReportItem.PayrollRecord = payrollRecord;
                    payrollReportItem.DailyTimeRecord = dailyTimeRecords.SingleOrDefault(dtr => dtr.EmployeeId == payrollRecord.EmployeeId);
                    payrollReportItem.EarningDeductionRecords = earningDeductionRecords.Where(edr => edr.EmployeeId == payrollRecord.EmployeeId).ToList();
                    payrollReportItem.Overtimes = overtimes.Where(ot => ot.EmployeeId == payrollRecord.EmployeeId).ToList();
                    payrollReportItem.Loans = loans.Where(l => l.EmployeeId == payrollRecord.EmployeeId).ToList();

                    payrollReportItems.Add(payrollReportItem);
                }

                var payRates = await _db.PayPercentages.AsNoTracking().ToListAsync();
                var earningDeductions = await _db.EarningDeductions.AsNoTracking().Where(ed => !ed.DeletedOn.HasValue).ToListAsync();
                var loanTypes = await _db.LoanTypes.AsNoTracking().Where(lt => !lt.DeletedOn.HasValue).ToListAsync();

                return new QueryResult
                {
                    PayrollProcessBatchId = query.PayrollProcessBatchId,
                    DisplayMode = query.DisplayMode,
                    PayrollProcessBatchResult = payrollProcessBatch,
                    PayrollReportItems = payrollReportItems,
                    PayRates = payRates,
                    EarningDeductions = earningDeductions,
                    LoanTypes = loanTypes
                };
            }
        }
    }
}
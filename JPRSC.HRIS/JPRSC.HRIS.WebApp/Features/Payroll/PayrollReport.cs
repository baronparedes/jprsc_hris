using AutoMapper;
using FluentValidation;
using Humanizer;
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

            public IEnumerable<PayrollReportItem> PayrollReportItems { get; set; } = new List<PayrollReportItem>();

            public class PayrollReportItem
            {
                public Models.PayrollRecord PayrollRecord { get; set; }
                public Models.DailyTimeRecord DailyTimeRecord { get; set; }
                public IEnumerable<Models.Overtime> Overtimes { get; set; } = new List<Models.Overtime>();
                public IEnumerable<Models.EarningDeductionRecord> EarningDeductionRecords { get; set; } = new List<Models.EarningDeductionRecord>();
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

                var payrollReportItems = new List<QueryResult.PayrollReportItem>(payrollRecords.Count);

                foreach (var payrollRecord in payrollRecords)
                {
                    var payrollReportItem = new QueryResult.PayrollReportItem();
                    payrollReportItem.PayrollRecord = payrollRecord;
                    payrollReportItem.DailyTimeRecord = dailyTimeRecords.SingleOrDefault(dtr => dtr.EmployeeId == payrollRecord.EmployeeId);
                    payrollReportItem.EarningDeductionRecords = earningDeductionRecords.Where(edr => edr.EmployeeId == payrollRecord.EmployeeId).ToList();
                    payrollReportItem.Overtimes = overtimes.Where(ot => ot.EmployeeId == payrollRecord.EmployeeId).ToList();

                    payrollReportItems.Add(payrollReportItem);
                }

                return new QueryResult
                {
                    PayrollProcessBatchId = query.PayrollProcessBatchId,
                    DisplayMode = query.DisplayMode,
                    PayrollProcessBatchResult = payrollProcessBatch,
                    PayrollReportItems = payrollReportItems
                };
            }
        }
    }
}
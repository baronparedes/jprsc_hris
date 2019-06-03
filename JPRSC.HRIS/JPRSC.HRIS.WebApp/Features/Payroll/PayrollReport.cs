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
                public IEnumerable<Models.LoanDeduction> Loans { get; set; } = new List<Models.LoanDeduction>();
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
                var payrollProcessBatch = await _db
                    .PayrollProcessBatches
                    .AsNoTracking()
                    .Include(ppb => ppb.Client)
                    .SingleAsync(ppb => ppb.Id == query.PayrollProcessBatchId);

                var payrollRecords = await _db
                    .PayrollRecords
                    .AsNoTracking()
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

                var test = await _db
                    .DailyTimeRecords
                    .AsNoTracking()
                    .Where(dtr => dtr.PayrollProcessBatchId.HasValue && dtr.PayrollProcessBatchId == payrollProcessBatch.Id)
                    .ToListAsync();

                var earningDeductionRecords = await _db
                    .EarningDeductionRecords
                    .AsNoTracking()
                    .Include(edr => edr.EarningDeduction)
                    .Where(edr => !edr.DeletedOn.HasValue && employeeIds.Contains(edr.EmployeeId.Value) && edr.PayrollPeriodFrom == payrollProcessBatch.PayrollPeriodFrom && edr.PayrollPeriodTo == payrollProcessBatch.PayrollPeriodTo)
                    .ToListAsync();

                var overtimes = await _db
                    .Overtimes
                    .AsNoTracking()
                    .Include(ot => ot.PayPercentage)
                    .Where(ot => !ot.DeletedOn.HasValue && employeeIds.Contains(ot.EmployeeId.Value) && ot.PayrollPeriodFrom == payrollProcessBatch.PayrollPeriodFrom && ot.PayrollPeriodTo == payrollProcessBatch.PayrollPeriodTo)
                    .ToListAsync();

                var payrollRecordIds = payrollRecords.Select(pr => pr.Id).ToList();

                var loanDeductions = await _db
                    .LoanDeductions
                    .AsNoTracking()
                    .Include(ld => ld.Loan)
                    .Include(ld => ld.Loan.LoanType)
                    .Where(ld => ld.PayrollRecordId.HasValue && payrollRecordIds.Contains(ld.PayrollRecordId.Value))
                    .ToListAsync();

                var loans = await _db
                    .Loans
                    .Include(l => l.LoanType)
                    .AsNoTracking()
                    .Where(l => !l.DeletedOn.HasValue && employeeIds.Contains(l.EmployeeId.Value) && DbFunctions.TruncateTime(l.StartDeductionDate) <= DbFunctions.TruncateTime(payrollProcessBatch.PayrollPeriodTo))
                    .ToListAsync();

                var payrollReportItems = new List<QueryResult.PayrollReportItem>(payrollRecords.Count);
                var marchFour2019 = new DateTime(2019, 3, 4);

                foreach (var payrollRecord in payrollRecords)
                {
                    try
                    {
                        var payrollReportItem = new QueryResult.PayrollReportItem();
                        payrollReportItem.PayrollRecord = payrollRecord;
                        payrollReportItem.DailyTimeRecord = dailyTimeRecords.SingleOrDefault(dtr => dtr.EmployeeId == payrollRecord.EmployeeId);
                        payrollReportItem.EarningDeductionRecords = earningDeductionRecords.Where(edr => edr.EmployeeId == payrollRecord.EmployeeId).ToList();
                        payrollReportItem.Overtimes = overtimes.Where(ot => ot.EmployeeId == payrollRecord.EmployeeId).ToList();

                        if (payrollRecord.AddedOn < marchFour2019)
                        {
                            payrollReportItem.Loans = loans
                                .Where(l => l.EmployeeId == payrollRecord.EmployeeId)
                                .Select(l => new Models.LoanDeduction
                                {
                                    DeductionAmount = l.DeductionAmount,
                                    Loan = l,
                                    LoanId = l.Id,
                                    PayrollRecord = payrollRecord,
                                    PayrollRecordId = payrollRecord.Id
                                })
                                .ToList();
                        }
                        else
                        {
                            payrollReportItem.Loans = loanDeductions.Where(ld => ld.PayrollRecordId == payrollRecord.Id).ToList();
                        }

                        payrollReportItems.Add(payrollReportItem);
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
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
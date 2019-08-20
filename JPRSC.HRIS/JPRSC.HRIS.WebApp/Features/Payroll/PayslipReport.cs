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

namespace JPRSC.HRIS.WebApp.Features.Payroll
{
    public class PayslipReport
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
            public List<PayslipRecord> PayslipRecords { get; set; } = new List<PayslipRecord>();
            public Models.PayrollProcessBatch PayrollProcessBatchResult { get; set; }
            public IEnumerable<Models.PayPercentage> PayRates { get; set; } = new List<Models.PayPercentage>();
            public IEnumerable<Models.EarningDeduction> EarningDeductions { get; set; } = new List<Models.EarningDeduction>();
            public IEnumerable<Models.LoanType> LoanTypes { get; set; } = new List<Models.LoanType>();

            public class PayslipRecord
            {
                public Models.PayrollRecord PayrollRecord { get; set; }
                public Models.DailyTimeRecord DailyTimeRecord { get; set; }
                public IEnumerable<Models.Overtime> Overtimes { get; set; } = new List<Models.Overtime>();
                public IEnumerable<Models.EarningDeductionRecord> EarningDeductionRecords { get; set; } = new List<Models.EarningDeductionRecord>();
                public IEnumerable<Models.Loan> Loans { get; set; } = new List<Models.Loan>();
                public IEnumerable<Models.LoanDeduction> LoanDeductions { get; set; }
                public Models.PayrollProcessBatch PayrollProcessBatchResult { get; set; }
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

                var payrollProcessBatchResult = payrollProcessBatch;

                var payrollRecords = await _db.PayrollRecords
                    .Include(pr => pr.Employee)
                    .Include(pr => pr.Employee.Department)
                    .Include(pr => pr.LoanDeductions)
                    .Include(pr => pr.LoanDeductions.Select(ld => ld.Loan.LoanType))
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

                var payrollRecordIds = payrollRecords.Select(pr => pr.Id).ToList();

                //var loanDeductions = await _db
                //    .LoanDeductions
                //    .Include(ld => ld.Loan)
                //    .AsNoTracking()
                //    .Where(ld => payrollRecordIds.Contains(ld.)

                var payslipRecords = new List<QueryResult.PayslipRecord>(payrollRecords.Count);

                foreach (var payrollRecord in payrollRecords)
                {
                    var payslipRecord = new QueryResult.PayslipRecord();
                    payslipRecord.PayrollProcessBatchResult = payrollProcessBatchResult;
                    payslipRecord.PayrollRecord = payrollRecord;

                    var matchingDailyTimeRecords = dailyTimeRecords.Where(dtr => dtr.EmployeeId == payrollRecord.EmployeeId).OrderByDescending(dtr => dtr.AddedOn).ToList();
                    if (matchingDailyTimeRecords.Count > 1)
                    {
                        var logEntry = new LogEntry
                        {
                            Action = "PayslipReport",
                            Controller = "Payroll",
                            LoggedOn = DateTime.UtcNow,
                            Level = LogLevel.Warn,
                            Message = $"Multiple daily time records for employee {payrollRecord.EmployeeId}, payroll record {payrollRecord.Id}, payroll process batch {query.PayrollProcessBatchId}"
                        };

                        Logger.Log(logEntry);
                    }

                    payslipRecord.DailyTimeRecord = matchingDailyTimeRecords.FirstOrDefault();
                    payslipRecord.EarningDeductionRecords = earningDeductionRecords.Where(edr => edr.EmployeeId == payrollRecord.EmployeeId).ToList();
                    payslipRecord.Overtimes = overtimes.Where(ot => ot.EmployeeId == payrollRecord.EmployeeId).ToList();
                    payslipRecord.Loans = loans.Where(l => l.EmployeeId == payrollRecord.EmployeeId).ToList();
                    payslipRecord.LoanDeductions = payrollRecord.LoanDeductions;

                    payslipRecords.Add(payslipRecord);
                }

                var payRates = await _db.PayPercentages.AsNoTracking().ToListAsync();
                var earningDeductions = await _db.EarningDeductions.AsNoTracking().Where(ed => !ed.DeletedOn.HasValue).ToListAsync();
                var loanTypes = await _db.LoanTypes.AsNoTracking().Where(lt => !lt.DeletedOn.HasValue).ToListAsync();

                return new QueryResult
                {
                    PayrollProcessBatchId = query.PayrollProcessBatchId,
                    DisplayMode = query.DisplayMode,
                    PayrollProcessBatchResult = payrollProcessBatchResult,
                    PayslipRecords = payslipRecords,
                    PayRates = payRates,
                    EarningDeductions = earningDeductions,
                    LoanTypes = loanTypes
                };
            }
        }
    }
}
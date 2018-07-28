using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace JPRSC.HRIS.WebApp.Features.Payroll
{
    public class Search
    {
        public class Query : IRequest<QueryResult>
        {
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
            }
        }

        public class QueryResult
        {
            public IEnumerable<PayrollProcessBatch> PayrollProcessBatches { get; set; } = new List<PayrollProcessBatch>();

            public class PayrollProcessBatch
            {
                public int Id { get; set; }
                public Client Client { get; set; }
                public int? ClientId { get; set; }
                public DateTime? EndProcessedOn { get; set; }
                public int? PayrollPeriod { get; set; }
                public DateTime? PayrollPeriodFrom { get; set; }
                public DateTime? PayrollPeriodTo { get; set; }

                public bool IsEndProcessed => EndProcessedOn.HasValue;
            }

            public class Client
            {
                public string Code { get; set; }
                public int? CurrentPayrollPeriod { get; set; }
                public string Description { get; set; }
                public int Id { get; set; }
                public int? NumberOfPayrollPeriodsAMonth { get; set; }
                public int? NumberOfWorkingDaysForThisPayrollPeriod { get; set; }
                public DateTime? PayrollPeriodFrom { get; set; }
                public DateTime? PayrollPeriodTo { get; set; }
                public string Name { get; set; }

                public int? NextPayrollPeriod => CurrentPayrollPeriod == NumberOfPayrollPeriodsAMonth ? 1 : CurrentPayrollPeriod += 1;
                public DateTime? NextPayrollPeriodFrom => PayrollPeriodFrom.Value.AddDays(NumberOfWorkingDaysForThisPayrollPeriod.Value);
                public DateTime? NextPayrollPeriodTo => PayrollPeriodTo.Value.AddDays(NumberOfWorkingDaysForThisPayrollPeriod.Value);
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
                var payrollProcessBatches = await _db
                    .PayrollProcessBatches
                    .Where(ppb => !ppb.DeletedOn.HasValue && !ppb.DateOverwritten.HasValue)
                    .OrderByDescending(ppb => ppb.AddedOn)
                    .ProjectToListAsync<QueryResult.PayrollProcessBatch>();

                return new QueryResult
                {
                    PayrollProcessBatches = payrollProcessBatches
                };
            }
        }
    }
}
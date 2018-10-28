using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
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
            public int? ClientId { get; set; }
            public Month? PayrollPeriodMonth { get; set; }
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
                public DateTime AddedOn { get; set; }
                public DateTime? DeletedOn { get; set; }
                public int Id { get; set; }
                public DateTime? ModifiedOn { get; set; }
                public Client Client { get; set; }
                public int? ClientId { get; set; }
                public DateTime? EndProcessedOn { get; set; }
                public int? PayrollPeriod { get; set; }
                public DateTime? PayrollPeriodFrom { get; set; }
                public Month? PayrollPeriodMonth { get; set; }
                public DateTime? PayrollPeriodTo { get; set; }

                public bool IsEndProcessed => EndProcessedOn.HasValue;
                public string MonthText => PayrollPeriodMonth.HasValue ? PayrollPeriodMonth.Value.ToString() : null;
            }

            public class Client
            {
                public string Code { get; set; }
                public string Description { get; set; }
                public int Id { get; set; }
                public DateTime? ModifiedOn { get; set; }
                public string Name { get; set; }
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
                var dbQuery = _db
                    .PayrollProcessBatches
                    .Where(ppb => !ppb.DeletedOn.HasValue && !ppb.DateOverwritten.HasValue);

                if (query.ClientId.HasValue && query.ClientId.Value > 0)
                {
                    dbQuery = dbQuery
                        .Where(ppb => ppb.ClientId == query.ClientId);
                }

                if (query.PayrollPeriodMonth.HasValue && query.PayrollPeriodMonth.Value > 0)
                {
                    dbQuery = dbQuery
                        .Where(ppb => ppb.PayrollPeriodMonth == query.PayrollPeriodMonth);
                }

                var payrollProcessBatches = await dbQuery
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
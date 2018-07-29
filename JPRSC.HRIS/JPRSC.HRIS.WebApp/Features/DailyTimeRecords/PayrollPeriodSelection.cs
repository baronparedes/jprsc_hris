using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.DailyTimeRecords
{
    public class PayrollPeriodSelection
    {
        public class Query : IRequest<QueryResult>
        {
            public int? ClientId { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(c => c.ClientId)
                    .NotEmpty();
            }
        }

        public class QueryResult
        {
            public IList<SelectListItem> PayrollPeriods { get; set; } = new List<SelectListItem>();
        }

        public class QueryHandler : IRequestHandler<Query, QueryResult>
        {
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<QueryResult> Handle(Query query, CancellationToken token)
            {
                var clientPayrollProcessBatches = await _db
                    .PayrollProcessBatches
                    .Where(ppb => ppb.ClientId == query.ClientId && !ppb.DeletedOn.HasValue && !ppb.DateOverwritten.HasValue && !ppb.EndProcessedOn.HasValue)
                    .OrderBy(ppb => ppb.PayrollPeriodFrom)
                    .ThenBy(ppb => ppb.PayrollPeriodTo)
                    .ToListAsync();

                return new QueryResult
                {
                    PayrollPeriods = GetPayrollPeriods(clientPayrollProcessBatches)
                };
            }

            private IList<SelectListItem> GetPayrollPeriods(IList<PayrollProcessBatch> payrollProcessBatches) =>
                payrollProcessBatches
                .Select(ppb => new SelectListItem
                {
                    Value = ppb.Id.ToString(),
                    Text = $"{ppb.PayrollPeriodFrom.Value:MMM d, yyyy} - {ppb.PayrollPeriodTo.Value:MMM d, yyyy}"
                })
                .ToList();
        }
    }
}
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
                var dailyTimeRecords = await _db
                    .DailyTimeRecords
                    .Include(dtr => dtr.Employee)
                    .Where(dtr => !dtr.DeletedOn.HasValue && dtr.Employee.ClientId == query.ClientId)
                    .ToListAsync();

                return new QueryResult
                {
                    PayrollPeriods = GetPayrollPeriods(dailyTimeRecords)
                };
            }

            private IList<SelectListItem> GetPayrollPeriods(IList<DailyTimeRecord> dailyTimeRecords)
            {
                var payrollPeriods = new List<Tuple<int, DateTime?, DateTime?>>();

                foreach (var dtr in dailyTimeRecords)
                {
                    if (!payrollPeriods.Any(pp => pp.Item2 == dtr.PayrollPeriodFrom && pp.Item3 == dtr.PayrollPeriodTo))
                    {
                        payrollPeriods.Add(Tuple.Create(dtr.Id, dtr.PayrollPeriodFrom, dtr.PayrollPeriodTo));
                    }
                }

                return payrollPeriods
                    .OrderBy(pp => pp.Item2)
                    .Select(pp => new SelectListItem
                    {
                        Value = pp.Item1.ToString(),
                        Text = $"{pp.Item2.Value:MMM d, yyyy} - {pp.Item3.Value:MMM d, yyyy}"
                    })
                    .ToList();
            }
        }
    }
}
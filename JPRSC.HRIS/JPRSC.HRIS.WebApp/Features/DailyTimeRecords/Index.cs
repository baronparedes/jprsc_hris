using AutoMapper;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.DailyTimeRecords
{
    public class Index
    {
        public class Query : IRequest<QueryResult>
        {
        }

        public class QueryResult
        {
            public IList<Client> Clients { get; set; } = new List<Client>();
            public IList<EarningDeduction> EarningDeductions { get; set; } = new List<EarningDeduction>();
            public IList<PayPercentage> PayRates { get; set; } = new List<PayPercentage>();

            public class Client
            {
                public string Code { get; set; }
                public int? CurrentPayrollPeriod { get; set; }
                public CutOffPeriod? CutOffPeriod { get; set; }
                public int? DaysPerWeek { get; set; }
                public string Description { get; set; }
                public int? HoursPerDay { get; set; }
                public int Id { get; set; }
                public string Name { get; set; }
                public int? NumberOfHoursInADay { get; set; }
                public int? NumberOfPayrollPeriodsAMonth { get; set; }
                public int? NumberOfWorkingDaysForThisPayrollPeriod { get; set; }
                public PayrollCode? PayrollCode { get; set; }
                public DateTime? PayrollPeriodFrom { get; set; }
                public Month? PayrollPeriodMonth { get; set; }
                public DateTime? PayrollPeriodTo { get; set; }
                public TaxTable? TaxTable { get; set; }
                public bool? ZeroBasic { get; set; }
            }

            public class EarningDeduction
            {
                public string Code { get; set; }
                public string Description { get; set; }
                public EarningDeductionType? EarningDeductionType { get; set; }
                public int Id { get; set; }
            }

            public class PayPercentage
            {
                public int Id { get; set; }
                public string Name { get; set; }
                public double? Percentage { get; set; }
            }
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
                var clients = await _db.Clients
                    .Where(c => !c.DeletedOn.HasValue)
                    .ProjectToListAsync<QueryResult.Client>();

                var earningDeductions = await _db.EarningDeductions
                    .Where(ed => !ed.DeletedOn.HasValue)
                    .ProjectToListAsync<QueryResult.EarningDeduction>();

                var payRates = await _db.PayPercentages
                    .ProjectToListAsync<QueryResult.PayPercentage>();

                return new QueryResult
                {
                    Clients = clients,
                    EarningDeductions = earningDeductions,
                    PayRates = payRates
                };
            }
        }
    }
}
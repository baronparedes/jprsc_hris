using AutoMapper;
using AutoMapper.QueryableExtensions;
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

namespace JPRSC.HRIS.Features.DailyTimeRecords
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

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Client, QueryResult.Client>();
                CreateMap<EarningDeduction, QueryResult.EarningDeduction>();
                CreateMap<PayPercentage, QueryResult.PayPercentage>();
            }
        }

        public class QueryHandler : IRequestHandler<Query, QueryResult>
        {
            private readonly ApplicationDbContext _db;
            private readonly IMapper _mapper;

            public QueryHandler(ApplicationDbContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<QueryResult> Handle(Query query, CancellationToken token)
            {
                var clients = await _db.Clients
                    .AsNoTracking()
                    .Where(c => !c.DeletedOn.HasValue)
                    .OrderBy(c => c.Code)
                    .ProjectTo<QueryResult.Client>(_mapper)
                    .ToListAsync();

                var earningDeductions = await _db.EarningDeductions
                    .AsNoTracking()
                    .Where(ed => !ed.DeletedOn.HasValue)
                    .ProjectTo<QueryResult.EarningDeduction>(_mapper)
                    .ToListAsync();

                var payRates = await _db.PayPercentages
                    .AsNoTracking()
                    .ProjectTo<QueryResult.PayPercentage>(_mapper)
                    .ToListAsync();

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
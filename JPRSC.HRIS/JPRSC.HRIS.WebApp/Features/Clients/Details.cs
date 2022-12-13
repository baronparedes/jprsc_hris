using AutoMapper;
using AutoMapper.QueryableExtensions;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Clients
{
    public class Details
    {
        public class Query : IRequest<QueryResult>
        {
            public int ClientId { get; set; }
        }

        public class QueryResult
        {
            public string Code { get; set; }
            public int? CurrentPayrollPeriod { get; set; }
            public int? DaysPerWeek { get; set; }
            public string Description { get; set; }
            public int? HoursPerDay { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
            public int? NumberOfPayrollPeriodsAMonth { get; set; }
            public int? NumberOfWorkingDaysForThisPayrollPeriod { get; set; }
            public PayrollCode? PayrollCode { get; set; }
            public DateTime? PayrollPeriodFrom { get; set; }
            public Month? PayrollPeriodMonth { get; set; }
            public DateTime? PayrollPeriodTo { get; set; }
            public TaxTable? TaxTable { get; set; }
            public bool? ZeroBasic { get; set; }

            public string SSSPayrollPeriod { get; set; }
            public bool? SSSBasic { get; set; }
            public bool? SSSOvertime { get; set; }
            public bool? SSSCola { get; set; }
            public bool? SSSEarnings { get; set; }
            public bool? SSSDeductions { get; set; }
            public bool? SSSUndertime { get; set; }

            public string PHICPayrollPeriod { get; set; }
            public bool? PHICBasic { get; set; }
            public bool? PHICOvertime { get; set; }
            public bool? PHICCola { get; set; }
            public bool? PHICEarnings { get; set; }
            public bool? PHICDeductions { get; set; }
            public bool? PHICUndertime { get; set; }

            public string PagIbigPayrollPeriod { get; set; }
            public bool? PagIbigBasic { get; set; }
            public bool? PagIbigOvertime { get; set; }
            public bool? PagIbigCola { get; set; }
            public bool? PagIbigEarnings { get; set; }
            public bool? PagIbigDeductions { get; set; }
            public bool? PagIbigUndertime { get; set; }

            public string TaxPayrollPeriod { get; set; }
            public bool? TaxBasic { get; set; }
            public bool? TaxOvertime { get; set; }
            public bool? TaxCola { get; set; }
            public bool? TaxEarnings { get; set; }
            public bool? TaxDeductions { get; set; }
            public bool? TaxUndertime { get; set; }

            public bool? LoanExempt { get; set; }
            public bool? PagIbigExempt { get; set; }
            public bool? PHICExempt { get; set; }
            public bool? SSSExempt { get; set; }
            public bool? TaxExempt { get; set; }
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
                return await _db.Clients.AsNoTracking().Where(c => c.Id == query.ClientId && !c.DeletedOn.HasValue).ProjectTo<QueryResult>(_mapper).SingleAsync();
            }
        }
    }
}
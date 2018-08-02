using AutoMapper;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Infrastructure.NET;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.EarningDeductions
{
    public class Search
    {
        public class Query : IRequest<QueryResult>
        {
            public string SearchTerm { get; set; }

            public string SearchLikeTerm
            {
                get
                {
                    if (String.IsNullOrWhiteSpace(SearchTerm)) return null;

                    return $"%{SearchTerm}%";
                }
            }
        }

        public class QueryResult
        {
            public IEnumerable<EarningDeduction> EarningDeductions { get; set; } = new List<EarningDeduction>();

            public class EarningDeduction
            {
                public string Code { get; set; }
                public string Description { get; set; }
                public EarningDeductionType? EarningDeductionType { get; set; }
                public string EarningDeductionTypeFormatted { get { return EarningDeductionType.HasValue ? EnumHelper.GetDisplayName(EarningDeductionType.Value) : null; } }
                public int Id { get; set; }
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
                var dbQuery = _db
                    .EarningDeductions
                    .Where(ed => !ed.DeletedOn.HasValue);

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {
                    dbQuery = dbQuery
                        .Where(ed => DbFunctions.Like(ed.Code, query.SearchLikeTerm) ||
                            DbFunctions.Like(ed.Description, query.SearchLikeTerm));
                }

                var earningDeductions = await dbQuery
                    .OrderBy(ed => ed.Id)
                    
                    .ProjectToListAsync<QueryResult.EarningDeduction>();

                return new QueryResult
                {
                    EarningDeductions = earningDeductions
                };
            }
        }
    }
}
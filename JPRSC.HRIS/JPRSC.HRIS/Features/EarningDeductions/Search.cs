using AutoMapper;
using AutoMapper.QueryableExtensions;
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

namespace JPRSC.HRIS.Features.EarningDeductions
{
    public class Search
    {
        public class Query : IRequest<QueryResult>
        {
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
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

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<EarningDeduction, QueryResult.EarningDeduction>();
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
                var pageNumber = query.PageNumber.HasValue && query.PageNumber > 0 ? query.PageNumber.Value : 1;
                var pageSize = query.PageSize.HasValue && query.PageSize > 0 ? Math.Min(query.PageSize.Value, 1000) : AppSettings.Int("DefaultGridPageSize");

                var dbQuery = _db
                    .EarningDeductions
                    .AsNoTracking()
                    .Where(ed => !ed.DeletedOn.HasValue);

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {
                    dbQuery = dbQuery
                        .Where(ed => DbFunctions.Like(ed.Code, query.SearchLikeTerm) ||
                            DbFunctions.Like(ed.Description, query.SearchLikeTerm));
                }

                var earningDeductions = await dbQuery
                    .OrderBy(ed => ed.Id)
                    .PageBy(pageNumber, pageSize)
                    .ProjectTo<QueryResult.EarningDeduction>(_mapper)
                    .ToListAsync();

                return new QueryResult
                {
                    EarningDeductions = earningDeductions
                };
            }
        }
    }
}
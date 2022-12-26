using AutoMapper;
using AutoMapper.QueryableExtensions;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.PayPercentages
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
            public IEnumerable<PayPercentage> PayPercentages { get; set; } = new List<PayPercentage>();

            public class PayPercentage
            {
                public string Code { get; set; }
                public int Id { get; set; }
                public bool? IncludeCOLA { get; set; }
                public string Name { get; set; }
                public double? Percentage { get; set; }
            }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
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
                var pageNumber = query.PageNumber.HasValue && query.PageNumber > 0 ? query.PageNumber.Value : 1;
                var pageSize = query.PageSize.HasValue && query.PageSize > 0 ? Math.Min(query.PageSize.Value, 1000) : AppSettings.Int("DefaultGridPageSize");

                var dbQuery = _db
                    .PayPercentages
                    .AsNoTracking()
                    .AsQueryable();

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {
                    dbQuery = dbQuery
                        .Where(pp => DbFunctions.Like(pp.Name, query.SearchLikeTerm));
                }

                var payPercentages = await dbQuery
                    .OrderBy(pp => pp.Id)
                    .PageBy(pageNumber, pageSize)
                    .ProjectTo<QueryResult.PayPercentage>(_mapper)
                    .ToListAsync();

                return new QueryResult
                {
                    PayPercentages = payPercentages
                };
            }
        }
    }
}
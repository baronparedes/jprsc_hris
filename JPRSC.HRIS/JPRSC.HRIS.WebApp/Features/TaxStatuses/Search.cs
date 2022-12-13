using AutoMapper;
using AutoMapper.QueryableExtensions;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.TaxStatuses
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
            public IEnumerable<TaxStatus> TaxStatuses { get; set; } = new List<TaxStatus>();

            public class TaxStatus
            {
                public string Code { get; set; }
                public double? Exemption { get; set; }
                public string Name { get; set; }
                public int Id { get; set; }
                public IList<TaxRange> TaxRanges { get; set; } = new List<TaxRange>();
            }

            public class TaxRange
            {
                [JsonIgnore]
                public DateTime? DeletedOn { get; set; }
                public decimal? From { get; set; }
                public int Id { get; set; }
                public double? Percentage { get; set; }
                public decimal? Plus { get; set; }
                public decimal? To { get; set; }
            }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<TaxStatus, QueryResult.TaxStatus>();
                CreateMap<TaxRange, QueryResult.TaxRange>();
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
                    .TaxStatuses
                    .AsNoTracking()
                    .Include(tr => tr.TaxRanges)
                    .Where(ts => !ts.DeletedOn.HasValue);

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {
                    dbQuery = dbQuery
                        .Where(ts => DbFunctions.Like(ts.Name, query.SearchLikeTerm) ||
                            DbFunctions.Like(ts.Code, query.SearchLikeTerm));
                }

                var taxStatuses = await dbQuery
                    .OrderBy(ts => ts.Id)
                    .PageBy(pageNumber, pageSize)
                    .ProjectTo<QueryResult.TaxStatus>(_mapper)
                    .ToListAsync();

                foreach (var taxStatus in taxStatuses)
                {
                    taxStatus.TaxRanges = taxStatus.TaxRanges.Where(tr => !tr.DeletedOn.HasValue).ToList();
                }

                return new QueryResult
                {
                    TaxStatuses = taxStatuses
                };
            }
        }
    }
}
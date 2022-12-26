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

namespace JPRSC.HRIS.Features.AuditTrails
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
            public IEnumerable<AuditTrail> AuditTrails { get; set; } = new List<AuditTrail>();
            public int TotalResultsCount { get; set; }

            public class AuditTrail
            {
                public DateTime? AddedOn { get; set; }
                public int Id { get; set; }
                public int? RecordId { get; set; }
                public string Action { get; set; }
                public string Module { get; set; }
                public string UserName { get; set; }
            }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<AuditTrailEntry, QueryResult.AuditTrail>();
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

                var totalResultsCount = _db
                    .AuditTrailEntries
                    .AsNoTracking()
                    .Count();

                var dbQuery = _db
                    .AuditTrailEntries
                    .AsNoTracking();

                var auditTrails = await dbQuery
                    .OrderByDescending(at => at.AddedOn)
                    .PageBy(pageNumber, pageSize)
                    .ProjectTo<QueryResult.AuditTrail>(_mapper)
                    .ToListAsync();

                return new QueryResult
                {
                    AuditTrails = auditTrails,
                    TotalResultsCount = totalResultsCount
                };
            }
        }
    }
}
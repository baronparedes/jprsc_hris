using AutoMapper;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.ApprovalLevels
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
            public IEnumerable<ApprovalLevel> ApprovalLevels { get; set; } = new List<ApprovalLevel>();

            public class ApprovalLevel
            {
                public int Id { get; set; }
                public int? Level { get; set; }
                public string UserName { get; set; }
            }
        }

        public class QueryHandler : AsyncRequestHandler<Query, QueryResult>
        {
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            protected override async Task<QueryResult> HandleCore(Query query)
            {
                var dbQuery = _db
                    .ApprovalLevels
                    .Where(r => !r.DeletedOn.HasValue);

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {
                    
                }

                var approvalLevels = await dbQuery
                    .OrderBy(r => r.Level)
                    .Take(AppSettings.Int("DefaultGridPageSize"))
                    .ProjectToListAsync<QueryResult.ApprovalLevel>();

                return new QueryResult
                {
                    ApprovalLevels = approvalLevels
                };
            }
        }
    }
}
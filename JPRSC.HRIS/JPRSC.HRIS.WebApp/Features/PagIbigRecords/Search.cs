using AutoMapper;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.PagIbigRecords
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
            public IEnumerable<PagIbigRecord> PagIbigRecords { get; set; } = new List<PagIbigRecord>();

            public class PagIbigRecord
            {
                public string Code { get; set; }
                public string Description { get; set; }
                public int Id { get; set; }
                public string Name { get; set; }
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
                    .PagIbigRecords
                    .Where(pir => !pir.DeletedOn.HasValue);

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {
                    dbQuery = dbQuery
                        .Where(pir => DbFunctions.Like(pir.Code, query.SearchLikeTerm) ||
                            DbFunctions.Like(pir.Description, query.SearchLikeTerm) ||
                            DbFunctions.Like(pir.Name, query.SearchLikeTerm));
                }

                var pagIbigRecords = await dbQuery
                    .OrderBy(pir => pir.Id)
                    
                    .ProjectToListAsync<QueryResult.PagIbigRecord>();

                return new QueryResult
                {
                    PagIbigRecords = pagIbigRecords
                };
            }
        }
    }
}
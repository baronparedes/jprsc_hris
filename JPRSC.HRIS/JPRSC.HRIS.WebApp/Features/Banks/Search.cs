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

namespace JPRSC.HRIS.WebApp.Features.Banks
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
            public IEnumerable<Bank> Banks { get; set; } = new List<Bank>();

            public class Bank
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
                    .Banks
                    .Where(b => !b.DeletedOn.HasValue);

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {
                    dbQuery = dbQuery
                        .Where(b => DbFunctions.Like(b.Code, query.SearchLikeTerm) ||
                            DbFunctions.Like(b.Description, query.SearchLikeTerm) ||
                            DbFunctions.Like(b.Name, query.SearchLikeTerm));
                }

                var banks = await dbQuery
                    .OrderBy(b => b.Id)
                    
                    .ProjectToListAsync<QueryResult.Bank>();

                return new QueryResult
                {
                    Banks = banks
                };
            }
        }
    }
}
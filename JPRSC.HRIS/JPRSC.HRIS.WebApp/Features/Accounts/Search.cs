using AutoMapper;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Accounts
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
            public IEnumerable<Account> Accounts { get; set; } = new List<Account>();

            public class Account
            {
                public string Name { get; set; }
                public string JobTitle { get; set; }
                public string UserName { get; set; }
                public string Id { get; set; }
            }
        }

        public class QueryHandler : IAsyncRequestHandler<Query, QueryResult>
        {
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<QueryResult> Handle(Query query)
            {
                var dbQuery = _db
                    .Users
                    .Where(u => !u.DeletedOn.HasValue);

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {
                    dbQuery = dbQuery
                        .Where(u => DbFunctions.Like(u.Name, query.SearchLikeTerm) ||
                            DbFunctions.Like(u.JobTitle, query.SearchLikeTerm) ||
                            DbFunctions.Like(u.UserName, query.SearchLikeTerm));
                }

                var users = await dbQuery
                    .OrderBy(u => u.Name)
                    .Take(AppSettings.Int("DefaultGridPageSize"))
                    .ProjectToListAsync<QueryResult.Account>();

                return new QueryResult
                {
                    Accounts = users
                };
            }
        }
    }
}
using AutoMapper;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.CustomRoles
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
            public IEnumerable<CustomRole> CustomRoles { get; set; } = new List<CustomRole>();

            public class CustomRole
            {
                public int Id { get; set; }
                public string Name { get; set; }
                //public IEnumerable<Permission> Permissions { get; set; }
            }
        }

        public class QueryHandler : IRequestHandler<Query, QueryResult>
        {
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<QueryResult> Handle(Query query, System.Threading.CancellationToken token)
            {
                var dbQuery = _db
                    .CustomRoles
                    .Where(cr => !cr.DeletedOn.HasValue);

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {
                    dbQuery = dbQuery
                        .Where(cr => DbFunctions.Like(cr.Name, query.SearchLikeTerm));
                }

                var customRoles = await dbQuery
                    .OrderBy(cr => cr.Id)
                    .Take(AppSettings.Int("DefaultGridPageSize"))
                    .ProjectToListAsync<QueryResult.CustomRole>();

                return new QueryResult
                {
                    CustomRoles = customRoles
                };
            }
        }
    }
}
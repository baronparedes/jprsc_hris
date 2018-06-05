using AutoMapper;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Companies
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
            public IEnumerable<Company> Companies { get; set; } = new List<Company>();

            public class Company
            {
                public string Address { get; set; }
                public string Email { get; set; }
                public int Id { get; set; }
                public string Name { get; set; }
                public string Phone { get; set; }
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
                    .Companies
                    .Where(cp => !cp.DeletedOn.HasValue);

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {
                    dbQuery = dbQuery
                        .Where(cp => DbFunctions.Like(cp.Name, query.SearchLikeTerm) ||
                            DbFunctions.Like(cp.Address, query.SearchLikeTerm) ||
                            DbFunctions.Like(cp.Email, query.SearchLikeTerm) ||
                            DbFunctions.Like(cp.Phone, query.SearchLikeTerm));
                }

                var companies = await dbQuery
                    .OrderBy(cp => cp.Id)
                    .Take(AppSettings.Int("DefaultGridPageSize"))
                    .ProjectToListAsync<QueryResult.Company>();

                return new QueryResult
                {
                    Companies = companies
                };
            }
        }
    }
}
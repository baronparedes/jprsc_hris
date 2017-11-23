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
            public IEnumerable<CompanyProfile> Companies { get; set; } = new List<CompanyProfile>();

            public class CompanyProfile
            {
                public string Address { get; set; }
                public string Email { get; set; }
                public int Id { get; set; }
                public bool? IsDefault { get; set; }
                public string Name { get; set; }
                public string Phone { get; set; }
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
                    .CompanyProfiles
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
                    .ProjectToListAsync<QueryResult.CompanyProfile>();

                return new QueryResult
                {
                    Companies = companies
                };
            }
        }
    }
}
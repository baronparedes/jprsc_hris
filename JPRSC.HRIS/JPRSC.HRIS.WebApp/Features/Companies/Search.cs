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

namespace JPRSC.HRIS.WebApp.Features.Companies
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
            public IEnumerable<Company> Companies { get; set; } = new List<Company>();

            public class Company
            {
                public string Address { get; set; }
                public string Code { get; set; }
                public string Email { get; set; }
                public int Id { get; set; }
                public string Name { get; set; }
                public string Phone { get; set; }
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
                var pageNumber = query.PageNumber.HasValue && query.PageNumber > 0 ? query.PageNumber.Value : 1;
                var pageSize = query.PageSize.HasValue && query.PageSize > 0 ? Math.Min(query.PageSize.Value, 1000) : AppSettings.Int("DefaultGridPageSize");

                var dbQuery = _db
                    .Companies
                    .Where(cp => !cp.DeletedOn.HasValue);

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {
                    dbQuery = dbQuery
                        .Where(cp => DbFunctions.Like(cp.Name, query.SearchLikeTerm) ||
                            DbFunctions.Like(cp.Code, query.SearchLikeTerm) ||
                            DbFunctions.Like(cp.Address, query.SearchLikeTerm) ||
                            DbFunctions.Like(cp.Email, query.SearchLikeTerm) ||
                            DbFunctions.Like(cp.Phone, query.SearchLikeTerm));
                }

                var companies = await dbQuery
                    .OrderBy(cp => cp.Id)
                    .PageBy(pageNumber, pageSize)
                    .ProjectToListAsync<QueryResult.Company>();

                return new QueryResult
                {
                    Companies = companies
                };
            }
        }
    }
}
using AutoMapper;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Employees
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
            public IEnumerable<Employee> Employees { get; set; } = new List<Employee>();

            public class Employee
            {
                public string CompanyName { get; set; }
                public string EmployeeCode { get; set; }
                public string FirstName { get; set; }
                public int Id { get; set; }
                public string LastName { get; set; }
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
                    .Employees
                    .Where(r => !r.DeletedOn.HasValue);

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {
                    dbQuery = dbQuery
                        .Where(r => DbFunctions.Like(r.FirstName, query.SearchLikeTerm) ||
                            DbFunctions.Like(r.LastName, query.SearchLikeTerm));
                }

                var employees = await dbQuery
                    .Include(e => e.Company)
                    .OrderBy(e => e.Id)
                    .Take(AppSettings.Int("DefaultGridPageSize"))
                    .ProjectToListAsync<QueryResult.Employee>();

                return new QueryResult
                {
                    Employees = employees
                };
            }
        }
    }
}
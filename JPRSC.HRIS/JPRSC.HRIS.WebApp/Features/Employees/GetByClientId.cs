using AutoMapper;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Employees
{
    public class GetByClientId
    {
        public class Query : IRequest<QueryResult>
        {
            public int ClientId { get; set; }
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
                    .Employees
                    .Where(e => e.ClientId.HasValue && e.ClientId.Value == query.ClientId && !e.DeletedOn.HasValue);

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
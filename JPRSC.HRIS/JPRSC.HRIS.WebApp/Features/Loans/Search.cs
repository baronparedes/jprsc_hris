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

namespace JPRSC.HRIS.WebApp.Features.Loans
{
    public class Search
    {
        public class Query : IRequest<QueryResult>
        {
            public string SearchTerm { get; set; }
            public int? ClientId { get; set; }

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
                public decimal? COLADaily { get; set; }
                public decimal? COLAHourly { get; set; }
                public decimal? DailyRate { get; set; }
                public string EmployeeCode { get; set; }
                public string FirstName { get; set; }
                public decimal? HourlyRate { get; set; }
                public int Id { get; set; }
                public string LastName { get; set; }
                public ICollection<Loan> Loans { get; set; } = new List<Loan>();
            }

            public class Loan
            {
                public int? EmployeeId { get; set; }
                public int Id { get; set; }
                public double? InterestRate { get; set; }
                public DateTime? LoanDate { get; set; }
                public LoanType LoanType { get; set; }
                public int? LoanTypeId { get; set; }
                public int? MonthsPayable { get; set; }
                public int? PayrollPeriod { get; set; }
                public decimal? PrincipalAmount { get; set; }
                public string TransactionNumber { get; set; }
                public DateTime? ZeroedOutOn { get; set; }
            }

            public class LoanType
            {
                public string Code { get; set; }
                public string Description { get; set; }
                public int Id { get; set; }
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
                if (!query.ClientId.HasValue) return new QueryResult();

                var dbQuery = _db
                    .Employees
                    .Include(e => e.Loans)
                    .Include(e => e.Loans.Select(l => l.LoanType))
                    .Where(e => !e.DeletedOn.HasValue && e.ClientId == query.ClientId);

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {
                    dbQuery = dbQuery
                        .Where(e => DbFunctions.Like(e.FirstName, query.SearchLikeTerm) ||
                            DbFunctions.Like(e.LastName, query.SearchLikeTerm));
                }

                var employees = await dbQuery
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
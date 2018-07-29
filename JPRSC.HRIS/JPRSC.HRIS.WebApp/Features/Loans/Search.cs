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
            public IEnumerable<Loan> Loans { get; set; } = new List<Loan>();

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
                public string MiddleName { get; set; }
            }

            public class Loan
            {
                public decimal? DeductionAmount { get; set; }
                public Employee Employee { get; set; }
                public int? EmployeeId { get; set; }
                public int Id { get; set; }
                public DateTime? LoanDate { get; set; }
                public LoanType LoanType { get; set; }
                public int? LoanTypeId { get; set; }
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
                var dbQuery = _db
                    .Loans
                    .Include(l => l.Employee)
                    .Include(l => l.LoanType)
                    .Where(l => l.EmployeeId.HasValue && !l.DeletedOn.HasValue);

                if (query.ClientId.HasValue)
                {
                    dbQuery = dbQuery.Where(l => l.Employee.ClientId == query.ClientId);
                }

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {
                    dbQuery = dbQuery
                        .Where(l => DbFunctions.Like(l.Employee.EmployeeCode, query.SearchLikeTerm) ||
                            DbFunctions.Like(l.LoanType.Description, query.SearchLikeTerm));
                }

                var loans = await dbQuery
                    .OrderBy(l => l.Id)
                    .Take(AppSettings.Int("DefaultGridPageSize"))
                    .ProjectToListAsync<QueryResult.Loan>();

                return new QueryResult
                {
                    Loans = loans
                };
            }
        }
    }
}
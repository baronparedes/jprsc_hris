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
    public class GetById
    {
        public class Query : IRequest<QueryResult>
        {
            public int Id { get; set; }
        }

        public class QueryResult
        {
            public Loan LoanResult { get; set; }

            public class Client
            {
                public string Code { get; set; }
                public string Name { get; set; }
            }

            public class Employee
            {
                public Client Client { get; set; }
                public decimal? COLADaily { get; set; }
                public decimal? COLAHourly { get; set; }
                public decimal? DailyRate { get; set; }
                public string EmployeeCode { get; set; }
                public string FirstName { get; set; }
                public decimal? HourlyRate { get; set; }
                public int Id { get; set; }
                public string LastName { get; set; }
                public string MiddleName { get; set; }

                public string Name => String.IsNullOrWhiteSpace(MiddleName) ? $"{LastName}, {FirstName}" : $"{LastName}, {FirstName}, {MiddleName.First()}";
            }

            public class Loan
            {
                public decimal? DeductionAmount { get; set; }
                public Employee Employee { get; set; }
                public int? EmployeeId { get; set; }
                public int Id { get; set; }
                public decimal? InterestAmount { get; set; }
                public DateTime? LoanDate { get; set; }
                public string LoanPayrollPeriod { get; set; }
                public IList<int> LoanPayrollPeriods => String.IsNullOrWhiteSpace(LoanPayrollPeriod) ? new List<int>() : LoanPayrollPeriod.Split(',').Select(p => Convert.ToInt32(p)).ToList();
                public LoanType LoanType { get; set; }
                public int? LoanTypeId { get; set; }
                public int? MonthsPayable { get; set; }
                public decimal? PrincipalAmount { get; set; }
                public decimal? PrincipalAndInterestAmount { get; set; }
                public decimal? RemainingBalance { get; set; }
                public DateTime? StartDeductionDate { get; set; }
                public string TransactionNumber { get; set; }
                public DateTime? ZeroedOutOn { get; set; }

                public bool IsZeroedOut => ZeroedOutOn.HasValue;
                public decimal? TotalAmount => DeductionAmount * MonthsPayable;
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
                var loan = await _db
                    .Loans
                    .Include(l => l.Employee)
                    .Include(l => l.LoanType)
                    .Where(l => l.Id == query.Id)
                    .ProjectToSingleAsync<QueryResult.Loan>();

                return new QueryResult
                {
                    LoanResult = loan
                };
            }
        }
    }
}
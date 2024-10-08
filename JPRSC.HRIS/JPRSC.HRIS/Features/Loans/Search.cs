﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.Loans
{
    public class Search
    {
        public class Query : IRequest<QueryResult>
        {
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
            public string SearchTerm { get; set; }
            public int? ClientId { get; set; }
            public int? LoanTypeId { get; set; }

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
            public int LastPageNumber { get; set; }
            public int TotalResultsCount { get; set; }

            public class Employee
            {
                public int? ClientId { get; set; }
                public decimal? COLADaily { get; set; }
                public decimal? COLAHourly { get; set; }
                public decimal? COLAMonthly { get; set; }
                public decimal? DailyRate { get; set; }
                public string EmployeeCode { get; set; }
                public string FirstName { get; set; }
                public decimal? HourlyRate { get; set; }
                public int Id { get; set; }
                public string LastName { get; set; }
                public string MiddleName { get; set; }
                public decimal? MonthlyRate { get; set; }
            }

            public class Loan
            {
                public decimal? DeductionAmount { get; set; }
                public Employee Employee { get; set; }
                public int? EmployeeId { get; set; }
                public int Id { get; set; }
                public decimal? InterestAmount { get; set; }
                public DateTime? LoanDate { get; set; }
                public LoanType LoanType { get; set; }
                public int? LoanTypeId { get; set; }
                public int? PayrollPeriod { get; set; }
                public string LoanPayrollPeriod { get; set; }
                public IList<int> LoanPayrollPeriods => String.IsNullOrWhiteSpace(LoanPayrollPeriod) ? new List<int>() : LoanPayrollPeriod.Split(',').Select(p => Convert.ToInt32(p)).ToList();
                public decimal? PrincipalAmount { get; set; }
                public decimal? RemainingBalance { get; set; }
                public decimal? RemainingBalanceForDisplay => !RemainingBalance.HasValue ? (decimal?)null : Math.Max(0, RemainingBalance.Value);
                public DateTime? StartDeductionDate { get; set; }
                public string TransactionNumber { get; set; }
                public DateTime? ZeroedOutOn { get; set; }

                public bool IsZeroedOut => ZeroedOutOn.HasValue;
            }

            public class LoanType
            {
                public string Code { get; set; }
                public string Description { get; set; }
                public int Id { get; set; }
            }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Employee, QueryResult.Employee>();
                CreateMap<Loan, QueryResult.Loan>();
                CreateMap<LoanType, QueryResult.LoanType>();
            }
        }

        public class QueryHandler : IRequestHandler<Query, QueryResult>
        {
            private readonly ApplicationDbContext _db;
            private readonly IMapper _mapper;

            public QueryHandler(ApplicationDbContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<QueryResult> Handle(Query query, CancellationToken token)
            {
                var pageNumber = query.PageNumber.HasValue && query.PageNumber > 0 ? query.PageNumber.Value : 1;
                var pageSize = query.PageSize.HasValue && query.PageSize > 0 ? Math.Min(query.PageSize.Value, 1000) : AppSettings.Int("DefaultGridPageSize");

                var dbQuery = _db
                    .Loans
                    .AsNoTracking()
                    .Include(l => l.Employee)
                    .Include(l => l.LoanType)
                    .Where(l => l.EmployeeId.HasValue && !l.DeletedOn.HasValue);

                if (query.ClientId.HasValue)
                {
                    dbQuery = dbQuery.Where(l => l.Employee.ClientId == query.ClientId);
                }

                if (query.LoanTypeId.HasValue)
                {
                    dbQuery = dbQuery.Where(l => l.LoanTypeId == query.LoanTypeId);
                }

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {
                    dbQuery = dbQuery
                        .Where(l => DbFunctions.Like(l.Employee.EmployeeCode, query.SearchLikeTerm) ||
                            DbFunctions.Like(l.Employee.FirstName, query.SearchLikeTerm) ||
                            DbFunctions.Like(l.Employee.LastName, query.SearchLikeTerm));
                }

                var totalResultsCount = await dbQuery
                    .CountAsync();

                var loans = await dbQuery
                    .OrderBy(l => l.Employee.LastName)
                    .ThenBy(l => l.Employee.FirstName)
                    .PageBy(pageNumber, pageSize)
                    .ProjectTo<QueryResult.Loan>(_mapper)
                    .ToListAsync();

                var remainder = totalResultsCount % pageSize;
                var divisor = totalResultsCount / pageSize;
                var lastPageNumber = remainder > 0 ? divisor + 1 : divisor;

                return new QueryResult
                {
                    Loans = loans,
                    LastPageNumber = lastPageNumber,
                    TotalResultsCount = totalResultsCount
                };
            }
        }
    }
}
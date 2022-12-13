﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Employees
{
    public class Search
    {
        public class Query : IRequest<QueryResult>
        {
            public int? ClientId { get; set; }
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
            public IEnumerable<Employee> Employees { get; set; } = new List<Employee>();
            public int LastPageNumber { get; set; }
            public int TotalResultsCount { get; set; }

            public class Employee
            {
                public string ClientName { get; set; }
                public string CompanyName { get; set; }
                public string EmployeeCode { get; set; }
                public string FirstName { get; set; }
                public int Id { get; set; }
                [JsonIgnore] public bool IsActive { get; set; }
                public string LastName { get; set; }
                public string MiddleName { get; set; }
                [JsonIgnore] public string ResignStatus { get; set; }
                public bool RehireEnabled => !IsActive || (ResignStatus == Models.ResignStatus.Resigned || ResignStatus == Models.ResignStatus.AWOL);
            }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Employee, QueryResult.Employee>();
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
                    .Employees
                    .AsNoTracking()
                    .Where(e => !e.DeletedOn.HasValue);

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {
                    dbQuery = dbQuery
                        .Where(e => DbFunctions.Like(e.FirstName, query.SearchLikeTerm) ||
                            DbFunctions.Like(e.MiddleName, query.SearchLikeTerm) ||
                            DbFunctions.Like(e.LastName, query.SearchLikeTerm) ||
                            DbFunctions.Like(e.EmployeeCode, query.SearchLikeTerm) ||
                            DbFunctions.Like(e.CompanyIdNumber, query.SearchLikeTerm) ||
                            DbFunctions.Like(e.TIN, query.SearchLikeTerm) ||
                            DbFunctions.Like(e.PhilHealth, query.SearchLikeTerm) ||
                            DbFunctions.Like(e.PagIbig, query.SearchLikeTerm) ||
                            DbFunctions.Like(e.SSS, query.SearchLikeTerm) ||
                            DbFunctions.Like(e.ATMAccountNumber, query.SearchLikeTerm));
                }

                if (query.ClientId.HasValue)
                {
                    dbQuery = dbQuery.Where(e => e.ClientId == query.ClientId);
                }

                var totalResultsCount = await dbQuery
                    .CountAsync();

                var employees = await dbQuery
                    .Include(e => e.Company)
                    .OrderBy(e => e.LastName)
                    .ThenBy(e => e.FirstName)
                    .PageBy(pageNumber, pageSize)
                    .ProjectTo<QueryResult.Employee>(_mapper)
                    .ToListAsync();

                var remainder = totalResultsCount % pageSize;
                var divisor = totalResultsCount / pageSize;
                var lastPageNumber = remainder > 0 ? divisor + 1 : divisor;

                return new QueryResult
                {
                    Employees = employees,
                    LastPageNumber = lastPageNumber,
                    TotalResultsCount = totalResultsCount
                };
            }
        }
    }
}
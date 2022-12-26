using AutoMapper;
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

namespace JPRSC.HRIS.Features.Employees
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
                public decimal? COLADaily { get; set; }
                public decimal? COLAHourly { get; set; }
                public decimal? COLAMonthly { get; set; }
                public string CompanyName { get; set; }
                public decimal? DailyRate { get; set; }
                public string EmployeeCode { get; set; }
                public string FirstName { get; set; }
                public decimal? HourlyRate { get; set; }
                public int Id { get; set; }
                public string LastName { get; set; }
                public string MiddleName { get; set; }
                public string MiddleInitial => String.IsNullOrWhiteSpace(MiddleName) ? null : MiddleName.First().ToString();
                public decimal? MonthlyRate { get; set; }

                public string Name => String.IsNullOrWhiteSpace(MiddleName) ? $"{LastName}, {FirstName}" : $"{LastName}, {FirstName} {MiddleInitial}.";
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
                var dbQuery = _db
                    .Employees
                    .AsNoTracking()
                    .Where(e => e.ClientId.HasValue && e.ClientId.Value == query.ClientId && !e.DeletedOn.HasValue);

                var employees = await dbQuery
                    .Include(e => e.Company)
                    .OrderBy(e => e.LastName)
                    .ThenBy(e => e.FirstName)                    
                    .ProjectTo<QueryResult.Employee>(_mapper)
                    .ToListAsync();

                return new QueryResult
                {
                    Employees = employees
                };
            }
        }
    }
}
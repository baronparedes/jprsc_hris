using AutoMapper;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.DailyTimeRecords
{
    public class Search
    {
        public class Query : IRequest<QueryResult>
        {
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
            public string SearchTerm { get; set; }
            public int? ClientId { get; set; }
            public int? DailyTimeRecordPayrollPeriodBasisId { get; set; }

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
            public IEnumerable<DailyTimeRecord> DailyTimeRecords { get; set; } = new List<DailyTimeRecord>();

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
                public string Name => $"{LastName}, {FirstName}";
            }

            public class DailyTimeRecord
            {
                [JsonIgnore]
                public DateTime AddedOn { get; set; }
                public decimal? DailyRate { get; set; }
                public double? DaysWorked { get; set; }
                public decimal? DaysWorkedValue { get; set; }
                public Employee Employee { get; set; }
                public decimal? HourlyRate { get; set; }
                public double? HoursLate { get; set; }
                public decimal? HoursLateValue { get; set; }
                public double? HoursUndertime { get; set; }
                public decimal? HoursUndertimeValue { get; set; }
                public double? HoursWorked { get; set; }
                public decimal? HoursWorkedValue { get; set; }
                public int Id { get; set; }
                public DateTime? PayrollPeriodFrom { get; set; }
                public DateTime? PayrollPeriodTo { get; set; }
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

                var pageNumber = query.PageNumber.HasValue && query.PageNumber > 0 ? query.PageNumber.Value : 1;
                var pageSize = query.PageSize.HasValue && query.PageSize > 0 ? Math.Min(query.PageSize.Value, 1000) : AppSettings.Int("DefaultGridPageSize");

                var dbQuery = _db
                    .DailyTimeRecords
                    .Include(dtr => dtr.Employee)
                    .Where(dtr => !dtr.DeletedOn.HasValue && !dtr.Employee.DeletedOn.HasValue && dtr.Employee.ClientId == query.ClientId);

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {
                    dbQuery = dbQuery
                        .Where(dtr => DbFunctions.Like(dtr.Employee.FirstName, query.SearchLikeTerm) ||
                            DbFunctions.Like(dtr.Employee.LastName, query.SearchLikeTerm));
                }

                if (query.DailyTimeRecordPayrollPeriodBasisId.HasValue)
                {
                    var dailyTimeRecordPayrollPeriodBasis = await _db
                        .DailyTimeRecords
                        .FindAsync(query.DailyTimeRecordPayrollPeriodBasisId.Value);

                    dbQuery = dbQuery
                        .Where(dtr => DbFunctions.TruncateTime(dtr.PayrollPeriodFrom) == DbFunctions.TruncateTime(dailyTimeRecordPayrollPeriodBasis.PayrollPeriodFrom) &&
                                      DbFunctions.TruncateTime(dtr.PayrollPeriodTo) == DbFunctions.TruncateTime(dailyTimeRecordPayrollPeriodBasis.PayrollPeriodTo));
                }

                var dailyTimeRecords = await dbQuery
                    .OrderBy(e => e.Employee.LastName)
                    .ThenBy(e => e.Employee.FirstName)
                    .PageBy(pageNumber, pageSize)
                    .ProjectToListAsync<QueryResult.DailyTimeRecord>();

                return new QueryResult
                {
                    DailyTimeRecords = dailyTimeRecords
                };
            }
        }
    }
}
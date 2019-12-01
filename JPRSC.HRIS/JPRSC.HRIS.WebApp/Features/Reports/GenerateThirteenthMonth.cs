using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Excel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Reports
{
    public class GenerateThirteenthMonth
    {
        public class Query : IRequest<QueryResult>
        {
            public int? ClientId { get; set; }
            public string Destination { get; set; }
            public string DisplayMode { get; set; }
            public int? EmployeeId { get; set; }
            public int PayrollPeriodFromYear { get; set; }
            public int PayrollPeriodToYear { get; set; }
            public Month? FromPayrollPeriodMonth { get; set; }
            public int? FromPayrollPeriod { get; set; }
            public Month? ToPayrollPeriodMonth { get; set; }
            public int? ToPayrollPeriod { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(q => q.FromPayrollPeriodMonth)
                    .NotEmpty();

                RuleFor(q => q.FromPayrollPeriod)
                    .NotEmpty();

                RuleFor(q => q.ToPayrollPeriodMonth)
                    .NotEmpty();

                RuleFor(q => q.ToPayrollPeriod)
                    .NotEmpty();

                RuleFor(q => q.FromPayrollPeriodMonth)
                    .Must(BeBeforeToPayrollPeriodMonth)
                    .WithMessage("From payroll period month should be before To payroll period month.");

                RuleFor(q => q.PayrollPeriodFromYear)
                    .Must(BeOnOrBeforePayrollPeriodToYear)
                    .WithMessage("From Year must be the same as or before To Year.");
            }

            private bool BeBeforeToPayrollPeriodMonth(Query query, Month? fromPayrollPeriodMonth)
            {
                if (query.PayrollPeriodFromYear == query.PayrollPeriodToYear)
                {
                    return (int)fromPayrollPeriodMonth.Value <= (int)query.ToPayrollPeriodMonth.Value;
                }

                return true;
            }

            private bool BeOnOrBeforePayrollPeriodToYear(Query query, int payrollPeriodFromYear)
            {
                return payrollPeriodFromYear <= query.PayrollPeriodToYear;
            }
        }

        public class QueryResult
        {
            public int? ClientId { get; set; }
            public string ClientName { get; set; }
            public int PayrollPeriodFromYear { get; set; }
            public int PayrollPeriodToYear { get; set; }
            public Month? FromPayrollPeriodMonth { get; set; }
            public int? FromPayrollPeriod { get; set; }
            public Month? ToPayrollPeriodMonth { get; set; }
            public int? ToPayrollPeriod { get; set; }
            public string DisplayMode { get; set; }
            public byte[] FileContent { get; set; }
            public string Filename { get; set; }
            public IList<ThirteenthMonthRecord> ThirteenthMonthRecords { get; set; } = new List<ThirteenthMonthRecord>();
            public Query Query { get; set; }

            public IList<string> GetTotals(IList<ThirteenthMonthRecord> thirteenthMonthRecords)
            {
                var totals = new List<string> { String.Empty, "TOTAL" };

                for (var i = Query.PayrollPeriodFromYear; i <= Query.PayrollPeriodToYear; i++)
                {
                    var startingJ = 1;

                    if (i == Query.PayrollPeriodFromYear)
                    {
                        startingJ = (int)Query.FromPayrollPeriodMonth / 10;
                    }

                    for (var j = startingJ; j < 13; j++)
                    {
                        var key = ValueTuple.Create(i, j);

                        totals.Add(String.Format("{0:n}", thirteenthMonthRecords.Where(r => r.MonthRecords.ContainsKey(key)).Select(r => r.MonthRecords[key]).Sum(mr => mr.Total)));
                    }
                }

                totals.Add(String.Format("{0:n}", thirteenthMonthRecords.Sum(r => r.Half)));
                totals.Add(String.Format("{0:n}", thirteenthMonthRecords.Sum(r => r.Whole)));

                return totals;
            }

            public IList<string> GetMonthHeaders()
            {
                var monthHeaders = new List<string>();

                var months = new List<string> { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };

                for (var i = Query.PayrollPeriodFromYear; i <= Query.PayrollPeriodToYear; i++)
                {
                    if (i == Query.PayrollPeriodFromYear)
                    {
                        for (var j = 0; j < months.Count; j++)
                        {
                            if (j + 1 < (int)Query.FromPayrollPeriodMonth / 10) continue;

                            monthHeaders.Add(months[j]);
                        }
                    }
                    else
                    {
                        monthHeaders.AddRange(months);
                    }
                }

                return monthHeaders;
            }

            public class ThirteenthMonthRecord
            {
                public Employee Employee { get; set; }
                public IDictionary<ValueTuple<int, int>, MonthRecord> MonthRecords { get; set; } = new Dictionary<ValueTuple<int, int>, MonthRecord>();
                public Query Query { get; set; }

                public decimal Half => Whole / 2.0m;
                public decimal Whole => MonthRecords.Select(mr => mr.Value).Sum(v => v.Total) / 12.0m;
                public string ThirteenthMonthMode { get; set; }

                public IList<IList<string>> DisplayLineCollection
                {
                    get
                    {
                        var monthsPlaceholder = new List<string> { String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty };

                        var nameLine = new List<string> { $"{Employee.LastName}, {Employee.FirstName}{String.Format("{0}", String.IsNullOrWhiteSpace(Employee.MiddleName) ? null : $", {Employee.MiddleName}")}", $"{Employee.EmployeeCode}" };
                        for (var i = Query.PayrollPeriodFromYear; i <= Query.PayrollPeriodToYear; i++)
                        {
                            if (i == Query.PayrollPeriodFromYear)
                            {
                                for (var j = 0; j < monthsPlaceholder.Count; j++)
                                {
                                    if (j + 1 < (int)Query.FromPayrollPeriodMonth / 10) continue;

                                    nameLine.Add(monthsPlaceholder[j]);
                                }
                            }
                            else
                            {
                                nameLine.AddRange(monthsPlaceholder);
                            }
                        }
                        nameLine.Add(String.Empty);
                        nameLine.Add(String.Empty);

                        var basicLine = new List<string> { String.Empty, "BASIC" };
                        PopulateLine(basicLine, m => m.Basic);
                        basicLine.Add(String.Empty);
                        basicLine.Add(String.Empty);

                        var utTardyLine = new List<string> { String.Empty, "UT/TARDINESS" };
                        PopulateLine(utTardyLine, m => m.UTTardy);
                        utTardyLine.Add(String.Empty);
                        utTardyLine.Add(String.Empty);

                        var totalLine = new List<string> { String.Empty, "TOTAL" };
                        PopulateLine(totalLine, m => m.Total);
                        totalLine.Add(String.Format("{0:n}", Half));
                        totalLine.Add(String.Format("{0:n}", Whole));

                        var retVal = new List<IList<string>>();
                        retVal.Add(nameLine);
                        retVal.Add(basicLine);
                        retVal.Add(utTardyLine);
                        retVal.Add(totalLine);

                        return retVal;
                    }
                }

                private void PopulateLine(IList<string> line, Func<MonthRecord, decimal> valueGetter)
                {
                    for (var i = Query.PayrollPeriodFromYear; i <= Query.PayrollPeriodToYear; i++)
                    {
                        var startingJ = 1;

                        if (i == Query.PayrollPeriodFromYear)
                        {
                            startingJ = (int)Query.FromPayrollPeriodMonth / 10;
                        }

                        for (var j = startingJ; j < 13; j++)
                        {
                            var key = ValueTuple.Create(i, j);

                            if (MonthRecords.ContainsKey(key))
                            {
                                line.Add(String.Format("{0:n}", valueGetter(MonthRecords[key])));
                            }
                            else
                            {
                                line.Add(String.Empty);
                            }
                        }
                    }
                }
            }

            public class MonthRecord
            {
                public decimal Basic { get; set; }
                public decimal UTTardy { get; set; }
                public decimal Total => Basic - UTTardy;
            }
        }

        public class QueryHandler : IRequestHandler<Query, QueryResult>
        {
            private readonly ApplicationDbContext _db;
            private readonly IExcelBuilder _excelBuilder;
            private readonly IMediator _mediator;

            public QueryHandler(ApplicationDbContext db, IExcelBuilder excelBuilder, IMediator mediator)
            {
                _db = db;
                _excelBuilder = excelBuilder;
                _mediator = mediator;
            }

            public async Task<QueryResult> Handle(Query query, CancellationToken token)
            {
                var clients = query.ClientId == -1 ?
                    await _db.Clients.AsNoTracking().Where(c => !c.DeletedOn.HasValue).ToListAsync() :
                    await _db.Clients.AsNoTracking().Where(c => !c.DeletedOn.HasValue && c.Id == query.ClientId.Value).ToListAsync();

                var clientIds = clients.Select(c => c.Id).ToList();

                var allPayrollProcessBatches = new List<PayrollProcessBatch>();

                for (var i = query.PayrollPeriodFromYear; i <= query.PayrollPeriodToYear; i++)
                {
                    DateTime startDate;
                    DateTime endDate;

                    int fromPayrollPeriodMonthAsInt;
                    int toPayrollPeriodMonthAsInt;

                    if (i == query.PayrollPeriodFromYear)
                    {
                        startDate = new DateTime(i, (int)query.FromPayrollPeriodMonth / 10, 1);
                        fromPayrollPeriodMonthAsInt = (int)query.FromPayrollPeriodMonth.Value;
                    }
                    else
                    {
                        startDate = new DateTime(i, 1, 1);
                        fromPayrollPeriodMonthAsInt = (int)Month.January;
                    }

                    startDate = startDate.AddMonths(-1);

                    if (i == query.PayrollPeriodToYear)
                    {
                        endDate = new DateTime(i, (int)query.ToPayrollPeriodMonth / 10, 1).AddMonths(1).AddDays(-1);
                        toPayrollPeriodMonthAsInt = (int)query.ToPayrollPeriodMonth.Value;
                    }
                    else
                    {
                        endDate = new DateTime(i, 12, 31);
                        toPayrollPeriodMonthAsInt = (int)Month.December;
                    }

                    var forOnlyOnePayrollPeriod = query.PayrollPeriodFromYear == query.PayrollPeriodToYear && query.FromPayrollPeriod == query.ToPayrollPeriod && query.FromPayrollPeriodMonth == query.ToPayrollPeriodMonth;
                    if (forOnlyOnePayrollPeriod)
                    {
                        var onlyPayrollProcessbatch = await _db.PayrollProcessBatches
                            .AsNoTracking()
                            .Include(ppb => ppb.PayrollRecords)
                            .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                            .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee.Department))
                            .Where(ppb => !ppb.DeletedOn.HasValue &&
                                clientIds.Contains(ppb.ClientId.Value) &&
                                DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) >= startDate &&
                                DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) <= endDate &&
                                (int)ppb.PayrollPeriodMonth == fromPayrollPeriodMonthAsInt &&
                                ppb.PayrollPeriod == query.FromPayrollPeriod)
                            .ToListAsync();

                        allPayrollProcessBatches.AddRange(onlyPayrollProcessbatch);
                    }
                    else
                    {
                        var payrollProcessBatchesInBeginningMonth = await _db.PayrollProcessBatches
                            .AsNoTracking()
                            .Include(ppb => ppb.PayrollRecords)
                            .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                            .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee.Department))
                            .Where(ppb => !ppb.DeletedOn.HasValue &&
                                clientIds.Contains(ppb.ClientId.Value) &&
                                DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) >= startDate &&
                                DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) <= endDate &&
                                (int)ppb.PayrollPeriodMonth == fromPayrollPeriodMonthAsInt &&
                                ppb.PayrollPeriod >= query.FromPayrollPeriod)
                            .ToListAsync();

                        var payrollProcessBatchesInEndingMonth = await _db.PayrollProcessBatches
                            .AsNoTracking()
                            .Include(ppb => ppb.PayrollRecords)
                            .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                            .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee.Department))
                            .Where(ppb => !ppb.DeletedOn.HasValue &&
                                clientIds.Contains(ppb.ClientId.Value) &&
                                DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) >= startDate &&
                                DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) <= endDate &&
                                (int)ppb.PayrollPeriodMonth == toPayrollPeriodMonthAsInt &&
                                ppb.PayrollPeriod <= query.ToPayrollPeriod)
                            .ToListAsync();

                        var payrollProcessBatchesInBetween = await _db.PayrollProcessBatches
                            .AsNoTracking()
                            .Include(ppb => ppb.PayrollRecords)
                            .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                            .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee.Department))
                            .Where(ppb => !ppb.DeletedOn.HasValue &&
                                clientIds.Contains(ppb.ClientId.Value) &&
                                DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) >= startDate &&
                                DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) <= endDate &&
                                (int)ppb.PayrollPeriodMonth > fromPayrollPeriodMonthAsInt &&
                                (int)ppb.PayrollPeriodMonth < toPayrollPeriodMonthAsInt)
                            .ToListAsync();

                        var payrollProcessBatchesBetweenYears = new List<PayrollProcessBatch>();
                        if (startDate.Year == endDate.Year - 1)
                        {
                            var betweenYearStartDate = new DateTime(startDate.Year, 12, 1);
                            var betweenYearEndDate = new DateTime(endDate.Year, 1, 1);

                            payrollProcessBatchesBetweenYears = await _db.PayrollProcessBatches
                                .AsNoTracking()
                                .Include(ppb => ppb.PayrollRecords)
                                .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                                .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee.Department))
                                .Where(ppb => !ppb.DeletedOn.HasValue &&
                                    clientIds.Contains(ppb.ClientId.Value) &&
                                    DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) >= betweenYearStartDate &&
                                    DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) <= betweenYearEndDate)
                                .ToListAsync();
                        }

                        var payrollProcessBatches = payrollProcessBatchesInBeginningMonth
                            .Concat(payrollProcessBatchesInBetween)
                            .Concat(payrollProcessBatchesInEndingMonth)
                            .Concat(payrollProcessBatchesBetweenYears);

                        allPayrollProcessBatches.AddRange(payrollProcessBatches);
                    }
                }

                allPayrollProcessBatches = allPayrollProcessBatches
                    .GroupBy(ppb => ppb.Id)
                    .Select(g => g.First())
                    .OrderBy(ppb => ppb.Id)
                    .ToList();

                var thirteenthMonthRecords = GetThirteenthMonthRecords(query, allPayrollProcessBatches);

                if (query.Destination == "Excel")
                {
                    var queryResult = new QueryResult
                    {
                        ClientId = query.ClientId,
                        DisplayMode = query.DisplayMode,
                        ThirteenthMonthRecords = thirteenthMonthRecords,
                        PayrollPeriodFromYear = query.PayrollPeriodFromYear,
                        PayrollPeriodToYear = query.PayrollPeriodToYear,
                        FromPayrollPeriodMonth = query.FromPayrollPeriodMonth,
                        FromPayrollPeriod = query.FromPayrollPeriod,
                        ToPayrollPeriodMonth = query.ToPayrollPeriodMonth,
                        ToPayrollPeriod = query.ToPayrollPeriod,
                        Query = query
                    };

                    var excelLines = new List<IList<string>>();

                    foreach (var thirteenthMonthRecord in thirteenthMonthRecords)
                    {
                        foreach (var displayLine in thirteenthMonthRecord.DisplayLineCollection)
                        {
                            excelLines.Add(displayLine);
                        }
                    }

                    var header = new List<string> { "Employee Name", "Employee Code" };
                    var monthHeaders = queryResult.GetMonthHeaders();
                    foreach (var monthHeader in monthHeaders)
                    {
                        header.Add(monthHeader);
                    }
                    header.Add("HALF");
                    header.Add("13th Month Pay");

                    excelLines.Insert(0, header);

                    var reportFileContent = _excelBuilder.BuildExcelFile(excelLines);

                    var reportFileNameBuilder = new StringBuilder(64);
                    reportFileNameBuilder.Append($"Thirteenth Month Report - ");

                    if (query.ClientId == -1)
                    {
                        reportFileNameBuilder.Append("All Clients");
                    }
                    else
                    {
                        reportFileNameBuilder.Append(clients.Single().Name);
                    }

                    reportFileNameBuilder.Append(" - ");

                    reportFileNameBuilder.Append($"{query.PayrollPeriodFromYear} to {query.PayrollPeriodToYear}");

                    reportFileNameBuilder.Append(".xlsx");

                    queryResult.FileContent = reportFileContent;
                    queryResult.Filename = reportFileNameBuilder.ToString();

                    return queryResult;
                }
                else
                {
                    var clientName = String.Empty;
                    if (query.ClientId.HasValue && query.ClientId.Value > 0)
                    {
                        clientName = (await _db.Clients.AsNoTracking().SingleAsync(c => c.Id == query.ClientId)).Name;
                    }

                    return new QueryResult
                    {
                        ClientId = query.ClientId,
                        ClientName = clientName,
                        DisplayMode = query.DisplayMode,
                        ThirteenthMonthRecords = thirteenthMonthRecords,
                        PayrollPeriodFromYear = query.PayrollPeriodFromYear,
                        PayrollPeriodToYear = query.PayrollPeriodToYear,
                        FromPayrollPeriodMonth = query.FromPayrollPeriodMonth,
                        FromPayrollPeriod = query.FromPayrollPeriod,
                        ToPayrollPeriodMonth = query.ToPayrollPeriodMonth,
                        ToPayrollPeriod = query.ToPayrollPeriod,
                        Query = query
                    };
                }
            }

            private IList<QueryResult.ThirteenthMonthRecord> GetThirteenthMonthRecords(Query query, IList<PayrollProcessBatch> payrollProcessBatches)
            {
                var thirteenthMonthRecordsDictionary = new Dictionary<int, QueryResult.ThirteenthMonthRecord>();

                for (var i = query.PayrollPeriodFromYear; i <= query.PayrollPeriodToYear; i++)
                {
                    var startingJ = 1;
                    var endingJ = 13;

                    if (i == query.PayrollPeriodFromYear)
                    {
                        startingJ = (int)query.FromPayrollPeriodMonth / 10;
                    }

                    if (i == query.PayrollPeriodToYear)
                    {
                        endingJ = (int)query.ToPayrollPeriodMonth / 10;
                    }

                    for (var j = startingJ; j <= endingJ; j++)
                    {
                        var batchesGroupedByPayrollPeriod = payrollProcessBatches
                            .GroupBy(ppb => ppb.PayrollPeriod.Value);

                        foreach (var group in batchesGroupedByPayrollPeriod)
                        {
                            var payrollRecordsInMonth = group
                                .Where(ppb => ppb.PayrollPeriodFrom.Value.Year == i && (int)ppb.PayrollPeriodMonth / 10 == j)
                                .SelectMany(ppb => ppb.PayrollRecords)
                                .ToList();

                            if (query.EmployeeId.HasValue && query.EmployeeId.Value > 0)
                            {
                                payrollRecordsInMonth = payrollRecordsInMonth
                                    .Where(pr => pr.EmployeeId == query.EmployeeId.Value)
                                    .ToList();
                            }

                            var isJanuaryFirstPayrollPeriod = j == 1 && group.Key == 1;
                            if (isJanuaryFirstPayrollPeriod)
                            {
                                var payrollRecordsInJanuaryFirstPeriod = group
                                    .Where(ppb => ppb.PayrollPeriodFrom.Value.Year == i - 1 && (int)ppb.PayrollPeriodMonth / 10 == j)
                                    .SelectMany(ppb => ppb.PayrollRecords)
                                    .ToList();

                                if (query.EmployeeId.HasValue && query.EmployeeId.Value > 0)
                                {
                                    payrollRecordsInJanuaryFirstPeriod = payrollRecordsInJanuaryFirstPeriod
                                        .Where(pr => pr.EmployeeId == query.EmployeeId.Value)
                                        .ToList();
                                }

                                if (payrollRecordsInJanuaryFirstPeriod.Any())
                                {
                                    foreach (var record in payrollRecordsInJanuaryFirstPeriod)
                                    {
                                        payrollRecordsInMonth.Add(record);
                                    }
                                }
                            }

                            payrollRecordsInMonth = payrollRecordsInMonth
                                .OrderBy(pr => pr.Employee.LastName)
                                .ThenBy(pr => pr.Employee.FirstName)
                                .ToList();

                            AddPayrollRecordsToThirteenthMonthRecordsDictionary(query, thirteenthMonthRecordsDictionary, i, j, group, payrollRecordsInMonth);
                        }
                    }
                }

                return thirteenthMonthRecordsDictionary.Select(t => t.Value).OrderBy(t => t.Employee.LastName).ThenBy(t => t.Employee.FirstName).ToList();
            }

            private static void AddPayrollRecordsToThirteenthMonthRecordsDictionary(Query query, Dictionary<int, QueryResult.ThirteenthMonthRecord> thirteenthMonthRecordsDictionary, int year, int month, IGrouping<int, PayrollProcessBatch> group, List<PayrollRecord> payrollRecordsInMonth)
            {
                foreach (var payrollRecord in payrollRecordsInMonth)
                {
                    if (!thirteenthMonthRecordsDictionary.ContainsKey(payrollRecord.EmployeeId.Value))
                    {
                        thirteenthMonthRecordsDictionary.Add(payrollRecord.EmployeeId.Value, new QueryResult.ThirteenthMonthRecord { Employee = payrollRecord.Employee, Query = query });
                    }

                    var key = ValueTuple.Create(year, month);
                    if (!thirteenthMonthRecordsDictionary[payrollRecord.EmployeeId.Value].MonthRecords.ContainsKey(key))
                    {
                        var newMonthRecord = new QueryResult.MonthRecord
                        {
                            Basic = payrollRecord.DaysWorkedValue.GetValueOrDefault() + payrollRecord.HoursWorkedValue.GetValueOrDefault(),
                            UTTardy = payrollRecord.HoursLateValue.GetValueOrDefault() + payrollRecord.HoursUndertimeValue.GetValueOrDefault()
                        };
                        thirteenthMonthRecordsDictionary[payrollRecord.EmployeeId.Value].MonthRecords.Add(key, newMonthRecord);
                    }
                    else
                    {
                        var monthRecord = thirteenthMonthRecordsDictionary[payrollRecord.EmployeeId.Value].MonthRecords[key];

                        monthRecord.Basic += payrollRecord.DaysWorkedValue.GetValueOrDefault() + payrollRecord.HoursWorkedValue.GetValueOrDefault();
                        monthRecord.UTTardy += payrollRecord.HoursLateValue.GetValueOrDefault() + payrollRecord.HoursUndertimeValue.GetValueOrDefault();
                    }
                }
            }
        }
    }
}
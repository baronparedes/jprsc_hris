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
                    for (var j = 1; j < 13; j++)
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
                    //monthHeaders.AddRange(months);
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

                        var lineOne = new List<string> { $"{Employee.LastName}, {Employee.FirstName}{String.Format("{0}", String.IsNullOrWhiteSpace(Employee.MiddleName) ? null : $", {Employee.MiddleName}")}", $"{Employee.EmployeeCode}" };
                        for (var i = Query.PayrollPeriodFromYear; i <= Query.PayrollPeriodToYear; i++)
                        {
                            if (i == Query.PayrollPeriodFromYear)
                            {
                                for (var j = 0; j < monthsPlaceholder.Count; j++)
                                {
                                    if (j + 1 < (int)Query.FromPayrollPeriodMonth / 10) continue;

                                    lineOne.Add(monthsPlaceholder[j]);
                                }
                            }
                            else
                            {
                                lineOne.AddRange(monthsPlaceholder);
                            }
                        }
                        lineOne.Add(String.Empty);
                        lineOne.Add(String.Empty);

                        var lineTwo = new List<string> { String.Empty, "BASIC" };

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
                                    lineTwo.Add(String.Format("{0:n}", MonthRecords[key].Basic));
                                }
                                else
                                {
                                    lineTwo.Add(String.Empty);
                                }
                            }
                        }

                        lineTwo.Add(String.Empty);
                        lineTwo.Add(String.Empty);

                        var lineThree = new List<string> { String.Empty, "UT/TARDINESS" };
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
                                    lineThree.Add(String.Format("{0:n}", MonthRecords[key].UTTardy));
                                }
                                else
                                {
                                    lineThree.Add(String.Empty);
                                }
                            }
                        }
                        lineThree.Add(String.Empty);
                        lineThree.Add(String.Empty);

                        var lineFour = new List<string> { String.Empty, "TOTAL" };
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
                                    lineFour.Add(String.Format("{0:n}", MonthRecords[key].Total));
                                }
                                else
                                {
                                    lineFour.Add(String.Empty);
                                }
                            }
                        }
                        lineFour.Add(String.Format("{0:n}", Half));
                        lineFour.Add(String.Format("{0:n}", Whole));

                        return new List<IList<string>> { lineOne, lineTwo, lineThree, lineFour };
                    }
                }
            }

            public class MonthRecord
            {
                public int Month { get; set; }
                public int Year { get; set; }
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
                    await _db.Clients.Where(c => !c.DeletedOn.HasValue).ToListAsync() :
                    await _db.Clients.Where(c => !c.DeletedOn.HasValue && c.Id == query.ClientId.Value).ToListAsync();

                var clientIds = clients.Select(c => c.Id).ToList();

                var allPayrollProcessBatches = new List<PayrollProcessBatch>();

                for (var i = query.PayrollPeriodFromYear; i <= query.PayrollPeriodToYear; i++)
                {
                    DateTime startDate;
                    DateTime endDate;

                    if (i == query.PayrollPeriodFromYear)
                    {
                        startDate = new DateTime(i, (int)query.FromPayrollPeriodMonth / 10, 1);
                    }
                    else
                    {
                        startDate = new DateTime(i, 1, 1);
                    }

                    startDate = startDate.AddMonths(-1);

                    if (i == query.PayrollPeriodToYear)
                    {
                        endDate = new DateTime(i, (int)query.ToPayrollPeriodMonth / 10, 1).AddMonths(1).AddDays(-1);
                    }
                    else
                    {
                        endDate = new DateTime(i, 12, 31);
                    }

                    var fromPayrollPeriodMonthAsInt = (int)query.FromPayrollPeriodMonth.Value;
                    var toPayrollPeriodMonthAsInt = (int)query.ToPayrollPeriodMonth.Value;

                    var payrollProcessBatchesInBeginningMonth = await _db.PayrollProcessBatches
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

                    var payrollProcessBatches = payrollProcessBatchesInBeginningMonth
                        .Concat(payrollProcessBatchesInBetween)
                        .Concat(payrollProcessBatchesInEndingMonth);

                    allPayrollProcessBatches.AddRange(payrollProcessBatches);
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
                        clientName = (await _db.Clients.SingleAsync(c => c.Id == query.ClientId)).Name;
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

                if (query.FromPayrollPeriodMonth == Month.January)
                {
                    var j = 1;

                    var payrollRecordsInMonth = payrollProcessBatches
                            //.Where(ppb => ppb.PayrollPeriodFrom.Value.Year == i && ppb.PayrollPeriodFrom.Value.Month == j)
                            .Where(ppb => ppb.PayrollPeriodFrom.Value.Year == query.PayrollPeriodFromYear - 1 && (int)ppb.PayrollPeriodMonth / 10 == j)
                            .SelectMany(ppb => ppb.PayrollRecords)
                            .OrderBy(pr => pr.Employee.LastName)
                            .ThenBy(pr => pr.Employee.FirstName)
                            .ToList();

                    foreach (var payrollRecord in payrollRecordsInMonth)
                    {
                        if (!thirteenthMonthRecordsDictionary.ContainsKey(payrollRecord.EmployeeId.Value))
                        {
                            thirteenthMonthRecordsDictionary.Add(payrollRecord.EmployeeId.Value, new QueryResult.ThirteenthMonthRecord { Employee = payrollRecord.Employee, Query = query });
                        }

                        var key = ValueTuple.Create(query.PayrollPeriodFromYear, j);
                        if (!thirteenthMonthRecordsDictionary[payrollRecord.EmployeeId.Value].MonthRecords.ContainsKey(key))
                        {
                            thirteenthMonthRecordsDictionary[payrollRecord.EmployeeId.Value].MonthRecords.Add(key, new QueryResult.MonthRecord
                            {
                                Basic = payrollRecord.DaysWorkedValue.GetValueOrDefault() + payrollRecord.HoursWorkedValue.GetValueOrDefault(),
                                UTTardy = payrollRecord.HoursLateValue.GetValueOrDefault() + payrollRecord.HoursUndertimeValue.GetValueOrDefault(),
                                Month = j,
                                Year = query.PayrollPeriodFromYear
                            });
                        }
                        else
                        {
                            thirteenthMonthRecordsDictionary[payrollRecord.EmployeeId.Value].MonthRecords[key].Basic += payrollRecord.DaysWorkedValue.GetValueOrDefault() + payrollRecord.HoursWorkedValue.GetValueOrDefault();
                            thirteenthMonthRecordsDictionary[payrollRecord.EmployeeId.Value].MonthRecords[key].UTTardy += payrollRecord.HoursLateValue.GetValueOrDefault() + payrollRecord.HoursUndertimeValue.GetValueOrDefault();
                        }
                    }
                }

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
                        var payrollRecordsInMonth = payrollProcessBatches
                            //.Where(ppb => ppb.PayrollPeriodFrom.Value.Year == i && ppb.PayrollPeriodFrom.Value.Month == j)
                            .Where(ppb => ppb.PayrollPeriodFrom.Value.Year == i && (int)ppb.PayrollPeriodMonth / 10 == j)
                            .SelectMany(ppb => ppb.PayrollRecords)
                            .OrderBy(pr => pr.Employee.LastName)
                            .ThenBy(pr => pr.Employee.FirstName)
                            .ToList();

                        foreach (var payrollRecord in payrollRecordsInMonth)
                        {
                            if (!thirteenthMonthRecordsDictionary.ContainsKey(payrollRecord.EmployeeId.Value))
                            {
                                thirteenthMonthRecordsDictionary.Add(payrollRecord.EmployeeId.Value, new QueryResult.ThirteenthMonthRecord { Employee = payrollRecord.Employee, Query = query });
                            }

                            var key = ValueTuple.Create(i, j);
                            if (!thirteenthMonthRecordsDictionary[payrollRecord.EmployeeId.Value].MonthRecords.ContainsKey(key))
                            {
                                thirteenthMonthRecordsDictionary[payrollRecord.EmployeeId.Value].MonthRecords.Add(key, new QueryResult.MonthRecord
                                {
                                    Basic = payrollRecord.DaysWorkedValue.GetValueOrDefault() + payrollRecord.HoursWorkedValue.GetValueOrDefault(),
                                    UTTardy = payrollRecord.HoursLateValue.GetValueOrDefault() + payrollRecord.HoursUndertimeValue.GetValueOrDefault(),
                                    Month = j,
                                    Year = i
                                });
                            }
                            else
                            {
                                thirteenthMonthRecordsDictionary[payrollRecord.EmployeeId.Value].MonthRecords[key].Basic += payrollRecord.DaysWorkedValue.GetValueOrDefault() + payrollRecord.HoursWorkedValue.GetValueOrDefault();
                                thirteenthMonthRecordsDictionary[payrollRecord.EmployeeId.Value].MonthRecords[key].UTTardy += payrollRecord.HoursLateValue.GetValueOrDefault() + payrollRecord.HoursUndertimeValue.GetValueOrDefault();
                            }
                        }
                    }
                }

                return thirteenthMonthRecordsDictionary.Select(t => t.Value).OrderBy(t => t.Employee.LastName).ThenBy(t => t.Employee.FirstName).ToList();
            }
        }
    }
}
﻿using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Features.Payroll;
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
            public int PayrollPeriodYear { get; set; }
        }

        public class QueryResult
        {
            public int? ClientId { get; set; }
            public string ClientName { get; set; }
            public int PayrollPeriodYear { get; set; }
            public string DisplayMode { get; set; }
            public byte[] FileContent { get; set; }
            public string Filename { get; set; }
            public string ThirteenthMonthMode { get; set; }
            public IList<ThirteenthMonthRecord> ThirteenthMonthRecords { get; set; } = new List<ThirteenthMonthRecord>();

            public IList<string> GetTotals(IList<ThirteenthMonthRecord> thirteenthMonthRecords)
            {
                var totals = new List<string> { String.Empty, "TOTAL" };

                if (ThirteenthMonthMode == "December")
                {
                    for (var i = 1; i < 13; i++)
                    {
                        totals.Add(String.Format("{0:n}", thirteenthMonthRecords.Where(r => r.MonthRecords.ContainsKey(i)).Select(r => r.MonthRecords[i]).Sum(mr => mr.Total)));
                    }
                }
                else if (ThirteenthMonthMode == "October")
                {
                    for (var i = 11; i < 13; i++)
                    {
                        totals.Add(String.Format("{0:n}", thirteenthMonthRecords.Where(r => r.MonthRecords.ContainsKey(i)).Select(r => r.MonthRecords[i]).Sum(mr => mr.Total)));
                    }
                    for (var i = 1; i < 11; i++)
                    {
                        totals.Add(String.Format("{0:n}", thirteenthMonthRecords.Where(r => r.MonthRecords.ContainsKey(i)).Select(r => r.MonthRecords[i]).Sum(mr => mr.Total)));
                    }
                }

                totals.Add(String.Format("{0:n}", thirteenthMonthRecords.Sum(r => r.Half)));
                totals.Add(String.Format("{0:n}", thirteenthMonthRecords.Sum(r => r.Whole)));

                return totals;
            }

            public class ThirteenthMonthRecord
            {
                public Employee Employee { get; set; }
                public IDictionary<int, MonthRecord> MonthRecords { get; set; } = new Dictionary<int, MonthRecord>();

                public decimal Half => Whole / 2.0m;
                public decimal Whole => MonthRecords.Select(mr => mr.Value).Sum(v => v.Total);
                public string ThirteenthMonthMode { get; set; }

                public IList<IList<string>> DisplayLineCollection
                {
                    get
                    {
                        var lineOne = new List<string> { $"{Employee.LastName}, {Employee.FirstName}{String.Format("{0}", String.IsNullOrWhiteSpace(Employee.MiddleName) ? null : $", {Employee.MiddleName}")}", $"{Employee.EmployeeCode}", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty };

                        var lineTwo = new List<string> { String.Empty, "BASIC" };
                        if (ThirteenthMonthMode == "December")
                        {
                            for (var i = 1; i < 13; i++)
                            {
                                if (MonthRecords.ContainsKey(i))
                                {
                                    lineTwo.Add(String.Format("{0:n}", MonthRecords[i].Basic));
                                }
                                else
                                {
                                    lineTwo.Add(String.Empty);
                                }
                            }
                        }
                        else if (ThirteenthMonthMode == "October")
                        {
                            for (var i = 11; i < 13; i++)
                            {
                                if (MonthRecords.ContainsKey(i))
                                {
                                    lineTwo.Add(String.Format("{0:n}", MonthRecords[i].Basic));
                                }
                                else
                                {
                                    lineTwo.Add(String.Empty);
                                }
                            }
                            for (var i = 1; i < 11; i++)
                            {
                                if (MonthRecords.ContainsKey(i))
                                {
                                    lineTwo.Add(String.Format("{0:n}", MonthRecords[i].Basic));
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
                        if (ThirteenthMonthMode == "December")
                        {
                            for (var i = 1; i < 13; i++)
                            {
                                if (MonthRecords.ContainsKey(i))
                                {
                                    lineThree.Add(String.Format("{0:n}", MonthRecords[i].UTTardy));
                                }
                                else
                                {
                                    lineThree.Add(String.Empty);
                                }
                            }
                        }
                        else if (ThirteenthMonthMode == "October")
                        {
                            for (var i = 11; i < 13; i++)
                            {
                                if (MonthRecords.ContainsKey(i))
                                {
                                    lineThree.Add(String.Format("{0:n}", MonthRecords[i].UTTardy));
                                }
                                else
                                {
                                    lineThree.Add(String.Empty);
                                }
                            }
                            for (var i = 1; i < 11; i++)
                            {
                                if (MonthRecords.ContainsKey(i))
                                {
                                    lineThree.Add(String.Format("{0:n}", MonthRecords[i].UTTardy));
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
                        if (ThirteenthMonthMode == "December")
                        {
                            for (var i = 1; i < 13; i++)
                            {
                                if (MonthRecords.ContainsKey(i))
                                {
                                    lineFour.Add(String.Format("{0:n}", MonthRecords[i].Total));
                                }
                                else
                                {
                                    lineFour.Add(String.Empty);
                                }
                            }
                        }
                        else if (ThirteenthMonthMode == "October")
                        {
                            for (var i = 11; i < 13; i++)
                            {
                                if (MonthRecords.ContainsKey(i))
                                {
                                    lineFour.Add(String.Format("{0:n}", MonthRecords[i].Total));
                                }
                                else
                                {
                                    lineFour.Add(String.Empty);
                                }
                            }
                            for (var i = 1; i < 11; i++)
                            {
                                if (MonthRecords.ContainsKey(i))
                                {
                                    lineFour.Add(String.Format("{0:n}", MonthRecords[i].Total));
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
            private readonly string _thirteenthMonthMode;

            public QueryHandler(ApplicationDbContext db, IExcelBuilder excelBuilder, IMediator mediator)
            {
                _db = db;
                _excelBuilder = excelBuilder;
                _mediator = mediator;

                _thirteenthMonthMode = AppSettings.String("ThirteenthMonthMode");
                if (_thirteenthMonthMode != "December" && _thirteenthMonthMode != "October") throw new Exception($"Unsupported thirteenth month mode: {_thirteenthMonthMode}");
            }

            public async Task<QueryResult> Handle(Query query, CancellationToken token)
            {
                var clients = query.ClientId == -1 ?
                    await _db.Clients.Where(c => !c.DeletedOn.HasValue).ToListAsync() :
                    await _db.Clients.Where(c => !c.DeletedOn.HasValue && c.Id == query.ClientId.Value).ToListAsync();

                var clientIds = clients.Select(c => c.Id).ToList();

                var startDate = DateTime.MinValue;
                var endDate = DateTime.Now;

                if (_thirteenthMonthMode == "December")
                {
                    startDate = new DateTime(query.PayrollPeriodYear, 1, 1);
                    endDate = new DateTime(query.PayrollPeriodYear, 12, 31);
                }
                else if (_thirteenthMonthMode == "October")
                {
                    startDate = new DateTime(query.PayrollPeriodYear, 11, 1);
                    endDate = new DateTime(query.PayrollPeriodYear + 1, 10, 31);
                }

                var payrollProcessBatches = await _db.PayrollProcessBatches
                    .Include(ppb => ppb.PayrollRecords)
                    .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                    .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee.Department))
                    .Where(ppb => !ppb.DeletedOn.HasValue && clientIds.Contains(ppb.ClientId.Value) && DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) >= startDate && DbFunctions.TruncateTime(ppb.PayrollPeriodTo.Value) <= endDate)
                    .ToListAsync();

                var thirteenthMonthRecords = GetThirteenthMonthRecords(payrollProcessBatches);

                if (query.Destination == "Excel")
                {
                    var excelLines = new List<IList<string>>();

                    foreach (var thirteenthMonthRecord in thirteenthMonthRecords)
                    {
                        foreach (var displayLine in thirteenthMonthRecord.DisplayLineCollection)
                        {
                            excelLines.Add(displayLine);
                        }
                    }

                    if (_thirteenthMonthMode == "December")
                    {
                        excelLines.Insert(0, new List<string> { "Employee Name", "Employee Code", "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEPT", "OCT", "NOV", "DEC", "HALF", "13th Month Pay" });
                    }
                    else if (_thirteenthMonthMode == "October")
                    {
                        excelLines.Insert(0, new List<string> { "Employee Name", "Employee Code", "NOV", "DEC", "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEPT", "OCT", "HALF", "13th Month Pay" });
                    }

                    excelLines.Add((new QueryResult { ThirteenthMonthMode = _thirteenthMonthMode }).GetTotals(thirteenthMonthRecords));

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

                    reportFileNameBuilder.Append($"{query.PayrollPeriodYear}");

                    reportFileNameBuilder.Append(".xlsx");

                    return new QueryResult
                    {
                        FileContent = reportFileContent,
                        Filename = reportFileNameBuilder.ToString()
                    };
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
                        ThirteenthMonthMode = _thirteenthMonthMode,
                        ThirteenthMonthRecords = thirteenthMonthRecords,
                        PayrollPeriodYear = query.PayrollPeriodYear
                    };
                }
            }

            private IList<QueryResult.ThirteenthMonthRecord> GetThirteenthMonthRecords(IList<PayrollProcessBatch> payrollProcessBatches)
            {
                var thirteenthMonthRecordsDictionary = new Dictionary<int, QueryResult.ThirteenthMonthRecord>();

                for (var i = 1; i < 13; i++)
                {
                    var payrollRecordsInMonth = payrollProcessBatches
                        .Where(ppb => ppb.PayrollPeriodFrom.Value.Month == i)
                        .SelectMany(ppb => ppb.PayrollRecords)
                        .OrderBy(pr => pr.Employee.LastName)
                        .ThenBy(pr => pr.Employee.FirstName)
                        .ToList();

                    foreach (var payrollRecord in payrollRecordsInMonth)
                    {
                        if (!thirteenthMonthRecordsDictionary.ContainsKey(payrollRecord.EmployeeId.Value))
                        {
                            thirteenthMonthRecordsDictionary.Add(payrollRecord.EmployeeId.Value, new QueryResult.ThirteenthMonthRecord { Employee = payrollRecord.Employee, ThirteenthMonthMode = _thirteenthMonthMode });
                        }

                        if (!thirteenthMonthRecordsDictionary[payrollRecord.EmployeeId.Value].MonthRecords.ContainsKey(i))
                        {
                            thirteenthMonthRecordsDictionary[payrollRecord.EmployeeId.Value].MonthRecords.Add(i, new QueryResult.MonthRecord
                            {
                                Basic = payrollRecord.DaysWorkedValue.GetValueOrDefault() + payrollRecord.HoursWorkedValue.GetValueOrDefault(),
                                UTTardy = payrollRecord.HoursLateValue.GetValueOrDefault() + payrollRecord.HoursUndertimeValue.GetValueOrDefault()
                            });
                        }
                        else
                        {
                            thirteenthMonthRecordsDictionary[payrollRecord.EmployeeId.Value].MonthRecords[i].Basic += payrollRecord.DaysWorkedValue.GetValueOrDefault() + payrollRecord.HoursWorkedValue.GetValueOrDefault();
                            thirteenthMonthRecordsDictionary[payrollRecord.EmployeeId.Value].MonthRecords[i].UTTardy += payrollRecord.HoursLateValue.GetValueOrDefault() + payrollRecord.HoursUndertimeValue.GetValueOrDefault();
                        }
                    }
                }

                return thirteenthMonthRecordsDictionary.Select(d => d.Value).ToList();
            }
        }
    }
}
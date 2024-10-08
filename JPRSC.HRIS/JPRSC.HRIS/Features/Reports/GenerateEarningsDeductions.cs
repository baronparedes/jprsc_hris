﻿using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.Infrastructure.Excel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.Reports
{
    public class GenerateEarningsDeductions
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
            public IList<EarningsDeductionsRecord> EarningsDeductionsRecords { get; set; } = new List<EarningsDeductionsRecord>();
            public Query Query { get; set; }

            public IList<string> GetTotals(IList<EarningsDeductionsRecord> earningsDeductionsRecords)
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

                        totals.Add(String.Format("{0:n}", earningsDeductionsRecords.Where(r => r.MonthRecords.ContainsKey(key)).Select(r => r.MonthRecords[key]).Sum(mr => mr.Total)));
                    }
                }

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

            public class EarningsDeductionsRecord
            {
                public Employee Employee { get; set; }
                public IDictionary<ValueTuple<int, int>, MonthRecord> MonthRecords { get; set; } = new Dictionary<ValueTuple<int, int>, MonthRecord>(); // key is a month+year combination
                public Query Query { get; set; }
                public IList<IList<string>> DisplayLineCollection { get; private set; } = new List<IList<string>>();

                public void PopulateDisplayLineCollection(IList<Tuple<int, string>> distinctEarningsTypes, IList<Tuple<int, string>> distinctDeductionTypes)
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

                    var retVal = new List<IList<string>>();
                    retVal.Add(nameLine);

                    foreach (var earningType in distinctEarningsTypes)
                    {
                        var earningLine = new List<string> { String.Empty, earningType.Item2 };
                        var hasEarningType = false;
                        var earningFound = false;

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
                                    var monthRecord = MonthRecords[key];
                                    hasEarningType = monthRecord.EarningsValues.TryGetValue(earningType.Item1, out Tuple<string, decimal> earningValue);
                                    if (hasEarningType)
                                    {
                                        earningFound = true;
                                        earningLine.Add(String.Format("{0:n}", earningValue.Item2));
                                    }
                                    else
                                    {
                                        earningLine.Add(String.Empty);
                                    }
                                }
                                else
                                {
                                    earningLine.Add(String.Empty);
                                }
                            }
                        }

                        if (earningFound) retVal.Add(earningLine);
                    }

                    foreach (var deductionType in distinctDeductionTypes)
                    {
                        var deductionLine = new List<string> { String.Empty, deductionType.Item2 };
                        var hasDeductionType = false;
                        var deductionFound = false;

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
                                    var monthRecord = MonthRecords[key];
                                    hasDeductionType = monthRecord.DeductionsValues.TryGetValue(deductionType.Item1, out Tuple<string, decimal> earningValue);
                                    if (hasDeductionType)
                                    {
                                        deductionFound = true;
                                        deductionLine.Add(String.Format("{0:n}", earningValue.Item2));
                                    }
                                    else
                                    {
                                        deductionLine.Add(String.Empty);
                                    }
                                }
                                else
                                {
                                    deductionLine.Add(String.Empty);
                                }
                            }
                        }

                        if (deductionFound) retVal.Add(deductionLine);
                    }

                    var totalLine = new List<string> { String.Empty, "TOTAL" };
                    PopulateLine(totalLine, m => m.Total);

                    retVal.Add(totalLine);

                    DisplayLineCollection = retVal;
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
                public IDictionary<int, Tuple<string, decimal>> EarningsValues { get; set; } = new Dictionary<int, Tuple<string, decimal>>(); // key is earningdeductionid
                public IDictionary<int, Tuple<string, decimal>> DeductionsValues { get; set; } = new Dictionary<int, Tuple<string, decimal>>(); // key is earningdeductionid
                public decimal Total => EarningsValues.Sum(e => e.Value.Item2) - DeductionsValues.Sum(d => d.Value.Item2);
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
                var allPayrollProcessBatches = await GetPayrollProcessBatches(query, clientIds);

                var earningsDeductionsRecords = GetEarningsDeductionsRecords(query, allPayrollProcessBatches);

                List<Tuple<int, string>> distinctEarningTypes = GetDistinctEarningTypes(earningsDeductionsRecords);
                List<Tuple<int, string>> distinctDeductionTypes = GetDistinctDeductionTypes(earningsDeductionsRecords);

                foreach (var earningsDeductionsRecord in earningsDeductionsRecords)
                {
                    earningsDeductionsRecord.PopulateDisplayLineCollection(distinctEarningTypes, distinctDeductionTypes);
                }

                if (query.Destination == "Excel")
                {
                    var queryResult = new QueryResult
                    {
                        ClientId = query.ClientId,
                        DisplayMode = query.DisplayMode,
                        EarningsDeductionsRecords = earningsDeductionsRecords,
                        PayrollPeriodFromYear = query.PayrollPeriodFromYear,
                        PayrollPeriodToYear = query.PayrollPeriodToYear,
                        FromPayrollPeriodMonth = query.FromPayrollPeriodMonth,
                        FromPayrollPeriod = query.FromPayrollPeriod,
                        ToPayrollPeriodMonth = query.ToPayrollPeriodMonth,
                        ToPayrollPeriod = query.ToPayrollPeriod,
                        Query = query
                    };

                    var excelLines = new List<IList<string>>();

                    foreach (var earningsDeductionsRecord in earningsDeductionsRecords)
                    {
                        foreach (var displayLine in earningsDeductionsRecord.DisplayLineCollection)
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

                    excelLines.Insert(0, header);

                    var reportFileContent = _excelBuilder.BuildExcelFile(excelLines);

                    var reportFileNameBuilder = new StringBuilder(64)
                        .Append($"Earnings and Deductions Report - ")
                        .Append(query.ClientId == -1 ? "All Clients" : clients.Single().Name)
                        .Append(" - ")
                        .Append($"{query.PayrollPeriodFromYear} to {query.PayrollPeriodToYear}")
                        .Append(".xlsx");

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
                        EarningsDeductionsRecords = earningsDeductionsRecords,
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

            private static List<Tuple<int, string>> GetDistinctDeductionTypes(IList<QueryResult.EarningsDeductionsRecord> earningsDeductionsRecords)
            {
                var allDeductionTypes = earningsDeductionsRecords
                    .SelectMany(edr => edr.MonthRecords)
                    .Select(dict => dict.Value)
                    .SelectMany(mr => mr.DeductionsValues);

                var distinctDeductionTypes = new List<Tuple<int, string>>();
                foreach (var deductionType in allDeductionTypes)
                {
                    if (distinctDeductionTypes.Any(dt => dt.Item1 == deductionType.Key)) continue;

                    distinctDeductionTypes.Add(Tuple.Create(deductionType.Key, deductionType.Value.Item1));
                }

                return distinctDeductionTypes;
            }

            private static List<Tuple<int, string>> GetDistinctEarningTypes(IList<QueryResult.EarningsDeductionsRecord> earningsDeductionsRecords)
            {
                var allEarningTypes = earningsDeductionsRecords
                    .SelectMany(edr => edr.MonthRecords)
                    .Select(dict => dict.Value)
                    .SelectMany(mr => mr.EarningsValues);

                var distinctEarningTypes = new List<Tuple<int, string>>();
                foreach (var earningType in allEarningTypes)
                {
                    if (distinctEarningTypes.Any(et => et.Item1 == earningType.Key)) continue;

                    distinctEarningTypes.Add(Tuple.Create(earningType.Key, earningType.Value.Item1));
                }

                return distinctEarningTypes;
            }

            private async Task<List<PayrollProcessBatch>> GetPayrollProcessBatches(Query query, List<int> clientIds)
            {
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
                            .Include(ppb => ppb.EarningDeductionRecords)
                            .Include(ppb => ppb.EarningDeductionRecords.Select(edr => edr.EarningDeduction))
                            .Include(ppb => ppb.EarningDeductionRecords.Select(edr => edr.Employee))
                            .Include(ppb => ppb.EarningDeductionRecords.Select(edr => edr.Employee.Department))
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
                            .Include(ppb => ppb.EarningDeductionRecords)
                            .Include(ppb => ppb.EarningDeductionRecords.Select(edr => edr.EarningDeduction))
                            .Include(ppb => ppb.EarningDeductionRecords.Select(edr => edr.Employee))
                            .Include(ppb => ppb.EarningDeductionRecords.Select(edr => edr.Employee.Department))
                            .Where(ppb => !ppb.DeletedOn.HasValue &&
                                clientIds.Contains(ppb.ClientId.Value) &&
                                DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) >= startDate &&
                                DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) <= endDate &&
                                (int)ppb.PayrollPeriodMonth == fromPayrollPeriodMonthAsInt &&
                                ppb.PayrollPeriod >= query.FromPayrollPeriod)
                            .ToListAsync();

                        var payrollProcessBatchesInEndingMonth = await _db.PayrollProcessBatches
                            .AsNoTracking()
                            .Include(ppb => ppb.EarningDeductionRecords)
                            .Include(ppb => ppb.EarningDeductionRecords.Select(edr => edr.EarningDeduction))
                            .Include(ppb => ppb.EarningDeductionRecords.Select(edr => edr.Employee))
                            .Include(ppb => ppb.EarningDeductionRecords.Select(edr => edr.Employee.Department))
                            .Where(ppb => !ppb.DeletedOn.HasValue &&
                                clientIds.Contains(ppb.ClientId.Value) &&
                                DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) >= startDate &&
                                DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) <= endDate &&
                                (int)ppb.PayrollPeriodMonth == toPayrollPeriodMonthAsInt &&
                                ppb.PayrollPeriod <= query.ToPayrollPeriod)
                            .ToListAsync();

                        var payrollProcessBatchesInBetween = await _db.PayrollProcessBatches
                            .AsNoTracking()
                            .Include(ppb => ppb.EarningDeductionRecords)
                            .Include(ppb => ppb.EarningDeductionRecords.Select(edr => edr.EarningDeduction))
                            .Include(ppb => ppb.EarningDeductionRecords.Select(edr => edr.Employee))
                            .Include(ppb => ppb.EarningDeductionRecords.Select(edr => edr.Employee.Department))
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
                }

                allPayrollProcessBatches = allPayrollProcessBatches
                    .GroupBy(ppb => ppb.Id)
                    .Select(g => g.First())
                    .OrderBy(ppb => ppb.Id)
                    .ToList();

                return allPayrollProcessBatches;
            }

            private IList<QueryResult.EarningsDeductionsRecord> GetEarningsDeductionsRecords(Query query, IList<PayrollProcessBatch> payrollProcessBatches)
            {
                // Key is employee id
                var earningsDeductionsRecordsDictionary = new Dictionary<int, QueryResult.EarningsDeductionsRecord>();

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
                            var earningDeductionRecordsInMonth = group
                                .Where(ppb => ppb.PayrollPeriodFrom.Value.Year == i && (int)ppb.PayrollPeriodMonth / 10 == j)
                                .SelectMany(ppb => ppb.EarningDeductionRecords)
                                .ToList();

                            if (query.EmployeeId.HasValue && query.EmployeeId.Value > 0)
                            {
                                earningDeductionRecordsInMonth = earningDeductionRecordsInMonth
                                    .Where(edr => edr.EmployeeId == query.EmployeeId.Value)
                                    .ToList();
                            }

                            var isJanuaryFirstPayrollPeriod = j == 1 && group.Key == 1;
                            if (isJanuaryFirstPayrollPeriod)
                            {
                                var earningDeductionRecordsInJanuaryFirstPeriod = group
                                    .Where(ppb => ppb.PayrollPeriodFrom.Value.Year == i - 1 && (int)ppb.PayrollPeriodMonth / 10 == j)
                                    .SelectMany(ppb => ppb.EarningDeductionRecords)
                                    .ToList();

                                if (query.EmployeeId.HasValue && query.EmployeeId.Value > 0)
                                {
                                    earningDeductionRecordsInJanuaryFirstPeriod = earningDeductionRecordsInJanuaryFirstPeriod
                                        .Where(pr => pr.EmployeeId == query.EmployeeId.Value)
                                        .ToList();
                                }

                                if (earningDeductionRecordsInJanuaryFirstPeriod.Any())
                                {
                                    foreach (var record in earningDeductionRecordsInJanuaryFirstPeriod)
                                    {
                                        earningDeductionRecordsInMonth.Add(record);
                                    }
                                }
                            }

                            earningDeductionRecordsInMonth = earningDeductionRecordsInMonth
                                .OrderBy(edr => edr.Employee.LastName)
                                .ThenBy(edr => edr.Employee.FirstName)
                                .ToList();

                            AddEarningDeductionsToEarningsDeductionsRecordsDictionary(query, earningsDeductionsRecordsDictionary, i, j, group, earningDeductionRecordsInMonth);
                        }
                    }
                }

                return earningsDeductionsRecordsDictionary.Select(t => t.Value).OrderBy(t => t.Employee.LastName).ThenBy(t => t.Employee.FirstName).ToList();
            }

            private static void AddEarningDeductionsToEarningsDeductionsRecordsDictionary(Query query, Dictionary<int, QueryResult.EarningsDeductionsRecord> earningsDeductionsRecordsDictionary, int year, int month, IGrouping<int, PayrollProcessBatch> group, List<EarningDeductionRecord> earningDeductionsRecordsInMonth)
            {
                foreach (var earningDeductionRecord in earningDeductionsRecordsInMonth)
                {
                    if (!earningsDeductionsRecordsDictionary.ContainsKey(earningDeductionRecord.EmployeeId.Value))
                    {
                        earningsDeductionsRecordsDictionary.Add(earningDeductionRecord.EmployeeId.Value, new QueryResult.EarningsDeductionsRecord { Employee = earningDeductionRecord.Employee, Query = query });
                    }

                    var key = ValueTuple.Create(year, month);
                    if (!earningsDeductionsRecordsDictionary[earningDeductionRecord.EmployeeId.Value].MonthRecords.ContainsKey(key))
                    {
                        var newMonthRecord = new QueryResult.MonthRecord();
                        earningsDeductionsRecordsDictionary[earningDeductionRecord.EmployeeId.Value].MonthRecords.Add(key, newMonthRecord);
                    }

                    var monthRecord = earningsDeductionsRecordsDictionary[earningDeductionRecord.EmployeeId.Value].MonthRecords[key];
                    var earningDeductionId = earningDeductionRecord.EarningDeductionId.GetValueOrDefault();

                    if (earningDeductionRecord.EarningDeduction.EarningDeductionType.GetValueOrDefault() == EarningDeductionType.Earnings)
                    {
                        if (!monthRecord.EarningsValues.ContainsKey(earningDeductionId))
                        {
                            monthRecord.EarningsValues.Add(earningDeductionId, Tuple.Create(earningDeductionRecord.EarningDeduction.Code, earningDeductionRecord.Amount.GetValueOrDefault()));
                        }
                        else
                        {
                            var earningValue = monthRecord.EarningsValues[earningDeductionId];
                            monthRecord.EarningsValues[earningDeductionId] = Tuple.Create(earningValue.Item1, earningValue.Item2 + earningDeductionRecord.Amount.GetValueOrDefault());
                        }
                    }
                    else if (earningDeductionRecord.EarningDeduction.EarningDeductionType.GetValueOrDefault() == EarningDeductionType.Deductions)
                    {
                        if (!monthRecord.DeductionsValues.ContainsKey(earningDeductionId))
                        {
                            monthRecord.DeductionsValues.Add(earningDeductionId, Tuple.Create(earningDeductionRecord.EarningDeduction.Code, earningDeductionRecord.Amount.GetValueOrDefault()));
                        }
                        else
                        {
                            var deductionValue = monthRecord.DeductionsValues[earningDeductionId];
                            monthRecord.DeductionsValues[earningDeductionId] = Tuple.Create(deductionValue.Item1, deductionValue.Item2 + earningDeductionRecord.Amount.GetValueOrDefault());
                        }
                    }
                }
            }
        }
    }
}
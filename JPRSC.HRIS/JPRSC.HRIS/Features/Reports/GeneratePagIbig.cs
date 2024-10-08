﻿using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.Features.Payroll;
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
    public class GeneratePagIbig
    {
        public class Query : IRequest<QueryResult>
        {
            public int? ClientId { get; set; }
            public string Destination { get; set; }
            public string DisplayMode { get; set; }
            public int? PayrollPeriodMonth { get; set; }
            public int PayrollPeriodYear { get; set; }
        }

        public class QueryResult
        {
            public int? ClientId { get; set; }
            public string ClientName { get; set; }
            public string DisplayMode { get; set; }
            public byte[] FileContent { get; set; }
            public string Filename { get; set; }
            public int? PayrollPeriodMonth { get; set; }
            public Month? PayrollPeriodMonthMonth { get; set; }
            public int PayrollPeriodYear { get; set; }
            public IList<PagIbigRecord> PagIbigRecords { get; set; } = new List<PagIbigRecord>();

            public class PagIbigRecord
            {
                public string CompanyPagIbig { get; set; }
                public decimal PagIbigDeductionBasis { get; set; }
                public Employee Employee { get; set; }
                public decimal NetPayValue { get; set; }
                public decimal TotalPagIbigEmployee { get; set; }
                public decimal TotalPagIbigEmployer { get; set; }
                public decimal ShareTotal => TotalPagIbigEmployee + TotalPagIbigEmployer;

                public IList<string> DisplayLine
                {
                    get
                    {
                        var line = new List<string>();

                        line.Add(CompanyPagIbig);
                        line.Add(String.Empty);
                        line.Add(Employee.PagIbig);
                        line.Add(Employee.LastName);
                        line.Add(Employee.FirstName);
                        line.Add(Employee.DateOfBirth.HasValue ? String.Format("{0:M/d/yyyy}", Employee.DateOfBirth.Value) : null);
                        line.Add(String.Empty);
                        line.Add(String.IsNullOrWhiteSpace(Employee.MiddleName) ? null : Employee.MiddleName.Trim());
                        line.Add(String.Format("{0:n}", PagIbigDeductionBasis));
                        line.Add(String.Empty);
                        line.Add(String.Format("{0:M/d/yyyy}", DateTime.Now));
                        line.Add(String.Empty);
                        line.Add(String.Format("{0:n}", TotalPagIbigEmployer));
                        line.Add(String.Format("{0:n}", TotalPagIbigEmployee));
                        line.Add(String.Format("{0:n}", ShareTotal));

                        return line;
                    }
                }
            }
        }

        public class QueryHandler : IRequestHandler<Query, QueryResult>
        {
            private readonly ApplicationDbContext _db;
            private readonly IExcelBuilder _excelBuilder;
            private readonly IMediator _mediator;
            private Models.SystemSettings _systemSettings;

            public QueryHandler(ApplicationDbContext db, IExcelBuilder excelBuilder, IMediator mediator)
            {
                _db = db;
                _excelBuilder = excelBuilder;
                _mediator = mediator;
            }

            public async Task<QueryResult> Handle(Query query, CancellationToken token)
            {
                _systemSettings = await _db.SystemSettings.SingleAsync();

                var clients = query.ClientId == -1 ?
                    await _db.Clients.Where(c => !c.DeletedOn.HasValue).ToListAsync() :
                    await _db.Clients.Where(c => !c.DeletedOn.HasValue && c.Id == query.ClientId.Value).ToListAsync();

                var clientIds = clients.Select(c => c.Id).ToList();

                var payrollProcessBatches = await _db.PayrollProcessBatchesByMonthAndYear(clientIds, query.PayrollPeriodMonth, query.PayrollPeriodYear);

                var pagIbigRecords = await GetPagIbigRecords(payrollProcessBatches);

                if (query.Destination == "Excel")
                {
                    var excelLines = pagIbigRecords.Select(pr => pr.DisplayLine).ToList();
                    excelLines.Insert(0, new List<string> { "Company PagIbig No.", String.Empty, "Employee PagIbig No.", "Last Name", "First Name", String.Empty, String.Empty, "Middle Initial", "Net pay", String.Empty, "Date Generated", String.Empty, "PagIbig Employer Share", "PagIbig Employee Share", "Share Total" });
                    excelLines.Add(new List<string> { String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Format("{0:n}", pagIbigRecords.Sum(sr => sr.PagIbigDeductionBasis)), String.Empty, String.Empty, String.Empty, String.Format("{0:n}", pagIbigRecords.Sum(sr => sr.TotalPagIbigEmployer)), String.Format("{0:n}", pagIbigRecords.Sum(sr => sr.TotalPagIbigEmployee)), String.Format("{0:n}", pagIbigRecords.Sum(sr => sr.ShareTotal)) });

                    var reportFileContent = _excelBuilder.BuildExcelFile(excelLines);

                    var reportFileNameBuilder = new StringBuilder(64)
                        .Append($"PagIbig Report - ")
                        .Append(query.ClientId == -1 ? "All Clients" : clients.Single().Name)
                        .Append(" - ")
                        .Append(query.PayrollPeriodMonth == -1 ? "All Payroll Period Months" : $"{(Month)query.PayrollPeriodMonth.Value}")
                        .Append(".xlsx");

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

                    Month? payrollPeriodMonth = null;
                    if (query.PayrollPeriodMonth.HasValue && query.PayrollPeriodMonth.Value > 0)
                    {
                        payrollPeriodMonth = (Month)query.PayrollPeriodMonth.Value;
                    }

                    return new QueryResult
                    {
                        ClientId = query.ClientId,
                        ClientName = clientName,
                        DisplayMode = query.DisplayMode,
                        PagIbigRecords = pagIbigRecords,
                        PayrollPeriodMonth = query.PayrollPeriodMonth,
                        PayrollPeriodMonthMonth = payrollPeriodMonth,
                        PayrollPeriodYear = query.PayrollPeriodYear
                    };
                }                
            }
            
            private async Task<IList<QueryResult.PagIbigRecord>> GetPagIbigRecords(IList<PayrollProcessBatch> payrollProcessBatches)
            {
                var allPayrollRecords = payrollProcessBatches.SelectMany(ppb => ppb.PayrollRecords).ToList();
                var distinctCompanyIds = allPayrollRecords.Select(pr => pr.Employee).Select(e => e.CompanyId).Distinct();
                var companies = await _db.Companies.AsNoTracking().Where(c => !c.DeletedOn.HasValue && distinctCompanyIds.Contains(c.Id)).ToListAsync();
                var companiesDictionary = companies.ToDictionary(c => c.Id, c => c);

                var pagIbigRecords = new List<QueryResult.PagIbigRecord>();

                var payrollProcessBatchesPerMonth = payrollProcessBatches
                    .GroupBy(ppb => ppb.PayrollPeriodMonth)
                    .ToList();

                foreach (var batch in payrollProcessBatchesPerMonth)
                {
                    var payrollRecordsInBatchPerEmployee = batch
                        .SelectMany(ppb => ppb.PayrollRecords)
                        .GroupBy(pr => pr.EmployeeId)
                        .OrderBy(pr => pr.First().Employee.LastName)
                        .ThenBy(pr => pr.First().Employee.FirstName)
                        .ToList();

                    foreach (var employeePayrollRecords in payrollRecordsInBatchPerEmployee)
                    {
                        var deductionBasis = employeePayrollRecords.Sum(pr => pr.PagIbigDeductionBasis.GetValueOrDefault());
                        if (deductionBasis == 0) continue;

                        var sampleEmployee = employeePayrollRecords.First().Employee;

                        var pagIbigRecord = new QueryResult.PagIbigRecord();
                        var companyFound = companiesDictionary.TryGetValue(sampleEmployee.CompanyId.Value, out Company company);
                        if (!companyFound)
                        {
                            company = await _db.Companies.AsNoTracking().SingleOrDefaultAsync(c => c.Id == sampleEmployee.CompanyId);
                            companiesDictionary[sampleEmployee.CompanyId.Value] = company;
                        }

                        pagIbigRecord.CompanyPagIbig = company?.PagIbig;
                        pagIbigRecord.PagIbigDeductionBasis = deductionBasis;
                        pagIbigRecord.Employee = sampleEmployee;
                        pagIbigRecord.NetPayValue = employeePayrollRecords.Sum(pr => pr.NetPayValue);
                        pagIbigRecord.TotalPagIbigEmployee = employeePayrollRecords.Sum(pr => pr.PagIbigValueEmployee.GetValueOrDefault());
                        pagIbigRecord.TotalPagIbigEmployer = employeePayrollRecords.Sum(pr => pr.PagIbigValueEmployer.GetValueOrDefault());

                        pagIbigRecords.Add(pagIbigRecord);
                    }
                }

                return pagIbigRecords
                    .Where(pr => pr.TotalPagIbigEmployee > 0 || pr.TotalPagIbigEmployer > 0)
                    .OrderBy(pr => pr.Employee.LastName)
                    .ThenBy(pr => pr.Employee.FirstName)
                    .ToList();
            }
        }
    }
}
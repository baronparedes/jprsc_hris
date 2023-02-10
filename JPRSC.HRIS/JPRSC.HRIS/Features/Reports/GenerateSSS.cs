using JPRSC.HRIS.Infrastructure.Data;
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
    public class GenerateSSS
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
            public IList<SSSRecord> SSSRecords { get; set; } = new List<SSSRecord>();

            public class SSSRecord
            {
                public string CompanySSS { get; set; }
                public decimal SSSDeductionBasis { get; set; }
                public Employee Employee { get; set; }
                public decimal NetPayValue { get; set; }
                public decimal TotalSSSEmployee { get; set; }
                public decimal TotalSSSEmployer { get; set; }
                public decimal ShareTotal => TotalSSSEmployee + TotalSSSEmployer;

                public IList<string> DisplayLine
                {
                    get
                    {
                        var line = new List<string>();

                        line.Add(CompanySSS);
                        line.Add(String.Empty);
                        line.Add(Employee.SSS);
                        line.Add(Employee.LastName);
                        line.Add(Employee.FirstName);
                        line.Add(String.Empty);
                        line.Add(String.IsNullOrWhiteSpace(Employee.MiddleName) ? null : Employee.MiddleName.Trim().First().ToString());
                        line.Add(String.Format("{0:n}", SSSDeductionBasis));
                        line.Add(String.Empty);
                        line.Add(String.Format("{0:M/d/yyyy}", DateTime.Now));
                        line.Add(String.Empty);
                        line.Add(String.Format("{0:n}", TotalSSSEmployer));
                        line.Add(String.Format("{0:n}", TotalSSSEmployee));
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

                var sssRecords = await GetSSSRecords(payrollProcessBatches);

                if (query.Destination == "Excel")
                {
                    var excelLines = sssRecords.Select(pr => pr.DisplayLine).ToList();
                    excelLines.Insert(0, new List<string> { "Company SSS No.", String.Empty, "Employee SSS No.", "Last Name", "First Name", String.Empty, "Middle Initial", "Net pay", String.Empty, "Date Generated", String.Empty, "SSS Employer Share", "SSS Employee Share", "Share Total" });
                    excelLines.Add(new List<string> { String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Format("{0:n}", sssRecords.Sum(sr => sr.SSSDeductionBasis)), String.Empty, String.Empty, String.Empty, String.Format("{0:n}", sssRecords.Sum(sr => sr.TotalSSSEmployer)), String.Format("{0:n}", sssRecords.Sum(sr => sr.TotalSSSEmployee)), String.Format("{0:n}", sssRecords.Sum(sr => sr.ShareTotal)) });

                    var reportFileContent = _excelBuilder.BuildExcelFile(excelLines);

                    var reportFileNameBuilder = new StringBuilder(64);
                    reportFileNameBuilder.Append($"SSS Report - ");

                    if (query.ClientId == -1)
                    {
                        reportFileNameBuilder.Append("All Clients");
                    }
                    else
                    {
                        reportFileNameBuilder.Append(clients.Single().Name);
                    }

                    reportFileNameBuilder.Append(" - ");

                    if (query.PayrollPeriodMonth == -1)
                    {
                        reportFileNameBuilder.Append("All Payroll Period Months");
                    }
                    else
                    {
                        reportFileNameBuilder.Append($"{(Month)query.PayrollPeriodMonth.Value}");
                    }

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
                        SSSRecords = sssRecords,
                        PayrollPeriodMonth = query.PayrollPeriodMonth,
                        PayrollPeriodMonthMonth = payrollPeriodMonth,
                        PayrollPeriodYear = query.PayrollPeriodYear
                    };
                }                
            }
            
            private async Task<IList<QueryResult.SSSRecord>> GetSSSRecords(IList<PayrollProcessBatch> payrollProcessBatches)
            {
                var allPayrollRecords = payrollProcessBatches.SelectMany(ppb => ppb.PayrollRecords).ToList();
                var distinctCompanyIds = allPayrollRecords.Select(pr => pr.Employee).Select(e => e.CompanyId).Distinct();
                var companies = await _db.Companies.Where(c => !c.DeletedOn.HasValue && distinctCompanyIds.Contains(c.Id)).ToListAsync();

                var sssRecords = new List<QueryResult.SSSRecord>();

                var payrollProcessBatchesPerMonth = payrollProcessBatches
                    .GroupBy(ppb => ppb.PayrollPeriodMonth)
                    .ToList();

                var sssRanges = await _db.SSSRecords
                    .ToListAsync();

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
                        var sampleEmployee = employeePayrollRecords.First().Employee;

                        
                        


                        var sssRecord = new QueryResult.SSSRecord();
                        sssRecord.CompanySSS = companies.SingleOrDefault(c => c.Id == sampleEmployee.CompanyId)?.SSS;
                        sssRecord.SSSDeductionBasis = employeePayrollRecords.Sum(pr => pr.SSSDeductionBasis.GetValueOrDefault());
                        sssRecord.Employee = sampleEmployee;
                        sssRecord.NetPayValue = employeePayrollRecords.Sum(pr => pr.NetPayValue);
                        sssRecord.TotalSSSEmployee = employeePayrollRecords.Sum(pr => pr.SSSValueEmployee.GetValueOrDefault());
                        sssRecord.TotalSSSEmployer = employeePayrollRecords.Sum(pr => pr.SSSValueEmployer.GetValueOrDefault() + GetECC(sssRanges, pr.SSSDeductionBasis.GetValueOrDefault()));

                        sssRecords.Add(sssRecord);
                    }
                }

                return sssRecords
                    .Where(sr => sr.TotalSSSEmployee > 0 || sr.TotalSSSEmployer > 0)
                    .OrderBy(sr => sr.Employee.LastName)
                    .ThenBy(sr => sr.Employee.FirstName)
                    .ToList();
            }

            private decimal GetECC(IList<SSSRecord> sssRanges, decimal deductionBasis)
            {
                SSSRecord matchingRange = null;

                try
                {
                    matchingRange = sssRanges
                        .OrderBy(s => s.Range1)
                        .First(s => s.Range1 > deductionBasis);
                }
                catch
                {
                    throw new Exception($"Matching SSS range not found for amount {deductionBasis}");
                }

                return matchingRange.ECC.GetValueOrDefault();
            }
        }
    }
}
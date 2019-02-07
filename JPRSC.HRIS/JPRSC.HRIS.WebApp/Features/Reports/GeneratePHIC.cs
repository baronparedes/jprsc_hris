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
    public class GeneratePHIC
    {
        public class Query : IRequest<QueryResult>
        {
            public int? ClientId { get; set; }
            public string Destination { get; set; }
            public string DisplayMode { get; set; }
            public int? PayrollPeriodMonth { get; set; }
        }

        public class QueryResult
        {
            public int? ClientId { get; set; }
            public string ClientName { get; set; }
            public string DisplayMode { get; set; }
            public byte[] FileContent { get; set; }
            public string Filename { get; set; }
            public IList<IList<string>> Lines { get; set; } = new List<IList<string>>();
            public int? PayrollPeriodMonth { get; set; }
            public Month? PayrollPeriodMonthMonth { get; set; }
            public IList<PHICRecord> PHICRecords { get; set; } = new List<PHICRecord>();

            public class PHICRecord
            {
                public string CompanyPhilHealth { get; set; }
                public decimal PHICDeductionBasis { get; set; }
                public Employee Employee { get; set; }
                public decimal NetPayValue { get; set; }
                public decimal TotalPHICEmployee { get; set; }
                public decimal TotalPHICEmployer { get; set; }
                public decimal ShareTotal => TotalPHICEmployee + TotalPHICEmployer;

                public IList<string> DisplayLine
                {
                    get
                    {
                        var line = new List<string>();

                        line.Add(CompanyPhilHealth);
                        line.Add(String.Empty);
                        line.Add(Employee.PhilHealth);
                        line.Add(Employee.LastName);
                        line.Add(Employee.FirstName);
                        line.Add(String.Empty);
                        line.Add(String.IsNullOrWhiteSpace(Employee.MiddleName) ? null : Employee.MiddleName.Trim());
                        line.Add(String.Format("{0:n}", PHICDeductionBasis));
                        line.Add(String.Empty);
                        line.Add(String.Format("{0:M/d/yyyy}", DateTime.Now));
                        line.Add(String.Empty);
                        line.Add(String.Format("{0:n}", TotalPHICEmployer));
                        line.Add(String.Format("{0:n}", TotalPHICEmployee));
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

                var payrollProcessBatches = query.PayrollPeriodMonth == -1 ?
                    await _db.PayrollProcessBatches
                        .Include(ppb => ppb.PayrollRecords)
                        .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                        .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee.Department))
                        .Where(ppb => !ppb.DeletedOn.HasValue && clientIds.Contains(ppb.ClientId.Value))
                        .ToListAsync() :
                    await _db.PayrollProcessBatches
                        .Include(ppb => ppb.PayrollRecords)
                        .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                        .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee.Department))
                        .Where(ppb => !ppb.DeletedOn.HasValue && clientIds.Contains(ppb.ClientId.Value) && ppb.PayrollPeriodMonth.HasValue && (int)ppb.PayrollPeriodMonth == query.PayrollPeriodMonth)
                        .ToListAsync();

                var phicRecords = await GetPHICRecords(payrollProcessBatches);

                if (query.Destination == "Excel")
                {
                    var excelLines = phicRecords.Select(pr => pr.DisplayLine).ToList();
                    excelLines.Insert(0, new List<string> { "Company PHIC No.", String.Empty, "Employee PHIC No.", "Last Name", "First Name", String.Empty, "Middle Initial", "Net pay", String.Empty, "Date Generated", String.Empty, "PHIC Employer Share", "PHIC Employee Share", "Share Total" });
                    excelLines.Add(new List<string> { String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Format("{0:n}", phicRecords.Sum(sr => sr.PHICDeductionBasis)), String.Empty, String.Empty, String.Empty, String.Format("{0:n}", phicRecords.Sum(sr => sr.TotalPHICEmployer)), String.Format("{0:n}", phicRecords.Sum(sr => sr.TotalPHICEmployee)), String.Format("{0:n}", phicRecords.Sum(sr => sr.ShareTotal)) });

                    var reportFileContent = _excelBuilder.BuildExcelFile(excelLines);

                    var reportFileNameBuilder = new StringBuilder(64);
                    reportFileNameBuilder.Append($"PHIC Report - ");

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
                        PHICRecords = phicRecords,
                        PayrollPeriodMonth = query.PayrollPeriodMonth,
                        PayrollPeriodMonthMonth = payrollPeriodMonth
                    };
                }                
            }
            
            private async Task<IList<QueryResult.PHICRecord>> GetPHICRecords(IList<PayrollProcessBatch> payrollProcessBatches)
            {
                var allPayrollRecords = payrollProcessBatches.SelectMany(ppb => ppb.PayrollRecords).ToList();
                var distinctCompanyIds = allPayrollRecords.Select(pr => pr.Employee).Select(e => e.CompanyId).Distinct();
                var companies = await _db.Companies.Where(c => !c.DeletedOn.HasValue && distinctCompanyIds.Contains(c.Id)).ToListAsync();

                var phicRecords = new List<QueryResult.PHICRecord>();

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
                        var sampleEmployee = employeePayrollRecords.First().Employee;

                        var phicRecord = new QueryResult.PHICRecord();
                        phicRecord.CompanyPhilHealth = companies.SingleOrDefault(c => c.Id == sampleEmployee.CompanyId)?.PhilHealth;
                        phicRecord.PHICDeductionBasis = employeePayrollRecords.Sum(pr => pr.PHICDeductionBasis.GetValueOrDefault());
                        phicRecord.Employee = sampleEmployee;
                        phicRecord.NetPayValue = employeePayrollRecords.Sum(pr => pr.NetPayValue);
                        phicRecord.TotalPHICEmployee = employeePayrollRecords.Sum(pr => pr.PHICValueEmployee.GetValueOrDefault());
                        phicRecord.TotalPHICEmployer = employeePayrollRecords.Sum(pr => pr.PHICValueEmployer.GetValueOrDefault());

                        phicRecords.Add(phicRecord);
                    }
                }

                return phicRecords
                    .OrderBy(p => p.Employee.LastName)
                    .ThenBy(p => p.Employee.FirstName)
                    .ToList();
            }
        }
    }
}
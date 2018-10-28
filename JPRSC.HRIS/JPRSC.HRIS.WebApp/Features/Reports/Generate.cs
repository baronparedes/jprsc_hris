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
    public class Generate
    {
        public class Query : IRequest<QueryResult>
        {
            public int? ClientId { get; set; }
            public int? PayrollPeriodMonth { get; set; }
            public string ReportType { get; set; }
        }

        public class QueryResult
        {
            public byte[] FileContent { get; set; }
            public string Filename { get; set; }
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
                var clients = query.ClientId == -1 ?
                    await _db.Clients.Where(c => !c.DeletedOn.HasValue).ToListAsync() :
                    await _db.Clients.Where(c => !c.DeletedOn.HasValue && c.Id == query.ClientId.Value).ToListAsync();

                var clientIds = clients.Select(c => c.Id).ToList();

                var payrollProcessBatches = query.PayrollPeriodMonth == -1 ?
                    await _db.PayrollProcessBatches
                        .Include(ppb => ppb.PayrollRecords)
                        .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                        .Where(ppb => !ppb.DeletedOn.HasValue && clientIds.Contains(ppb.ClientId.Value))
                        .ToListAsync() :
                    await _db.PayrollProcessBatches
                        .Include(ppb => ppb.PayrollRecords)
                        .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                        .Where(ppb => !ppb.DeletedOn.HasValue && clientIds.Contains(ppb.ClientId.Value) && ppb.PayrollPeriodMonth.HasValue && (int)ppb.PayrollPeriodMonth == query.PayrollPeriodMonth)
                        .ToListAsync();

                _systemSettings = await _db.SystemSettings.SingleAsync();

                var lines = query.ReportType == "PHIC" ?
                    await GetPHICReportLines(payrollProcessBatches) :
                    await GetSSSReportLines(payrollProcessBatches);

                var reportFileContent = _excelBuilder.BuildExcelFile(lines);

                var reportFileNameBuilder = new StringBuilder(64);
                reportFileNameBuilder.Append($"{query.ReportType} Report - ");

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

            private async Task<IList<IList<string>>> GetPHICReportLines(IList<PayrollProcessBatch> payrollProcessBatches)
            {
                var allPayrollRecords = payrollProcessBatches.SelectMany(ppb => ppb.PayrollRecords).ToList();
                var distinctCompanyIds = allPayrollRecords.Select(pr => pr.Employee).Select(e => e.CompanyId).Distinct();
                var companies = await _db.Companies.Where(c => !c.DeletedOn.HasValue && distinctCompanyIds.Contains(c.Id)).ToListAsync();

                var lines = new List<IList<string>>();

                var header = new List<string> { "Company PHIC No.", String.Empty, "Employee PHIC No.", "Last Name", "First Name", String.Empty, "Middle Initial", "Net pay", String.Empty, "Date Generated", String.Empty, "PHIC Employer Share", "PHIC Employee Share" };
                lines.Add(header);

                var payrollProcessBatchesPerMonth = payrollProcessBatches
                    .GroupBy(ppb => ppb.PayrollPeriodMonth)
                    .ToList();

                foreach (var batch in payrollProcessBatchesPerMonth)
                {
                    var payrollRecordsInBatchPerEmployee = batch
                        .SelectMany(ppb => ppb.PayrollRecords)
                        .GroupBy(pr => pr.EmployeeId)
                        .ToList();

                    foreach (var employeePayrollRecords in payrollRecordsInBatchPerEmployee)
                    {
                        var line = new List<string>();

                        var netPayValue = NetPayHelper.GetNetPay(_systemSettings, employeePayrollRecords.Sum(pr => pr.BasicPayValue), employeePayrollRecords.Sum(pr => pr.TotalEarningsValue), employeePayrollRecords.Sum(pr => pr.TotalGovDeductionsValue), employeePayrollRecords.Sum(pr => pr.DeductionsValue.GetValueOrDefault()), employeePayrollRecords.Sum(pr => pr.LoanPaymentValue.GetValueOrDefault()));

                        var sampleEmployee = employeePayrollRecords.First().Employee;
                        
                        line.Add(companies.SingleOrDefault(c => c.Id == sampleEmployee.CompanyId)?.PhilHealth);
                        line.Add(String.Empty);
                        line.Add(sampleEmployee.PhilHealth);
                        line.Add(sampleEmployee.LastName);
                        line.Add(sampleEmployee.FirstName);
                        line.Add(String.Empty);
                        line.Add(String.IsNullOrWhiteSpace(sampleEmployee.MiddleName) ? null : sampleEmployee.MiddleName.Trim().First().ToString());
                        line.Add(String.Format("{0:n}", netPayValue));
                        line.Add(String.Empty);
                        line.Add(String.Format("{0:M/d/yyyy}", DateTime.Now));
                        line.Add(String.Empty);
                        line.Add(String.Format("{0:n}", employeePayrollRecords.Sum(pr => pr.PHICValueEmployer.GetValueOrDefault())));
                        line.Add(String.Format("{0:n}", employeePayrollRecords.Sum(pr => pr.PHICValueEmployee.GetValueOrDefault())));

                        lines.Add(line);
                    }
                }

                return lines;
            }

            private async Task<IList<IList<string>>> GetSSSReportLines(IList<PayrollProcessBatch> payrollProcessBatches)
            {
                var allPayrollRecords = payrollProcessBatches.SelectMany(ppb => ppb.PayrollRecords).ToList();
                var distinctCompanyIds = allPayrollRecords.Select(pr => pr.Employee).Select(e => e.CompanyId).Distinct();
                var companies = await _db.Companies.Where(c => !c.DeletedOn.HasValue && distinctCompanyIds.Contains(c.Id)).ToListAsync();

                var lines = new List<IList<string>>();

                var header = new List<string> { "Company SSS No.", String.Empty, "Employee SSS No.", "Last Name", "First Name", String.Empty, "Middle Initial", "Net pay", String.Empty, "Date Generated", String.Empty, "SSS Employer Share", "SSS Employee Share" };
                lines.Add(header);

                var payrollProcessBatchesPerMonth = payrollProcessBatches
                    .GroupBy(ppb => ppb.PayrollPeriodMonth)
                    .ToList();

                foreach (var batch in payrollProcessBatchesPerMonth)
                {
                    var payrollRecordsInBatchPerEmployee = batch
                        .SelectMany(ppb => ppb.PayrollRecords)
                        .GroupBy(pr => pr.EmployeeId)
                        .ToList();

                    foreach (var employeePayrollRecords in payrollRecordsInBatchPerEmployee)
                    {
                        var line = new List<string>();

                        var netPayValue = NetPayHelper.GetNetPay(_systemSettings, employeePayrollRecords.Sum(pr => pr.BasicPayValue), employeePayrollRecords.Sum(pr => pr.TotalEarningsValue), employeePayrollRecords.Sum(pr => pr.TotalGovDeductionsValue), employeePayrollRecords.Sum(pr => pr.DeductionsValue.GetValueOrDefault()), employeePayrollRecords.Sum(pr => pr.LoanPaymentValue.GetValueOrDefault()));

                        var sampleEmployee = employeePayrollRecords.First().Employee;

                        line.Add(companies.SingleOrDefault(c => c.Id == sampleEmployee.CompanyId)?.SSS);
                        line.Add(String.Empty);
                        line.Add(sampleEmployee.SSS);
                        line.Add(sampleEmployee.LastName);
                        line.Add(sampleEmployee.FirstName);
                        line.Add(String.Empty);
                        line.Add(String.IsNullOrWhiteSpace(sampleEmployee.MiddleName) ? null : sampleEmployee.MiddleName.Trim().First().ToString());
                        line.Add(String.Format("{0:n}", netPayValue));
                        line.Add(String.Empty);
                        line.Add(String.Format("{0:M/d/yyyy}", DateTime.Now));
                        line.Add(String.Empty);
                        line.Add(String.Format("{0:n}", employeePayrollRecords.Sum(pr => pr.SSSValueEmployer.GetValueOrDefault())));
                        line.Add(String.Format("{0:n}", employeePayrollRecords.Sum(pr => pr.SSSValueEmployee.GetValueOrDefault())));

                        lines.Add(line);
                    }
                }

                return lines;
            }
        }
    }
}
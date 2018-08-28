using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.WebApp.Infrastructure.Excel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Payroll
{
    public class ExportToExcel
    {
        public class Query : IRequest<QueryResult>
        {
            public string ReportType { get; set; }
            public int? PayrollProcessBatchId { get; set; }
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

            public QueryHandler(ApplicationDbContext db, IExcelBuilder excelBuilder, IMediator mediator)
            {
                _db = db;
                _excelBuilder = excelBuilder;
                _mediator = mediator;
            }

            public async Task<QueryResult> Handle(Query query, CancellationToken token)
            {
                byte[] fileContent = new byte[] { };
                var filename = String.Empty;

                if (query.ReportType == "Payslip")
                {
                    var payslipReportQuery = new PayslipReport.Query { PayrollProcessBatchId = query.PayrollProcessBatchId };
                    var payslipReportResult = await _mediator.Send(payslipReportQuery);

                    filename = $"Payslip report for {payslipReportResult.PayrollProcessBatchResult.Client.Name} {payslipReportResult.PayrollProcessBatchResult.PayrollPeriodFromFormatted} - {payslipReportResult.PayrollProcessBatchResult.PayrollPeriodToFormatted}.xlsx";

                    var columns = GetColumnsForPayslipReport(payslipReportResult.PayrollProcessBatchResult.DeductedSSS, payslipReportResult.PayrollProcessBatchResult.DeductedPagIbig, payslipReportResult.PayrollProcessBatchResult.DeductedPHIC, payslipReportResult.PayrollProcessBatchResult.DeductedTax);

                    var excelObject = payslipReportResult.PayslipRecords.ToExcelObject(columns);
                    fileContent = _excelBuilder.BuildExcelFile(excelObject);
                }
                else if (query.ReportType == "Payroll")
                {
                    var payrollReportQuery = new PayrollReport.Query { PayrollProcessBatchId = query.PayrollProcessBatchId };
                    var payrollReportResult = await _mediator.Send(payrollReportQuery);

                    filename = $"Payroll report for {payrollReportResult.PayrollProcessBatchResult.Client.Name} {payrollReportResult.PayrollProcessBatchResult.PayrollPeriodFromFormatted} - {payrollReportResult.PayrollProcessBatchResult.PayrollPeriodToFormatted}.xlsx";

                    IEnumerable<ColumnInfo<PayrollReport.QueryResult.PayrollRecord>> columns = GetColumnsForPayrollReport(payrollReportResult.PayrollProcessBatchResult.DeductedSSS, payrollReportResult.PayrollProcessBatchResult.DeductedPagIbig, payrollReportResult.PayrollProcessBatchResult.DeductedPHIC, payrollReportResult.PayrollProcessBatchResult.DeductedTax);

                    var excelObject = payrollReportResult.PayrollRecords.ToExcelObject(columns);

                    var lines = new List<IEnumerable<string>>();
                    lines.Add(new List<string> { "Job Placement Resources Services Cooperative" });
                    lines.Add(new List<string> { $"Payroll Report for {payrollReportResult.PayrollProcessBatchResult.Client.Name}" });
                    lines.Add(new List<string> { $"For the period {payrollReportResult.PayrollProcessBatchResult.PayrollPeriodFromFormatted} to {payrollReportResult.PayrollProcessBatchResult.PayrollPeriodToFormatted} ({payrollReportResult.PayrollProcessBatchResult.PayrollPeriodFormatted} payroll period)" });
                    lines.Add(new List<string> { $"{payrollReportResult.PayrollRecords.Count()} record/s total" });
                    lines.Add(new List<string> { Environment.NewLine });
                    lines.AddRange(excelObject.ToLines());

                    fileContent = _excelBuilder.BuildExcelFile(lines);
                }

                return new QueryResult
                {
                    FileContent = fileContent,
                    Filename = filename
                };
            }

            private IEnumerable<ColumnInfo<PayrollReport.QueryResult.PayrollRecord>> GetColumnsForPayrollReport(bool? deductedSSS, bool? deductedPagIbig, bool? deductedPHIC, bool? deductedTax)
            {
                var columns = new List<ColumnInfo<PayrollReport.QueryResult.PayrollRecord>>
                {
                    new ColumnInfo<PayrollReport.QueryResult.PayrollRecord>("Employee Code", "EmployeeCode", p => p.Employee.EmployeeCode),
                    new ColumnInfo<PayrollReport.QueryResult.PayrollRecord>("Employee", "Employee", p => String.Format("{0}, {1}", p.Employee.LastName, p.Employee.FirstName)),
                    new ColumnInfo<PayrollReport.QueryResult.PayrollRecord>("Regulary Pay", "RegularPay", p => String.Format("{0:n}", p.DaysWorkedValue.GetValueOrDefault() + p.HoursWorkedValue.GetValueOrDefault())),
                    new ColumnInfo<PayrollReport.QueryResult.PayrollRecord>("Overtime", "Overtime", p => String.Format("{0:n}", p.OvertimeValue.GetValueOrDefault())),
                    new ColumnInfo<PayrollReport.QueryResult.PayrollRecord>("UT/Tardy", "UT/Tardy", p => String.Format("{0:n}", p.HoursUndertimeValue.GetValueOrDefault() + p.HoursLateValue.GetValueOrDefault())),
                    new ColumnInfo<PayrollReport.QueryResult.PayrollRecord>("COLA", "COLA", p => String.Format("{0:n}", p.COLADailyValue.GetValueOrDefault() + p.COLAHourlyValue.GetValueOrDefault())),
                    new ColumnInfo<PayrollReport.QueryResult.PayrollRecord>("Earnings", "Earnings", p => String.Format("{0:n}", p.EarningsValue.GetValueOrDefault())),
                    new ColumnInfo<PayrollReport.QueryResult.PayrollRecord>("Total Earnings", "TotalEarnings", p => String.Format("{0:n}", p.TotalEarningsValue))
                };

                if (deductedSSS == true)
                {
                    columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollRecord>("SSS Prem", "SSS", p => String.Format("{0:n}", p.SSSValueEmployee.GetValueOrDefault())));
                }

                if (deductedPagIbig == true)
                {
                    columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollRecord>("Pag-Ibig", "PagIbig", p => String.Format("{0:n}", p.PagIbigValue.GetValueOrDefault())));
                }

                columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollRecord>("Deductions", "Deductions", p => String.Format("{0:n}", p.DeductionsValue.GetValueOrDefault())));
                columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollRecord>("Loan Payments", "LoanPayments", p => String.Format("{0:n}", p.LoanPaymentValue.GetValueOrDefault())));

                if (deductedPHIC == true)
                {
                    columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollRecord>("PHIC", "PHIC", p => String.Format("{0:n}", p.PHICValueEmployee.GetValueOrDefault())));
                }

                if (deductedTax == true)
                {
                    // Do nothing
                }

                columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollRecord>("Total Deductions", "TotalDeductions", p => String.Format("{0:n}", p.TotalDeductionsValue)));
                columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollRecord>("Net Pay", "NetPay", p => String.Format("{0:n}", p.NetPayValue)));

                return columns;
            }

            private IEnumerable<ColumnInfo<PayslipReport.QueryResult.PayslipData>> GetColumnsForPayslipReport(bool? deductedSSS, bool? deductedPagIbig, bool? deductedPHIC, bool? deductedTax)
            {
                var columns = new List<ColumnInfo<PayslipReport.QueryResult.PayslipData>>
                {
                    new ColumnInfo<PayslipReport.QueryResult.PayslipData>("Payroll Period From", "PayrollPeriodFrom", p => p.PayrollProcessBatchResult.PayrollPeriodFromFormatted),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipData>("Payroll Period To", "PayrollPeriodTo", p => p.PayrollProcessBatchResult.PayrollPeriodToFormatted),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipData>("Employee", "Employee", p => String.Format("{0}, {1} {2} ({3})", p.Employee.LastName, p.Employee.FirstName, p.Employee.MiddleName, p.Employee.EmployeeCode)),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipData>("Department", "Department", p => String.Format("{0}", p.Employee.Department?.Name)),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipData>("Basic pay", "BasicPay"),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipData>("Overtime", "Overtime"),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipData>("COLA", "COLA"),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipData>("OtherEarnings", "OtherEarnings"),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipData>("TotalEarnings", "TotalEarnings")
                };

                if (deductedSSS == true)
                {
                    columns.Add(new ColumnInfo<PayslipReport.QueryResult.PayslipData>("SSS", "SSS"));
                }

                if (deductedPagIbig == true)
                {
                    columns.Add(new ColumnInfo<PayslipReport.QueryResult.PayslipData>("PagIbig", "PagIbig"));
                }

                if (deductedPHIC == true)
                {
                    columns.Add(new ColumnInfo<PayslipReport.QueryResult.PayslipData>("PHIC", "PHIC"));
                }

                if (deductedTax == true)
                {
                    columns.Add(new ColumnInfo<PayslipReport.QueryResult.PayslipData>("Tax", "Tax"));
                }

                columns.AddRange(new List<ColumnInfo<PayslipReport.QueryResult.PayslipData>>
                {
                    new ColumnInfo<PayslipReport.QueryResult.PayslipData>("OtherDeductions", "OtherDeductions"),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipData>("LoanPayment", "LoanPayment"),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipData>("UTTardy", "UTTardy"),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipData>("TotalDeductions", "TotalDeductions"),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipData>("NetPay", "NetPay")
                });

                return columns;
            }
        }
    }
}
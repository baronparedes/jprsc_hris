using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.WebApp.Infrastructure.Excel;
using MediatR;
using System;
using System.Collections.Generic;
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
                var excelObject = new ExcelObject();
                var filename = String.Empty;

                if (query.ReportType == "Payslip")
                {
                    var payslipReportQuery = new PayslipReport.Query { PayrollProcessBatchId = query.PayrollProcessBatchId };
                    var payslipReportResult = await _mediator.Send(payslipReportQuery);

                    filename = $"Payslip report for {payslipReportResult.PayrollProcessBatchResult.Client.Name} {payslipReportResult.PayrollProcessBatchResult.PayrollPeriodFromFormatted} - {payslipReportResult.PayrollProcessBatchResult.PayrollPeriodToFormatted}.xlsx";

                    var columns = GetColumnsForPayslipReport(payslipReportResult.PayrollProcessBatchResult.DeductedSSS, payslipReportResult.PayrollProcessBatchResult.DeductedPagIbig, payslipReportResult.PayrollProcessBatchResult.DeductedPHIC, payslipReportResult.PayrollProcessBatchResult.DeductedTax);

                    excelObject = payslipReportResult.PayslipRecords.ToExcelObject(columns);
                }

                return new QueryResult
                {
                    FileContent = _excelBuilder.BuildExcelFile(excelObject),
                    Filename = filename
                };
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
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
                
                if (query.ReportType == "Bank")
                {
                    var bankReportQuery = new BankReport.Query { PayrollProcessBatchId = query.PayrollProcessBatchId };
                    var bankReportResult = await _mediator.Send(bankReportQuery);

                    filename = $"Bank report for {bankReportResult.PayrollProcessBatchResult.Client.Name} {bankReportResult.PayrollProcessBatchResult.PayrollPeriodFromFormatted} - {bankReportResult.PayrollProcessBatchResult.PayrollPeriodToFormatted}.xlsx";

                    var columns = GetColumnsForBankReport();

                    var excelObject = bankReportResult.PayrollRecords.ToExcelObject(columns);

                    var lines = new List<IEnumerable<string>>();
                    lines.Add(new List<string> { "Job Placement Resources Services Cooperative" });
                    lines.Add(new List<string> { $"Bank Report for {bankReportResult.PayrollProcessBatchResult.Client.Name}" });
                    lines.Add(new List<string> { $"For the period {bankReportResult.PayrollProcessBatchResult.PayrollPeriodFromFormatted} to {bankReportResult.PayrollProcessBatchResult.PayrollPeriodToFormatted} ({bankReportResult.PayrollProcessBatchResult.PayrollPeriodFormatted} bank period)" });
                    lines.Add(new List<string> { $"{bankReportResult.PayrollRecords.Count()} record/s total" });
                    lines.Add(new List<string> { Environment.NewLine });
                    lines.AddRange(excelObject.ToLines());
                    lines.Add(new List<string>
                    {
                        "Total:",
                        String.Format("{0:n}", bankReportResult.PayrollRecords.Sum(p => p.NetPayValue)),
                        String.Empty,
                        String.Empty
                    });

                    fileContent = _excelBuilder.BuildExcelFile(lines);
                }
                else if (query.ReportType == "CashHold")
                {
                    var cashHoldReportQuery = new CashHoldReport.Query { PayrollProcessBatchId = query.PayrollProcessBatchId };
                    var cashHoldReportResult = await _mediator.Send(cashHoldReportQuery);

                    filename = $"CashHold report for {cashHoldReportResult.PayrollProcessBatchResult.Client.Name} {cashHoldReportResult.PayrollProcessBatchResult.PayrollPeriodFromFormatted} - {cashHoldReportResult.PayrollProcessBatchResult.PayrollPeriodToFormatted}.xlsx";

                    var columns = GetColumnsForCashHoldReport();

                    var excelObject = cashHoldReportResult.PayrollRecords.ToExcelObject(columns);

                    var lines = new List<IEnumerable<string>>();
                    lines.Add(new List<string> { "Job Placement Resources Services Cooperative" });
                    lines.Add(new List<string> { $"Cash Hold Report for {cashHoldReportResult.PayrollProcessBatchResult.Client.Name}" });
                    lines.Add(new List<string> { $"For the period {cashHoldReportResult.PayrollProcessBatchResult.PayrollPeriodFromFormatted} to {cashHoldReportResult.PayrollProcessBatchResult.PayrollPeriodToFormatted} ({cashHoldReportResult.PayrollProcessBatchResult.PayrollPeriodFormatted} cashHold period)" });
                    lines.Add(new List<string> { $"{cashHoldReportResult.PayrollRecords.Count()} record/s total" });
                    lines.Add(new List<string> { Environment.NewLine });
                    lines.AddRange(excelObject.ToLines());
                    lines.Add(new List<string>
                    {
                        "Total:",
                        String.Format("{0:n}", cashHoldReportResult.PayrollRecords.Sum(p => p.NetPayValue)),
                        String.Empty,
                        String.Empty
                    });

                    fileContent = _excelBuilder.BuildExcelFile(lines);
                }
                else if (query.ReportType == "Cash")
                {
                    var cashReportQuery = new CashReport.Query { PayrollProcessBatchId = query.PayrollProcessBatchId };
                    var cashReportResult = await _mediator.Send(cashReportQuery);

                    filename = $"Cash report for {cashReportResult.PayrollProcessBatchResult.Client.Name} {cashReportResult.PayrollProcessBatchResult.PayrollPeriodFromFormatted} - {cashReportResult.PayrollProcessBatchResult.PayrollPeriodToFormatted}.xlsx";

                    var columns = GetColumnsForCashReport();

                    var excelObject = cashReportResult.PayrollRecords.ToExcelObject(columns);

                    var lines = new List<IEnumerable<string>>();
                    lines.Add(new List<string> { "Job Placement Resources Services Cooperative" });
                    lines.Add(new List<string> { $"Cash Report for {cashReportResult.PayrollProcessBatchResult.Client.Name}" });
                    lines.Add(new List<string> { $"For the period {cashReportResult.PayrollProcessBatchResult.PayrollPeriodFromFormatted} to {cashReportResult.PayrollProcessBatchResult.PayrollPeriodToFormatted} ({cashReportResult.PayrollProcessBatchResult.PayrollPeriodFormatted} cash period)" });
                    lines.Add(new List<string> { $"{cashReportResult.PayrollRecords.Count()} record/s total" });
                    lines.Add(new List<string> { Environment.NewLine });
                    lines.AddRange(excelObject.ToLines());
                    lines.Add(new List<string>
                    {
                        "Total:",
                        String.Format("{0:n}", cashReportResult.PayrollRecords.Sum(p => p.NetPayValue)),
                        String.Empty,
                        String.Empty
                    });

                    fileContent = _excelBuilder.BuildExcelFile(lines);
                }
                else if (query.ReportType == "Check")
                {
                    var checkReportQuery = new CheckReport.Query { PayrollProcessBatchId = query.PayrollProcessBatchId };
                    var checkReportResult = await _mediator.Send(checkReportQuery);

                    filename = $"Check report for {checkReportResult.PayrollProcessBatchResult.Client.Name} {checkReportResult.PayrollProcessBatchResult.PayrollPeriodFromFormatted} - {checkReportResult.PayrollProcessBatchResult.PayrollPeriodToFormatted}.xlsx";

                    var columns = GetColumnsForCheckReport();

                    var excelObject = checkReportResult.PayrollRecords.ToExcelObject(columns);

                    var lines = new List<IEnumerable<string>>();
                    lines.Add(new List<string> { "Job Placement Resources Services Cooperative" });
                    lines.Add(new List<string> { $"Check Report for {checkReportResult.PayrollProcessBatchResult.Client.Name}" });
                    lines.Add(new List<string> { $"For the period {checkReportResult.PayrollProcessBatchResult.PayrollPeriodFromFormatted} to {checkReportResult.PayrollProcessBatchResult.PayrollPeriodToFormatted} ({checkReportResult.PayrollProcessBatchResult.PayrollPeriodFormatted} check period)" });
                    lines.Add(new List<string> { $"{checkReportResult.PayrollRecords.Count()} record/s total" });
                    lines.Add(new List<string> { Environment.NewLine });
                    lines.AddRange(excelObject.ToLines());
                    lines.Add(new List<string>
                    {
                        "Total:",
                        String.Format("{0:n}", checkReportResult.PayrollRecords.Sum(p => p.NetPayValue)),
                        String.Empty,
                        String.Empty
                    });

                    fileContent = _excelBuilder.BuildExcelFile(lines);
                }
                else if (query.ReportType == "Hold")
                {
                    var holdReportQuery = new HoldReport.Query { PayrollProcessBatchId = query.PayrollProcessBatchId };
                    var holdReportResult = await _mediator.Send(holdReportQuery);

                    filename = $"Hold report for {holdReportResult.PayrollProcessBatchResult.Client.Name} {holdReportResult.PayrollProcessBatchResult.PayrollPeriodFromFormatted} - {holdReportResult.PayrollProcessBatchResult.PayrollPeriodToFormatted}.xlsx";

                    var columns = GetColumnsForHoldReport();

                    var excelObject = holdReportResult.PayrollRecords.ToExcelObject(columns);

                    var lines = new List<IEnumerable<string>>();
                    lines.Add(new List<string> { "Job Placement Resources Services Cooperative" });
                    lines.Add(new List<string> { $"Hold Report for {holdReportResult.PayrollProcessBatchResult.Client.Name}" });
                    lines.Add(new List<string> { $"For the period {holdReportResult.PayrollProcessBatchResult.PayrollPeriodFromFormatted} to {holdReportResult.PayrollProcessBatchResult.PayrollPeriodToFormatted} ({holdReportResult.PayrollProcessBatchResult.PayrollPeriodFormatted} hold period)" });
                    lines.Add(new List<string> { $"{holdReportResult.PayrollRecords.Count()} record/s total" });
                    lines.Add(new List<string> { Environment.NewLine });
                    lines.AddRange(excelObject.ToLines());
                    lines.Add(new List<string>
                    {
                        "Total:",
                        String.Format("{0:n}", holdReportResult.PayrollRecords.Sum(p => p.NetPayValue)),
                        String.Empty,
                        String.Empty
                    });

                    fileContent = _excelBuilder.BuildExcelFile(lines);
                }
                else if (query.ReportType == "Payroll")
                {
                    var payrollReportQuery = new PayrollReport.Query { PayrollProcessBatchId = query.PayrollProcessBatchId };
                    var payrollReportResult = await _mediator.Send(payrollReportQuery);

                    filename = $"Payroll report for {payrollReportResult.PayrollProcessBatchResult.Client.Name} {payrollReportResult.PayrollProcessBatchResult.PayrollPeriodFromFormatted} - {payrollReportResult.PayrollProcessBatchResult.PayrollPeriodToFormatted}.xlsx";

                    var columns = GetColumnsForPayrollReport(payrollReportResult.PayrollProcessBatchResult.DeductedSSS, payrollReportResult.PayrollProcessBatchResult.DeductedPagIbig, payrollReportResult.PayrollProcessBatchResult.DeductedPHIC, payrollReportResult.PayrollProcessBatchResult.DeductedTax);

                    var excelObject = payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).ToExcelObject(columns);

                    var lines = new List<IEnumerable<string>>();
                    lines.Add(new List<string> { "Job Placement Resources Services Cooperative" });
                    lines.Add(new List<string> { $"Payroll Report for {payrollReportResult.PayrollProcessBatchResult.Client.Name}" });
                    lines.Add(new List<string> { $"For the period {payrollReportResult.PayrollProcessBatchResult.PayrollPeriodFromFormatted} to {payrollReportResult.PayrollProcessBatchResult.PayrollPeriodToFormatted} ({payrollReportResult.PayrollProcessBatchResult.PayrollPeriodFormatted} payroll period)" });
                    lines.Add(new List<string> { $"{payrollReportResult.PayrollReportItems.Count()} record/s total" });
                    lines.Add(new List<string> { Environment.NewLine });
                    lines.AddRange(excelObject.ToLines());

                    var totals = new List<string>
                    {
                        String.Empty,
                        "Total:",
                        String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.DaysWorkedValue.GetValueOrDefault() + p.HoursWorkedValue.GetValueOrDefault())),
                        String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.OvertimeValue.GetValueOrDefault())),
                        String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.HoursUndertimeValue.GetValueOrDefault() + p.HoursLateValue.GetValueOrDefault())),
                        String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.COLADailyValue.GetValueOrDefault() + p.COLAHourlyValue.GetValueOrDefault())),
                        String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.EarningsValue.GetValueOrDefault())),
                        String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.TotalEarningsValue))
                    };

                    if (payrollReportResult.PayrollProcessBatchResult.DeductedSSS == true)
                    {
                        totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.SSSValueEmployee.GetValueOrDefault())));
                    }

                    if (payrollReportResult.PayrollProcessBatchResult.DeductedPagIbig == true)
                    {
                        totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.PagIbigValue.GetValueOrDefault())));
                    }

                    totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.DeductionsValue.GetValueOrDefault())));
                    totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.LoanPaymentValue.GetValueOrDefault())));

                    if (payrollReportResult.PayrollProcessBatchResult.DeductedPHIC == true)
                    {
                        totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.PHICValueEmployee.GetValueOrDefault())));
                    }

                    if (payrollReportResult.PayrollProcessBatchResult.DeductedTax == true)
                    {
                        // Do nothing
                    }

                    totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.TotalDeductionsValue)));
                    totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.NetPayValue)));

                    lines.Add(totals);

                    fileContent = _excelBuilder.BuildExcelFile(lines);
                }
                else if (query.ReportType == "Payslip")
                {
                    var payslipReportQuery = new PayslipReport.Query { PayrollProcessBatchId = query.PayrollProcessBatchId };
                    var payslipReportResult = await _mediator.Send(payslipReportQuery);

                    filename = $"Payslip report for {payslipReportResult.PayrollProcessBatchResult.Client.Name} {payslipReportResult.PayrollProcessBatchResult.PayrollPeriodFromFormatted} - {payslipReportResult.PayrollProcessBatchResult.PayrollPeriodToFormatted}.xlsx";

                    var columns = GetColumnsForPayslipReport(payslipReportResult.PayrollProcessBatchResult.DeductedSSS, payslipReportResult.PayrollProcessBatchResult.DeductedPagIbig, payslipReportResult.PayrollProcessBatchResult.DeductedPHIC, payslipReportResult.PayrollProcessBatchResult.DeductedTax);

                    var excelObject = payslipReportResult.PayslipRecords.ToExcelObject(columns);
                    fileContent = _excelBuilder.BuildExcelFile(excelObject);
                }

                return new QueryResult
                {
                    FileContent = fileContent,
                    Filename = filename
                };
            }

            private IEnumerable<ColumnInfo<HoldReport.QueryResult.PayrollRecord>> GetColumnsForHoldReport()
            {
                var columns = new List<ColumnInfo<HoldReport.QueryResult.PayrollRecord>>
                {
                    new ColumnInfo<HoldReport.QueryResult.PayrollRecord>("ATM Account #", "ATMAccountNumber", p => p.Employee.ATMAccountNumber),
                    new ColumnInfo<HoldReport.QueryResult.PayrollRecord>("Amount", "Amount", p => String.Format("{0:n}", p.NetPayValue)),
                    new ColumnInfo<HoldReport.QueryResult.PayrollRecord>("Last Name, First Name, MI", "Name", p => @String.Format("{0}, {1}, {2}", p.Employee.LastName, p.Employee.FirstName, !String.IsNullOrWhiteSpace(p.Employee.MiddleName) ? p.Employee.MiddleName.First().ToString() : null)),
                    new ColumnInfo<HoldReport.QueryResult.PayrollRecord>("Remarks", "Remarks", p => String.Empty)
                };

                return columns;
            }

            private IEnumerable<ColumnInfo<CheckReport.QueryResult.PayrollRecord>> GetColumnsForCheckReport()
            {
                var columns = new List<ColumnInfo<CheckReport.QueryResult.PayrollRecord>>
                {
                    new ColumnInfo<CheckReport.QueryResult.PayrollRecord>("ATM Account #", "ATMAccountNumber", p => p.Employee.ATMAccountNumber),
                    new ColumnInfo<CheckReport.QueryResult.PayrollRecord>("Amount", "Amount", p => String.Format("{0:n}", p.NetPayValue)),
                    new ColumnInfo<CheckReport.QueryResult.PayrollRecord>("Last Name, First Name, MI", "Name", p => @String.Format("{0}, {1}, {2}", p.Employee.LastName, p.Employee.FirstName, !String.IsNullOrWhiteSpace(p.Employee.MiddleName) ? p.Employee.MiddleName.First().ToString() : null)),
                    new ColumnInfo<CheckReport.QueryResult.PayrollRecord>("Remarks", "Remarks", p => String.Empty)
                };

                return columns;
            }

            private IEnumerable<ColumnInfo<CashReport.QueryResult.PayrollRecord>> GetColumnsForCashReport()
            {
                var columns = new List<ColumnInfo<CashReport.QueryResult.PayrollRecord>>
                {
                    new ColumnInfo<CashReport.QueryResult.PayrollRecord>("ATM Account #", "ATMAccountNumber", p => p.Employee.ATMAccountNumber),
                    new ColumnInfo<CashReport.QueryResult.PayrollRecord>("Amount", "Amount", p => String.Format("{0:n}", p.NetPayValue)),
                    new ColumnInfo<CashReport.QueryResult.PayrollRecord>("Last Name, First Name, MI", "Name", p => @String.Format("{0}, {1}, {2}", p.Employee.LastName, p.Employee.FirstName, !String.IsNullOrWhiteSpace(p.Employee.MiddleName) ? p.Employee.MiddleName.First().ToString() : null)),
                    new ColumnInfo<CashReport.QueryResult.PayrollRecord>("Remarks", "Remarks", p => String.Empty)
                };

                return columns;
            }

            private IEnumerable<ColumnInfo<CashHoldReport.QueryResult.PayrollRecord>> GetColumnsForCashHoldReport()
            {
                var columns = new List<ColumnInfo<CashHoldReport.QueryResult.PayrollRecord>>
                {
                    new ColumnInfo<CashHoldReport.QueryResult.PayrollRecord>("ATM Account #", "ATMAccountNumber", p => p.Employee.ATMAccountNumber),
                    new ColumnInfo<CashHoldReport.QueryResult.PayrollRecord>("Amount", "Amount", p => String.Format("{0:n}", p.NetPayValue)),
                    new ColumnInfo<CashHoldReport.QueryResult.PayrollRecord>("Last Name, First Name, MI", "Name", p => @String.Format("{0}, {1}, {2}", p.Employee.LastName, p.Employee.FirstName, !String.IsNullOrWhiteSpace(p.Employee.MiddleName) ? p.Employee.MiddleName.First().ToString() : null)),
                    new ColumnInfo<CashHoldReport.QueryResult.PayrollRecord>("Remarks", "Remarks", p => String.Empty)
                };

                return columns;
            }

            private IEnumerable<ColumnInfo<BankReport.QueryResult.PayrollRecord>> GetColumnsForBankReport()
            {
                var columns = new List<ColumnInfo<BankReport.QueryResult.PayrollRecord>>
                {
                    new ColumnInfo<BankReport.QueryResult.PayrollRecord>("ATM Account #", "ATMAccountNumber", p => p.Employee.ATMAccountNumber),
                    new ColumnInfo<BankReport.QueryResult.PayrollRecord>("Amount", "Amount", p => String.Format("{0:n}", p.NetPayValue)),
                    new ColumnInfo<BankReport.QueryResult.PayrollRecord>("Last Name, First Name, MI", "Name", p => @String.Format("{0}, {1}, {2}", p.Employee.LastName, p.Employee.FirstName, !String.IsNullOrWhiteSpace(p.Employee.MiddleName) ? p.Employee.MiddleName.First().ToString() : null)),
                    new ColumnInfo<BankReport.QueryResult.PayrollRecord>("Remarks", "Remarks", p => String.Empty)
                };

                return columns;
            }

            private IEnumerable<ColumnInfo<Models.PayrollRecord>> GetColumnsForPayrollReport(bool? deductedSSS, bool? deductedPagIbig, bool? deductedPHIC, bool? deductedTax)
            {
                var columns = new List<ColumnInfo<Models.PayrollRecord>>
                {
                    new ColumnInfo<Models.PayrollRecord>("Employee Code", "EmployeeCode", p => p.Employee.EmployeeCode),
                    new ColumnInfo<Models.PayrollRecord>("Employee", "Employee", p => String.Format("{0}, {1}", p.Employee.LastName, p.Employee.FirstName)),
                    new ColumnInfo<Models.PayrollRecord>("Regulary Pay", "RegularPay", p => String.Format("{0:n}", p.DaysWorkedValue.GetValueOrDefault() + p.HoursWorkedValue.GetValueOrDefault())),
                    new ColumnInfo<Models.PayrollRecord>("Overtime", "Overtime", p => String.Format("{0:n}", p.OvertimeValue.GetValueOrDefault())),
                    new ColumnInfo<Models.PayrollRecord>("UT/Tardy", "UT/Tardy", p => String.Format("{0:n}", p.HoursUndertimeValue.GetValueOrDefault() + p.HoursLateValue.GetValueOrDefault())),
                    new ColumnInfo<Models.PayrollRecord>("COLA", "COLA", p => String.Format("{0:n}", p.COLADailyValue.GetValueOrDefault() + p.COLAHourlyValue.GetValueOrDefault())),
                    new ColumnInfo<Models.PayrollRecord>("Earnings", "Earnings", p => String.Format("{0:n}", p.EarningsValue.GetValueOrDefault())),
                    new ColumnInfo<Models.PayrollRecord>("Total Earnings", "TotalEarnings", p => String.Format("{0:n}", p.TotalEarningsValue))
                };

                if (deductedSSS == true)
                {
                    columns.Add(new ColumnInfo<Models.PayrollRecord>("SSS Prem", "SSS", p => String.Format("{0:n}", p.SSSValueEmployee.GetValueOrDefault())));
                }

                if (deductedPagIbig == true)
                {
                    columns.Add(new ColumnInfo<Models.PayrollRecord>("Pag-Ibig", "PagIbig", p => String.Format("{0:n}", p.PagIbigValue.GetValueOrDefault())));
                }

                columns.Add(new ColumnInfo<Models.PayrollRecord>("Deductions", "Deductions", p => String.Format("{0:n}", p.DeductionsValue.GetValueOrDefault())));
                columns.Add(new ColumnInfo<Models.PayrollRecord>("Loan Payments", "LoanPayments", p => String.Format("{0:n}", p.LoanPaymentValue.GetValueOrDefault())));

                if (deductedPHIC == true)
                {
                    columns.Add(new ColumnInfo<Models.PayrollRecord>("PHIC", "PHIC", p => String.Format("{0:n}", p.PHICValueEmployee.GetValueOrDefault())));
                }

                if (deductedTax == true)
                {
                    // Do nothing
                }

                columns.Add(new ColumnInfo<Models.PayrollRecord>("Total Deductions", "TotalDeductions", p => String.Format("{0:n}", p.TotalDeductionsValue)));
                columns.Add(new ColumnInfo<Models.PayrollRecord>("Net Pay", "NetPay", p => String.Format("{0:n}", p.NetPayValue)));

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
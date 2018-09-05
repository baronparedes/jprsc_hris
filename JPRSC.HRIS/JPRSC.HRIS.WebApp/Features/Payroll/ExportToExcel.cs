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
            public bool? ViewDetailed { get; set; }
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

                    var columns = GetColumnsForPayrollReport(payrollReportResult.PayrollProcessBatchResult.DeductedSSS, payrollReportResult.PayrollProcessBatchResult.DeductedPagIbig, payrollReportResult.PayrollProcessBatchResult.DeductedPHIC, payrollReportResult.PayrollProcessBatchResult.DeductedTax, payrollReportResult, query.ViewDetailed);

                    var excelObject = payrollReportResult.PayrollReportItems.ToExcelObject(columns);

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
                        String.Empty,
                        String.Empty,
                        String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.OvertimeValue.GetValueOrDefault())),
                        String.Empty
                    };

                    if (query.ViewDetailed == true)
                    {
                        foreach (var payRate in payrollReportResult.PayRates)
                        {
                            if (payrollReportResult.PayrollReportItems.SelectMany(p => p.Overtimes.Where(ot => ot.PayPercentageId == payRate.Id)).Sum(ot => ot.NumberOfHoursValue.GetValueOrDefault()) > 0)
                            {
                                totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.SelectMany(p => p.Overtimes.Where(ot => ot.PayPercentageId == payRate.Id)).Sum(ot => ot.NumberOfHoursValue.GetValueOrDefault())));
                            }
                        }
                    }

                    totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.HoursUndertimeValue.GetValueOrDefault() + p.HoursLateValue.GetValueOrDefault())));
                    totals.Add(String.Empty);
                    totals.Add(String.Empty);
                    totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.COLADailyValue.GetValueOrDefault() + p.COLAHourlyValue.GetValueOrDefault())));
                    totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.EarningsValue.GetValueOrDefault())));
                    totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.TotalEarningsValue)));
                    totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.DeductionsValue.GetValueOrDefault())));
                    totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.TotalDeductionsValue)));

                    if (query.ViewDetailed == true)
                    {
                        foreach (var earningDeduction in payrollReportResult.EarningDeductions)
                        {
                            if (payrollReportResult.PayrollReportItems.SelectMany(p => p.EarningDeductionRecords.Where(edr => edr.EarningDeductionId == earningDeduction.Id)).Sum(edr => edr.Amount.GetValueOrDefault()) > 0)
                            {
                                totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.SelectMany(p => p.EarningDeductionRecords.Where(edr => edr.EarningDeductionId == earningDeduction.Id)).Sum(edr => edr.Amount.GetValueOrDefault())));
                            }
                        }
                    }

                    if (payrollReportResult.PayrollProcessBatchResult.DeductedSSS == true)
                    {
                        totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.SSSValueEmployee.GetValueOrDefault())));
                    }

                    if (payrollReportResult.PayrollProcessBatchResult.DeductedPagIbig == true)
                    {
                        totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.PagIbigValue.GetValueOrDefault())));
                    }

                    if (payrollReportResult.PayrollProcessBatchResult.DeductedPHIC == true)
                    {
                        totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.PHICValueEmployee.GetValueOrDefault())));
                    }

                    if (payrollReportResult.PayrollProcessBatchResult.DeductedTax == true)
                    {
                        // Do nothing
                    }

                    totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.Select(p => p.PayrollRecord).Sum(p => p.LoanPaymentValue.GetValueOrDefault())));
                    
                    foreach (var loanType in payrollReportResult.LoanTypes)
                    {
                        if (payrollReportResult.PayrollReportItems.SelectMany(p => p.Loans.Where(l => l.LoanTypeId == loanType.Id)).Sum(l => l.DeductionAmount.GetValueOrDefault()) > 0)
                        {
                            totals.Add(String.Format("{0:n}", payrollReportResult.PayrollReportItems.SelectMany(p => p.Loans.Where(l => l.LoanTypeId == loanType.Id)).Sum(l => l.DeductionAmount.GetValueOrDefault())));
                        }
                    }

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

            private IEnumerable<ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>> GetColumnsForPayrollReport(bool? deductedSSS, bool? deductedPagIbig, bool? deductedPHIC, bool? deductedTax, PayrollReport.QueryResult payrollReportResult, bool? viewDetailed)
            {
                var columns = new List<ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>>
                {
                    new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("Employee Code", "EmployeeCode", p => p.PayrollRecord.Employee.EmployeeCode),
                    new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("Employee", "Employee", p => String.Format("{0}, {1}", p.PayrollRecord.Employee.LastName, p.PayrollRecord.Employee.FirstName)),
                    new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("Regulary Pay", "RegularPay", p => String.Format("{0:n}", p.PayrollRecord.DaysWorkedValue.GetValueOrDefault() + p.PayrollRecord.HoursWorkedValue.GetValueOrDefault())),
                    new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("Days Worked", "DaysWorked", p => String.Format("{0:n}", p.DailyTimeRecord.DaysWorked?.ToString() ?? "0")),
                    new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("Hours Worked", "HoursWorked", p => String.Format("{0:n}", p.DailyTimeRecord.HoursWorked?.ToString() ?? "0")),
                    new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("Overtime", "Overtime", p => String.Format("{0:n}", p.PayrollRecord.OvertimeValue.GetValueOrDefault())),
                    new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("Overtime Worked", "OvertimeWorked", p => String.Format("{0:n}", p.Overtimes.Sum(ot => ot.NumberOfHours.GetValueOrDefault())))
                };

                if (viewDetailed == true)
                {
                    foreach (var payRate in payrollReportResult.PayRates)
                    {
                        if (payrollReportResult.PayrollReportItems.SelectMany(p => p.Overtimes.Where(ot => ot.PayPercentageId == payRate.Id)).Sum(ot => ot.NumberOfHoursValue.GetValueOrDefault()) > 0)
                        {
                            columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>($"{payRate.Code}", $"{payRate.Code}", p => String.Format("{0:n}", p.Overtimes.SingleOrDefault(edr => edr.PayPercentageId == payRate.Id)?.NumberOfHoursValue.GetValueOrDefault().ToString() ?? "0.00")));
                        }
                    }
                }

                columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("UT/Tardy", "UT/Tardy", p => String.Format("{0:n}", p.PayrollRecord.HoursUndertimeValue.GetValueOrDefault() + p.PayrollRecord.HoursLateValue.GetValueOrDefault())));
                columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("Undertime Minutes", "UndertimeMinutes", p => String.Format("{0:n}", p.DailyTimeRecord.HoursUndertime.GetValueOrDefault() * 60)));
                columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("Tardiness Minutes", "TardinessMinutes", p => String.Format("{0:n}", p.DailyTimeRecord.HoursLate.GetValueOrDefault() * 60)));
                columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("COLA", "COLA", p => String.Format("{0:n}", p.PayrollRecord.COLADailyValue.GetValueOrDefault() + p.PayrollRecord.COLAHourlyValue.GetValueOrDefault())));
                columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("Earnings", "Earnings", p => String.Format("{0:n}", p.PayrollRecord.EarningsValue.GetValueOrDefault())));
                columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("Total Earnings", "TotalEarnings", p => String.Format("{0:n}", p.PayrollRecord.TotalEarningsValue)));
                columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("Deductions", "Deductions", p => String.Format("{0:n}", p.PayrollRecord.DeductionsValue.GetValueOrDefault())));
                columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("Total Deductions", "TotalDeductions", p => String.Format("{0:n}", p.PayrollRecord.TotalDeductionsValue)));

                if (viewDetailed == true)
                {
                    foreach (var earningDeduction in payrollReportResult.EarningDeductions)
                    {
                        if (payrollReportResult.PayrollReportItems.SelectMany(p => p.EarningDeductionRecords.Where(edr => edr.EarningDeductionId == earningDeduction.Id)).Sum(edr => edr.Amount.GetValueOrDefault()) > 0)
                        {
                            columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>($"{earningDeduction.Code}", $"{earningDeduction.Code}", p => String.Format("{0:n}", p.EarningDeductionRecords.SingleOrDefault(edr => edr.EarningDeductionId == earningDeduction.Id)?.Amount.GetValueOrDefault().ToString() ?? "0.00")));
                        }
                    }
                }

                if (deductedSSS == true)
                {
                    columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("SSS Prem", "SSS", p => String.Format("{0:n}", p.PayrollRecord.SSSValueEmployee.GetValueOrDefault())));
                }

                if (deductedPagIbig == true)
                {
                    columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("Pag-Ibig", "PagIbig", p => String.Format("{0:n}", p.PayrollRecord.PagIbigValue.GetValueOrDefault())));
                }

                if (deductedPHIC == true)
                {
                    columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("PHIC", "PHIC", p => String.Format("{0:n}", p.PayrollRecord.PHICValueEmployee.GetValueOrDefault())));
                }

                if (deductedTax == true)
                {
                    // Do nothing
                }

                columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("Loan Payments", "LoanPayments", p => String.Format("{0:n}", p.PayrollRecord.LoanPaymentValue.GetValueOrDefault())));
                
                foreach (var loanType in payrollReportResult.LoanTypes)
                {
                    if (payrollReportResult.PayrollReportItems.SelectMany(p => p.Loans.Where(l => l.LoanTypeId == loanType.Id)).Sum(l => l.DeductionAmount.GetValueOrDefault()) > 0)
                    {
                        columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>($"{loanType.Code}", $"{loanType.Code}", p => String.Format("{0:n}", p.Loans.SingleOrDefault(lt => lt.LoanTypeId == loanType.Id)?.DeductionAmount.GetValueOrDefault().ToString() ?? "0.00")));
                    }
                }

                columns.Add(new ColumnInfo<PayrollReport.QueryResult.PayrollReportItem>("Net Pay", "NetPay", p => String.Format("{0:n}", p.PayrollRecord.NetPayValue)));

                return columns;
            }

            private IEnumerable<ColumnInfo<PayslipReport.QueryResult.PayslipRecord>> GetColumnsForPayslipReport(bool? deductedSSS, bool? deductedPagIbig, bool? deductedPHIC, bool? deductedTax)
            {
                var columns = new List<ColumnInfo<PayslipReport.QueryResult.PayslipRecord>>
                {
                    new ColumnInfo<PayslipReport.QueryResult.PayslipRecord>("Payroll Period From", "PayrollPeriodFrom", p => p.PayrollProcessBatchResult.PayrollPeriodFromFormatted),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipRecord>("Payroll Period To", "PayrollPeriodTo", p => p.PayrollProcessBatchResult.PayrollPeriodToFormatted),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipRecord>("Employee", "Employee", p => String.Format("{0}, {1} {2} ({3})", p.PayrollRecord.Employee.LastName, p.PayrollRecord.Employee.FirstName, p.PayrollRecord.Employee.MiddleName, p.PayrollRecord.Employee.EmployeeCode)),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipRecord>("Department", "Department", p => String.Format("{0}", p.PayrollRecord.Employee.Department?.Name)),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipRecord>("Basic pay", "BasicPay", p => String.Format("P{0:n}", p.PayrollRecord.DaysWorkedValue.GetValueOrDefault() + p.PayrollRecord.HoursWorkedValue.GetValueOrDefault())),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipRecord>("Overtime", "Overtime", p => String.Format("P{0:n}", p.PayrollRecord.OvertimeValue.GetValueOrDefault())),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipRecord>("COLA", "COLA", p => String.Format("P{0:n}", p.PayrollRecord.COLADailyValue.GetValueOrDefault() + p.PayrollRecord.COLAHourlyValue.GetValueOrDefault())),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipRecord>("OtherEarnings", "OtherEarnings", p => String.Format("P{0:n}", p.PayrollRecord.EarningsValue.GetValueOrDefault())),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipRecord>("TotalEarnings", "TotalEarnings", p => String.Format("P{0:n}", p.PayrollRecord.TotalEarningsValue))
                };

                if (deductedSSS == true)
                {
                    columns.Add(new ColumnInfo<PayslipReport.QueryResult.PayslipRecord>("SSS", "SSS", p => String.Format("P{0:n}", p.PayrollRecord.SSSValueEmployee.GetValueOrDefault())));
                }

                if (deductedPagIbig == true)
                {
                    columns.Add(new ColumnInfo<PayslipReport.QueryResult.PayslipRecord>("PagIbig", "PagIbig", p => String.Format("P{0:n}", p.PayrollRecord.PagIbigValue.GetValueOrDefault())));
                }

                if (deductedPHIC == true)
                {
                    columns.Add(new ColumnInfo<PayslipReport.QueryResult.PayslipRecord>("PHIC", "PHIC", p => String.Format("P{0:n}", p.PayrollRecord.PHICValueEmployee.GetValueOrDefault())));
                }

                if (deductedTax == true)
                {
                    columns.Add(new ColumnInfo<PayslipReport.QueryResult.PayslipRecord>("Tax", "Tax", p => String.Format("P{0:n}", p.PayrollRecord.TaxValue.GetValueOrDefault())));
                }

                columns.AddRange(new List<ColumnInfo<PayslipReport.QueryResult.PayslipRecord>>
                {
                    new ColumnInfo<PayslipReport.QueryResult.PayslipRecord>("OtherDeductions", "OtherDeductions", p => String.Format("P{0:n}", p.PayrollRecord.DeductionsValue.GetValueOrDefault())),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipRecord>("LoanPayment", "LoanPayment", p => String.Format("P{0:n}", p.PayrollRecord.LoanPaymentValue.GetValueOrDefault())),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipRecord>("UTTardy", "UTTardy", p => String.Format("P{0:n}", p.PayrollRecord.HoursUndertimeValue.GetValueOrDefault() + p.PayrollRecord.HoursLateValue.GetValueOrDefault())),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipRecord>("TotalDeductions", "TotalDeductions", p => String.Format("P{0:n}", p.PayrollRecord.TotalDeductionsValue)),
                    new ColumnInfo<PayslipReport.QueryResult.PayslipRecord>("NetPay", "NetPay", p => @String.Format("P{0:n}", p.PayrollRecord.NetPayValue))
                });

                return columns;
            }
        }
    }
}
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Infrastructure.NET;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.CSV;
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
    public class GenerateAlphalist
    {
        public const string BIRFormNumber = "1604CF";
        public const string CompanyTIN = "225628759";
        public const string U_0039 = "0039";
        public const string U_N = "N";
        public const string U_039 = "039";
        public const string U_0000 = "0000";

        public class Query : IRequest<QueryResult>
        {
            public string AlphalistType { get; set; }
            public int? ClientId { get; set; }
            public DateTime DateGenerated { get; set; }
            public string Destination { get; set; }
            public string DisplayMode { get; set; }
            public int PayrollPeriodFromYear { get; set; }
            public int PayrollPeriodToYear { get; set; }
            public Month? FromPayrollPeriodMonth { get; set; }
            public int? FromPayrollPeriod { get; set; }
            public Month? ToPayrollPeriodMonth { get; set; }
            public int? ToPayrollPeriod { get; set; }
            public int ThirteenthMonthPayrollPeriodFromYear { get; set; }
            public int ThirteenthMonthPayrollPeriodToYear { get; set; }
            public Month? ThirteenthMonthFromPayrollPeriodMonth { get; set; }
            public int? ThirteenthMonthFromPayrollPeriod { get; set; }
            public Month? ThirteenthMonthToPayrollPeriodMonth { get; set; }
            public int? ThirteenthMonthToPayrollPeriod { get; set; }
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
            public string AlphalistType { get; set; }
            public int? ClientId { get; set; }
            public string ClientName { get; set; }
            public int PayrollPeriodFromYear { get; set; }
            public int PayrollPeriodToYear { get; set; }
            public Month? FromPayrollPeriodMonth { get; set; }
            public int? FromPayrollPeriod { get; set; }
            public Month? ToPayrollPeriodMonth { get; set; }
            public int? ToPayrollPeriod { get; set; }
            public int ThirteenthMonthPayrollPeriodFromYear { get; set; }
            public int ThirteenthMonthPayrollPeriodToYear { get; set; }
            public Month? ThirteenthMonthFromPayrollPeriodMonth { get; set; }
            public int? ThirteenthMonthFromPayrollPeriod { get; set; }
            public Month? ThirteenthMonthToPayrollPeriodMonth { get; set; }
            public int? ThirteenthMonthToPayrollPeriod { get; set; }
            public string DisplayMode { get; set; }
            public byte[] FileContent { get; set; }
            public string Filename { get; set; }
            public IList<AlphalistRecord> AlphalistRecords { get; set; } = new List<AlphalistRecord>();
            public Query Query { get; set; }

            public IList<string> GetTotals(IList<AlphalistRecord> alphalistRecords)
            {
                var totals = new List<string> { String.Empty, String.Empty, String.Empty, String.Empty, "TOTAL" };
                totals.Add(String.Format("{0:n}", alphalistRecords.Sum(a => a.PRES_NT_TotalEarnings)));
                totals.Add(String.Empty); // Daily Rate
                totals.Add(String.Empty); // Monthly Rate
                totals.Add(String.Empty); // Yearly Rate
                totals.Add(String.Empty); // Number of weekdays in year
                totals.Add(String.Format("{0:n}", alphalistRecords.Sum(a => a.PRES_NT_TotalOvertimeValue)));
                totals.Add(String.Format("{0:n}", alphalistRecords.Sum(a => a.PRES_NT_TotalThirteenthMonth + a.PRES_NT_TotalEarnings)));
                totals.Add(String.Format("{0:n}", alphalistRecords.Sum(a => a.PRES_NT_TotalContributions)));
                totals.Add(String.Format("{0:n}", alphalistRecords.Sum(a => a.PRES_NT_TotalEarnings + a.PRES_NT_TotalOvertimeValue + a.PRES_NT_TotalThirteenthMonth + a.PRES_NT_TotalContributions)));

                return totals;
            }

            public IList<string> GetCSVHeader()
            {
                var displayLine = new List<string>();

                displayLine.Add(BIRFormNumber);
                displayLine.Add(CompanyTIN);
                displayLine.Add(U_0039);
                displayLine.Add(String.Format("MM/dd/yyyy", Query.DateGenerated));
                displayLine.Add(U_N);
                displayLine.Add("0");
                displayLine.Add(U_039);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);
                displayLine.Add(String.Empty);

                return displayLine;
            }

            public IList<string> GetPreHeaders()
            {
                return new List<string>
                {
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,

                    // Gross Compensation Income Previous Employer Non-Taxable
                    "Gross Compensation Income Previous Employer Non-Taxable",
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,

                    // Gross Compensation Income Previous Employer Taxable
                    "Taxable",
                    String.Empty,
                    String.Empty,

                    // Gross Compensation Income Present Employer Non-Taxable
                    "Gross Compensation Income Present Employer Non-Taxable",
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,

                    // Gross Compensation Income Present Employer Taxable
                    "Taxable",
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    "Exemption",
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    String.Empty,

                    // Gross Compensation Income Present Employer Tax Withheld
                    "Tax Withheld",
                    String.Empty,

                    // Gross Compensation Income Present Employer Year End Adjustments
                    "Year-End Adjustment (11a or 11b)",
                    String.Empty,
                    String.Empty
                };
            }

            public IList<string> GetHeaders()
            {
                return new List<string>
                {
                    "Employee Last Name",
                    "First Name",
                    "Middle Name",
                    "Employee Code",
                    "Employee TIN",
                    "Employee Region",
                    "Permanent Addrress",
                    "Birth Date",
                    "Contact Number",
                    "Zip Code",

                    // Gross Compensation Income Previous Employer Non-Taxable
                    "Gross compensation income previous",
                    "Regular Pay",
                    "Total Earnings",
                    "Basic/SMW",
                    "Holiday Pay",
                    "Overtime Pay",
                    "Night Shift Differential",
                    "Hazard Pay",
                    "13th month pay and other benefits",
                    "De minimis benefits",
                    "SSS, GSIS, PHIC and PAG-IBIG Contris and other union dues",
                    "Salaries and other forms of compensations",
                    "Total non-taxable/exempt compensation income",

                    // Gross Compensation Income Previous Employer Taxable
                    "13th month pay and other benefits",
                    "Salaries and other forms of compensation",
                    "Total taxable (previous employer)",

                    // Gross Compensation Income Present Employer Non-Taxable
                    "Date Hired",
                    "Date Resigned",
                    "Gross compensation present",
                    "Basic SMW per day",
                    "Basic SMW per month",
                    "Basic SMW per year",
                    "Factor used (no of days/year)",
                    "Holiday Pay",
                    "Overtime Pay",
                    "Night Shift Differential",
                    "Hazard Pay",
                    "13th month pay and other benefits",
                    "De minimis benefits",
                    "SSS, GSIS, PHIC and PAG-IBIG Contris and other union dues",
                    "Salaries and other forms of compensations",

                    // Gross Compensation Income Present Employer Taxable
                    "13th month pay and other benefits",
                    "Salaries and other forms of compensations",
                    "Total compensation present",
                    "Total compensation income (previous and present employers)",
                    "Exemption code",
                    "Amount",
                    "Premium paid on health and or hospital insurance",
                    "Net taxable compensation income",
                    "Tax due (Jan - Dec)",

                    // Gross Compensation Income Present Employer Tax Withheld
                    "Previous Employer",
                    "Present Employer",

                    // Gross Compensation Income Present Employer Year End Adjustments
                    "Amt withheld and paid for in December",
                    "Over withheld tax employee",
                    "Amount of tax withheld as adjusted"
                };
            }

            public class AlphalistRecord
            {
                public AlphalistRecord(DateTime dateGenerated)
                {
                    DateGenerated = dateGenerated;
                }

                public DateTime DateGenerated { get; set; }
                public int Order { get; set; }

                public Employee Employee { get; set; }
                public Query Query { get; set; }

                // Gross Compensation Income Previous Employer Non-Taxable
                public string PREV_NT__GrossCompensationIncomePrevious { get; set; } = "NA";
                public string PREV_NT_BasicSMW { get; set; } = "NA";
                public string PREV_NT_HolidayPay { get; set; } = "NA";
                public string PREV_NT_OvertimePay { get; set; } = "NA";
                public string PREV_NT_NightShiftDifferential { get; set; } = "NA";
                public string PREV_NT_HazardPay { get; set; } = "NA";
                public string PREV_NT_ThirteenthMonthPayAndOtherBenefits { get; set; } = "NA";
                public string PREV_NT_DeMinimisBenefits { get; set; } = "NA";
                public string PREV_NT_Contributions { get; set; } = "NA";
                public string PREV_NT_SalariesAndOtherFormsOfCompensations { get; set; } = "NA";
                public string PREV_NT_TotalIncome { get; set; } = "NA";

                // Gross Compensation Income Previous Employer Taxable
                public string PREV_T_ThirteenthMonthPayAndOtherBenefits { get; set; } = "NA";
                public string PREV_T_SalariesAndOtherFormsOfCompensations { get; set; } = "NA";
                public string PREV_T_TotalIncome { get; set; } = "NA";

                // Gross Compensation Income Present Employer Non-Taxable
                public decimal TotalHoursWorkedValue { get; set; }
                public decimal TotalDaysWorkedValue { get; set; }
                public decimal PRES_NT_TotalOvertimeValue { get; set; }
                public decimal TotalCOLAHourlyValue { get; set; }
                public decimal TotalCOLADailyValue { get; set; }
                public decimal TotalCOLAMonthlyValue { get; set; }
                public decimal TotalEarningsValue { get; set; }
                public decimal PRES_NT_TotalEarnings => TotalDaysWorkedValue + TotalHoursWorkedValue + TotalCOLAHourlyValue + TotalCOLADailyValue + TotalCOLAMonthlyValue + TotalEarningsValue + PRES_NT_TotalOvertimeValue;
                public decimal PRES_NT_RegularPay => TotalDaysWorkedValue + TotalHoursWorkedValue + TotalCOLAHourlyValue + TotalCOLADailyValue + TotalCOLAMonthlyValue;
                public decimal PRES_NT_TotalThirteenthMonth { get; set; }
                public int NumberOfWeekdaysInToYear { get; set; }
                public decimal TotalSSSValue { get; set; }
                public decimal TotalPHICValue { get; set; }
                public decimal TotalPagIbigValue { get; set; }
                public decimal PRES_NT_TotalContributions => TotalSSSValue + TotalPHICValue + TotalPagIbigValue;
                public string PRES_NT_HolidayPay { get; set; } = "NA";
                public string PRES_NT_NightShiftDifferential { get; set; } = "NA";
                public string PRES_NT_HazardPay { get; set; } = "NA";
                public string PRES_NT_DeMinimisBenefits { get; set; } = "NA";

                // Gross Compensation Income Present Employer Taxable
                public string PRES_T_ThirteenthMonthPayAndOtherBenefits { get; set; } = "NA";
                public string PRES_T_SalariesAndOtherFormsOfCompensations { get; set; } = "NA";
                public string PRES_T_TotalCompensationPresent { get; set; } = "NA";
                public string PRES_T_TotalCompensationIncome { get; set; } = "NA";
                public string PRES_T_ExemptionCode { get; set; } = "NA";
                public string PRES_T_Amount { get; set; } = "NA";
                public string PRES_T_PremiumPaidHospital { get; set; } = "NA";
                public string PRES_T_NetTaxableCompensationIncome { get; set; } = "NA";
                public string PRES_T_TaxDue { get; set; } = "NA";

                // Gross Compensation Income Present Employer Tax Withheld
                public string PRES_TW_PreviousEmployer { get; set; } = "NA";
                public string PRES_TW_PresentEmployer { get; set; } = "NA";

                // Gross Compensation Income Present Employer Year End Adjustments
                public string PRES_YEA_AmountWithheldPaidDecember { get; set; } = "NA";
                public string PRES_YEA_OverWithheldTax { get; set; } = "NA";
                public string PRES_YEA_AmountWithheldRequested { get; set; } = "NA";

                public IList<string> CSVDisplayLine
                {
                    get
                    {
                        var dateHired = Employee.DateHired.HasValue ? $"{Employee.DateHired.Value:MM/dd/yyyy}" : String.Empty;
                        var dateResigned = Employee.DateResigned.HasValue ? $"{Employee.DateResigned.Value:MM/dd/yyyy}" : String.Empty;

                        var displayLine = new List<string>();

                        displayLine.Add("D" + Query.AlphalistType);
                        displayLine.Add(BIRFormNumber);
                        displayLine.Add(CompanyTIN);
                        displayLine.Add(U_0039);
                        displayLine.Add(String.Format("{0:MM/dd/yyyy}", Query.DateGenerated));
                        displayLine.Add(Order.ToString());
                        displayLine.Add(Employee.TIN);
                        displayLine.Add(U_0000);
                        displayLine.Add($"\"{Employee.LastName}\"");
                        displayLine.Add($"\"{Employee.FirstName}\"");
                        displayLine.Add($"\"{String.Format("{0}", String.IsNullOrWhiteSpace(Employee.MiddleName) ? String.Empty : $", {Employee.MiddleName}")}\"");
                        displayLine.Add(dateHired);
                        displayLine.Add(dateResigned);
                        displayLine.Add(String.Format("{0:n}", PRES_NT_TotalEarnings));
                        displayLine.Add(String.Format("{0:n}", PRES_NT_TotalThirteenthMonth));
                        displayLine.Add(String.Format("{0:n}", PRES_NT_TotalContributions));
                        displayLine.Add(String.Format("{0:n}", PRES_NT_TotalEarnings - PRES_NT_TotalOvertimeValue - PRES_NT_TotalThirteenthMonth - PRES_NT_TotalContributions));
                        displayLine.Add(String.Format("{0:n}", PRES_NT_TotalEarnings - PRES_NT_TotalOvertimeValue - PRES_NT_TotalThirteenthMonth - PRES_NT_TotalContributions));
                        displayLine.Add("N/A");
                        displayLine.Add("N/A");
                        displayLine.Add("N/A");
                        displayLine.Add("N/A");
                        displayLine.Add("N/A");
                        displayLine.Add("N/A");
                        displayLine.Add("N/A");
                        displayLine.Add("N/A");
                        displayLine.Add("N/A");
                        displayLine.Add("N/A");
                        displayLine.Add("N/A");
                        displayLine.Add("Y");

                        return displayLine;
                    }
                }

                public IList<string> DisplayLine
                {
                    get
                    {
                        var dateHired = Employee.DateHired.HasValue ? $"{Employee.DateHired.Value:MM/dd/yyyy}" : String.Empty;
                        var dateResigned = Employee.DateResigned.HasValue ? $"{Employee.DateResigned.Value:MM/dd/yyyy}" : "present";
                        var employmentDuration = Employee.DateHired.HasValue && Employee.DateResigned.HasValue ? $"{dateHired} - {dateResigned}" :
                            Employee.DateHired.HasValue && !Employee.DateResigned.HasValue ? $"{dateHired} - present" :
                            String.Empty;
                        var yearlyRate = Employee.MonthlyRate.HasValue ? Employee.MonthlyRate.Value * 12 : (decimal?)null;

                        var displayLine = new List<string>();
                        displayLine.Add(Employee.LastName);
                        displayLine.Add(Employee.FirstName);
                        displayLine.Add(String.Format("{0}", String.IsNullOrWhiteSpace(Employee.MiddleName) ? null : $"{Employee.MiddleName}"));
                        displayLine.Add(Employee.EmployeeCode);
                        displayLine.Add(Employee.TIN);                       
                        displayLine.Add(Employee.Region.HasValue ? EnumHelper.GetDisplayName(Employee.Region.Value) : String.Empty);

                        displayLine.Add(Employee.PermanentAddress);
                        displayLine.Add(Employee.DateOfBirth.HasValue ? $"{Employee.DateOfBirth.Value:MM/dd/yyyy}" : String.Empty);
                        displayLine.Add(Employee.CelNo);
                        displayLine.Add(Employee.ZipCode);

                        // Gross Compensation Income Previous Employer Non-Taxable
                        displayLine.Add(PREV_NT__GrossCompensationIncomePrevious);
                        displayLine.Add(PREV_NT_BasicSMW);
                        displayLine.Add(PREV_NT_HolidayPay);
                        displayLine.Add(PREV_NT_OvertimePay);
                        displayLine.Add(PREV_NT_NightShiftDifferential);
                        displayLine.Add(PREV_NT_HazardPay);
                        displayLine.Add(PREV_NT_ThirteenthMonthPayAndOtherBenefits);
                        displayLine.Add(PREV_NT_DeMinimisBenefits);
                        displayLine.Add(PREV_NT_Contributions);
                        displayLine.Add(PREV_NT_SalariesAndOtherFormsOfCompensations);
                        displayLine.Add(PREV_NT_TotalIncome);

                        // Gross Compensation Income Previous Employer Taxable
                        displayLine.Add(PREV_T_ThirteenthMonthPayAndOtherBenefits);
                        displayLine.Add(PREV_T_SalariesAndOtherFormsOfCompensations);
                        displayLine.Add(PREV_T_TotalIncome);

                        // Gross Compensation Income Present Employer Non-Taxable
                        displayLine.Add(dateHired);
                        displayLine.Add(dateResigned);
                        displayLine.Add(String.Format("{0:n}", PRES_NT_TotalEarnings));
                        displayLine.Add(String.Format("{0:n}", PRES_NT_RegularPay));
                        displayLine.Add(String.Format("{0:n}", TotalEarningsValue));
                        displayLine.Add(Employee.DailyRate.HasValue ? String.Format("{0:n}", Employee.DailyRate) : String.Empty);
                        displayLine.Add(Employee.MonthlyRate.HasValue ? String.Format("{0:n}", Employee.MonthlyRate) : String.Empty);
                        displayLine.Add(yearlyRate.HasValue ? String.Format("{0:n}", yearlyRate) : String.Empty);
                        displayLine.Add(NumberOfWeekdaysInToYear.ToString());
                        displayLine.Add(PRES_NT_HolidayPay);
                        displayLine.Add(String.Format("{0:n}", PRES_NT_TotalOvertimeValue));
                        displayLine.Add(PRES_NT_NightShiftDifferential);
                        displayLine.Add(PRES_NT_HazardPay);
                        displayLine.Add(String.Format("{0:n}", PRES_NT_TotalThirteenthMonth));
                        displayLine.Add(PRES_NT_DeMinimisBenefits);
                        displayLine.Add(String.Format("{0:n}", PRES_NT_TotalContributions));
                        displayLine.Add(String.Format("{0:n}", PRES_NT_TotalEarnings - PRES_NT_TotalOvertimeValue - PRES_NT_TotalThirteenthMonth - PRES_NT_TotalContributions));

                        // Gross Compensation Income Present Employer Taxable
                        displayLine.Add(PRES_T_ThirteenthMonthPayAndOtherBenefits);
                        displayLine.Add(PRES_T_SalariesAndOtherFormsOfCompensations);
                        displayLine.Add(PRES_T_TotalCompensationPresent);
                        displayLine.Add(PRES_T_TotalCompensationIncome);
                        displayLine.Add(PRES_T_ExemptionCode);
                        displayLine.Add(PRES_T_Amount);
                        displayLine.Add(PRES_T_PremiumPaidHospital);
                        displayLine.Add(PRES_T_NetTaxableCompensationIncome);
                        displayLine.Add(PRES_T_TaxDue);

                        // Gross Compensation Income Present Employer Tax Withheld
                        displayLine.Add(PRES_TW_PreviousEmployer);
                        displayLine.Add(PRES_TW_PresentEmployer);

                        // Gross Compensation Income Present Employer Year End Adjustments
                        displayLine.Add(PRES_YEA_AmountWithheldPaidDecember);
                        displayLine.Add(PRES_YEA_OverWithheldTax);
                        displayLine.Add(PRES_YEA_AmountWithheldRequested);

                        return displayLine;
                    }
                }
            }
        }

        public class QueryHandler : IRequestHandler<Query, QueryResult>
        {
            private readonly ApplicationDbContext _db;
            private readonly ICSVBuilder _csvBuilder;
            private readonly IExcelBuilder _excelBuilder;
            private readonly IMediator _mediator;

            public QueryHandler(ApplicationDbContext db, ICSVBuilder csvBuilder, IExcelBuilder excelBuilder, IMediator mediator)
            {
                _db = db;
                _csvBuilder = csvBuilder;
                _excelBuilder = excelBuilder;
                _mediator = mediator;

                _db.Database.CommandTimeout = AppSettings.Int("CommandTimeout_GenerateAlphalist");
            }

            public async Task<QueryResult> Handle(Query query, CancellationToken token)
            {
                query.DateGenerated = DateTime.Now;

                var clients = query.ClientId == -1 ?
                    await _db.Clients.AsNoTracking().Where(c => !c.DeletedOn.HasValue).ToListAsync() :
                    await _db.Clients.AsNoTracking().Where(c => !c.DeletedOn.HasValue && c.Id == query.ClientId.Value).ToListAsync();

                var clientIds = clients.Select(c => c.Id).ToList();

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
                            .Include(ppb => ppb.PayrollRecords)
                            .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                            .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee.Department))
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
                            .Include(ppb => ppb.PayrollRecords)
                            .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                            .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee.Department))
                            .Where(ppb => !ppb.DeletedOn.HasValue &&
                                clientIds.Contains(ppb.ClientId.Value) &&
                                DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) >= startDate &&
                                DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) <= endDate &&
                                (int)ppb.PayrollPeriodMonth == fromPayrollPeriodMonthAsInt &&
                                ppb.PayrollPeriod >= query.FromPayrollPeriod)
                            .ToListAsync();

                        var payrollProcessBatchesInEndingMonth = await _db.PayrollProcessBatches
                            .AsNoTracking()
                            .Include(ppb => ppb.PayrollRecords)
                            .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                            .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee.Department))
                            .Where(ppb => !ppb.DeletedOn.HasValue &&
                                clientIds.Contains(ppb.ClientId.Value) &&
                                DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) >= startDate &&
                                DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) <= endDate &&
                                (int)ppb.PayrollPeriodMonth == toPayrollPeriodMonthAsInt &&
                                ppb.PayrollPeriod <= query.ToPayrollPeriod)
                            .ToListAsync();

                        var payrollProcessBatchesInBetween = await _db.PayrollProcessBatches
                            .AsNoTracking()
                            .Include(ppb => ppb.PayrollRecords)
                            .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                            .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee.Department))
                            .Where(ppb => !ppb.DeletedOn.HasValue &&
                                clientIds.Contains(ppb.ClientId.Value) &&
                                DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) >= startDate &&
                                DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) <= endDate &&
                                (int)ppb.PayrollPeriodMonth > fromPayrollPeriodMonthAsInt &&
                                (int)ppb.PayrollPeriodMonth < toPayrollPeriodMonthAsInt)
                            .ToListAsync();

                        var payrollProcessBatchesBetweenYears = new List<PayrollProcessBatch>();
                        if (startDate.Year == endDate.Year - 1)
                        {
                            var betweenYearStartDate = new DateTime(startDate.Year, 12, 1);
                            var betweenYearEndDate = new DateTime(endDate.Year, 1, 1);

                            payrollProcessBatchesBetweenYears = await _db.PayrollProcessBatches
                                .AsNoTracking()
                                .Include(ppb => ppb.PayrollRecords)
                                .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                                .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee.Department))
                                .Where(ppb => !ppb.DeletedOn.HasValue &&
                                    clientIds.Contains(ppb.ClientId.Value) &&
                                    DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) >= betweenYearStartDate &&
                                    DbFunctions.TruncateTime(ppb.PayrollPeriodFrom.Value) <= betweenYearEndDate)
                                .ToListAsync();
                        }

                        var payrollProcessBatches = payrollProcessBatchesInBeginningMonth
                            .Concat(payrollProcessBatchesInBetween)
                            .Concat(payrollProcessBatchesInEndingMonth)
                            .Concat(payrollProcessBatchesBetweenYears);

                        allPayrollProcessBatches.AddRange(payrollProcessBatches);
                    }
                }

                _db.Database.CommandTimeout = 30;

                allPayrollProcessBatches = allPayrollProcessBatches
                    .GroupBy(ppb => ppb.Id)
                    .Select(g => g.First())
                    .OrderBy(ppb => ppb.Id)
                    .ToList();

                var alphalistRecords = GetAlphalistRecords(query, allPayrollProcessBatches);

                if (query.AlphalistType == "7.1")
                {
                    alphalistRecords = alphalistRecords
                        .Where(ar => (!ar.Employee.IsActive.HasValue || (ar.Employee.IsActive.HasValue && !ar.Employee.IsActive.Value)) && (ar.Employee.DateResigned.HasValue && ar.Employee.DateResigned.Value.Year == query.PayrollPeriodToYear))
                        .ToList();
                }
                else if (query.AlphalistType == "7.5")
                {
                    alphalistRecords = alphalistRecords
                        .Where(ar => (ar.Employee.IsActive.HasValue && ar.Employee.IsActive.Value) && !ar.Employee.DateResigned.HasValue)
                        .ToList();
                }

                var weekdays = GetWeekdays(query.PayrollPeriodToYear);

                var thirteenthMonthQuery = new GenerateThirteenthMonth.Query
                {
                    ClientId = query.ClientId,
                    FromPayrollPeriod = query.ThirteenthMonthFromPayrollPeriod,
                    FromPayrollPeriodMonth = query.ThirteenthMonthFromPayrollPeriodMonth,
                    PayrollPeriodFromYear = query.ThirteenthMonthPayrollPeriodFromYear,
                    PayrollPeriodToYear = query.ThirteenthMonthPayrollPeriodToYear,
                    ToPayrollPeriod = query.ThirteenthMonthToPayrollPeriod,
                    ToPayrollPeriodMonth = query.ThirteenthMonthToPayrollPeriodMonth
                };
                var thirteenthMonthQueryResult = await _mediator.Send(thirteenthMonthQuery);

                var thirteenthMonthRecordsDictionary = thirteenthMonthQueryResult
                    .ThirteenthMonthRecords
                    .ToDictionary(t => t.Employee.Id, t => t);

                for (var i = 0; i < alphalistRecords.Count; i++)
                {
                    var alphalistRecord = alphalistRecords[i];

                    alphalistRecord.Order = i + 1;
                    alphalistRecord.NumberOfWeekdaysInToYear = weekdays;

                    if (thirteenthMonthRecordsDictionary.ContainsKey(alphalistRecord.Employee.Id))
                    {
                        alphalistRecord.PRES_NT_TotalThirteenthMonth = thirteenthMonthRecordsDictionary[alphalistRecord.Employee.Id].Whole;
                    }
                }

                if (query.Destination == "Excel")
                {
                    var queryResult = new QueryResult
                    {
                        AlphalistType = query.AlphalistType,
                        ClientId = query.ClientId,
                        DisplayMode = query.DisplayMode,
                        AlphalistRecords = alphalistRecords,
                        PayrollPeriodFromYear = query.PayrollPeriodFromYear,
                        PayrollPeriodToYear = query.PayrollPeriodToYear,
                        FromPayrollPeriodMonth = query.FromPayrollPeriodMonth,
                        FromPayrollPeriod = query.FromPayrollPeriod,
                        ToPayrollPeriodMonth = query.ToPayrollPeriodMonth,
                        ToPayrollPeriod = query.ToPayrollPeriod,
                        ThirteenthMonthFromPayrollPeriod = query.ThirteenthMonthFromPayrollPeriod,
                        ThirteenthMonthFromPayrollPeriodMonth = query.ThirteenthMonthFromPayrollPeriodMonth,
                        ThirteenthMonthPayrollPeriodFromYear = query.ThirteenthMonthPayrollPeriodFromYear,
                        ThirteenthMonthPayrollPeriodToYear = query.ThirteenthMonthPayrollPeriodToYear,
                        ThirteenthMonthToPayrollPeriod = query.ThirteenthMonthToPayrollPeriod,
                        ThirteenthMonthToPayrollPeriodMonth = query.ThirteenthMonthToPayrollPeriodMonth,
                        Query = query
                    };

                    var excelLines = new List<IList<string>>();

                    foreach (var alphalistRecord in alphalistRecords)
                    {
                        excelLines.Add(alphalistRecord.DisplayLine);
                    }

                    excelLines.Insert(0, queryResult.GetHeaders());
                    excelLines.Insert(0, queryResult.GetPreHeaders());

                    var totals = queryResult.GetTotals(queryResult.AlphalistRecords);
                    excelLines.Add(totals);

                    var reportFileContent = _excelBuilder.BuildExcelFile(excelLines);

                    var reportFileNameBuilder = new StringBuilder(64);
                    reportFileNameBuilder.Append($"Alphalist Report - ");

                    if (query.ClientId == -1)
                    {
                        reportFileNameBuilder.Append("All Clients");
                    }
                    else
                    {
                        reportFileNameBuilder.Append(clients.Single().Name);
                    }

                    reportFileNameBuilder.Append(" - ");

                    reportFileNameBuilder.Append($"{query.PayrollPeriodFromYear} to {query.PayrollPeriodToYear}");

                    reportFileNameBuilder.Append(".xlsx");

                    queryResult.FileContent = reportFileContent;
                    queryResult.Filename = reportFileNameBuilder.ToString();

                    return queryResult;
                }
                else if (query.Destination == "CSV")
                {
                    var queryResult = new QueryResult
                    {
                        AlphalistType = query.AlphalistType,
                        ClientId = query.ClientId,
                        DisplayMode = query.DisplayMode,
                        AlphalistRecords = alphalistRecords,
                        PayrollPeriodFromYear = query.PayrollPeriodFromYear,
                        PayrollPeriodToYear = query.PayrollPeriodToYear,
                        FromPayrollPeriodMonth = query.FromPayrollPeriodMonth,
                        FromPayrollPeriod = query.FromPayrollPeriod,
                        ToPayrollPeriodMonth = query.ToPayrollPeriodMonth,
                        ToPayrollPeriod = query.ToPayrollPeriod,
                        ThirteenthMonthFromPayrollPeriod = query.ThirteenthMonthFromPayrollPeriod,
                        ThirteenthMonthFromPayrollPeriodMonth = query.ThirteenthMonthFromPayrollPeriodMonth,
                        ThirteenthMonthPayrollPeriodFromYear = query.ThirteenthMonthPayrollPeriodFromYear,
                        ThirteenthMonthPayrollPeriodToYear = query.ThirteenthMonthPayrollPeriodToYear,
                        ThirteenthMonthToPayrollPeriod = query.ThirteenthMonthToPayrollPeriod,
                        ThirteenthMonthToPayrollPeriodMonth = query.ThirteenthMonthToPayrollPeriodMonth,
                        Query = query
                    };

                    var csvLines = new List<IList<string>>();

                    foreach (var alphalistRecord in alphalistRecords)
                    {
                        csvLines.Add(alphalistRecord.CSVDisplayLine);
                    }

                    csvLines.Insert(0, queryResult.GetCSVHeader());

                    var reportFileContent = _csvBuilder.BuildCSVFile(csvLines);

                    var reportFileNameBuilder = new StringBuilder(64);
                    reportFileNameBuilder.Append($"Alphalist Report - ");

                    if (query.ClientId == -1)
                    {
                        reportFileNameBuilder.Append("All Clients");
                    }
                    else
                    {
                        reportFileNameBuilder.Append(clients.Single().Name);
                    }

                    reportFileNameBuilder.Append(" - ");

                    reportFileNameBuilder.Append($"{query.PayrollPeriodFromYear} to {query.PayrollPeriodToYear}");

                    reportFileNameBuilder.Append(".csv");

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
                        AlphalistType = query.AlphalistType,
                        ClientId = query.ClientId,
                        ClientName = clientName,
                        DisplayMode = query.DisplayMode,
                        AlphalistRecords = alphalistRecords,
                        PayrollPeriodFromYear = query.PayrollPeriodFromYear,
                        PayrollPeriodToYear = query.PayrollPeriodToYear,
                        FromPayrollPeriodMonth = query.FromPayrollPeriodMonth,
                        FromPayrollPeriod = query.FromPayrollPeriod,
                        ToPayrollPeriodMonth = query.ToPayrollPeriodMonth,
                        ToPayrollPeriod = query.ToPayrollPeriod,
                        ThirteenthMonthFromPayrollPeriod = query.ThirteenthMonthFromPayrollPeriod,
                        ThirteenthMonthFromPayrollPeriodMonth = query.ThirteenthMonthFromPayrollPeriodMonth,
                        ThirteenthMonthPayrollPeriodFromYear = query.ThirteenthMonthPayrollPeriodFromYear,
                        ThirteenthMonthPayrollPeriodToYear = query.ThirteenthMonthPayrollPeriodToYear,
                        ThirteenthMonthToPayrollPeriod = query.ThirteenthMonthToPayrollPeriod,
                        ThirteenthMonthToPayrollPeriodMonth = query.ThirteenthMonthToPayrollPeriodMonth,
                        Query = query
                    };
                }
            }

            private IList<QueryResult.AlphalistRecord> GetAlphalistRecords(Query query, IList<PayrollProcessBatch> payrollProcessBatches)
            {
                var alphalistRecordsDictionary = new Dictionary<int, QueryResult.AlphalistRecord>();

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
                            var payrollRecordsInMonth = group
                                .Where(ppb => ppb.PayrollPeriodFrom.Value.Year == i && (int)ppb.PayrollPeriodMonth / 10 == j)
                                .SelectMany(ppb => ppb.PayrollRecords)
                                .ToList();

                            var isJanuaryFirstPayrollPeriod = j == 1 && group.Key == 1;
                            if (isJanuaryFirstPayrollPeriod)
                            {
                                var payrollRecordsInJanuaryFirstPeriod = group
                                    .Where(ppb => ppb.PayrollPeriodFrom.Value.Year == i - 1 && (int)ppb.PayrollPeriodMonth / 10 == j)
                                    .SelectMany(ppb => ppb.PayrollRecords)
                                    .ToList();

                                if (payrollRecordsInJanuaryFirstPeriod.Any())
                                {
                                    foreach (var record in payrollRecordsInJanuaryFirstPeriod)
                                    {
                                        payrollRecordsInMonth.Add(record);
                                    }
                                }
                            }

                            payrollRecordsInMonth = payrollRecordsInMonth
                                .OrderBy(pr => pr.Employee.LastName)
                                .ThenBy(pr => pr.Employee.FirstName)
                                .ToList();

                            AddPayrollRecordsToAlphalistRecordsDictionary(query, alphalistRecordsDictionary, i, j, group, payrollRecordsInMonth);
                        }
                    }
                }

                return alphalistRecordsDictionary.Select(t => t.Value).OrderBy(t => t.Employee.LastName).ThenBy(t => t.Employee.FirstName).ToList();
            }

            private static void AddPayrollRecordsToAlphalistRecordsDictionary(Query query, Dictionary<int, QueryResult.AlphalistRecord> alphalistRecordsDictionary, int year, int month, IGrouping<int, PayrollProcessBatch> group, List<PayrollRecord> payrollRecordsInMonth)
            {
                foreach (var payrollRecord in payrollRecordsInMonth)
                {
                    if (!alphalistRecordsDictionary.ContainsKey(payrollRecord.EmployeeId.Value))
                    {
                        alphalistRecordsDictionary.Add(payrollRecord.EmployeeId.Value, new QueryResult.AlphalistRecord(query.DateGenerated) { Employee = payrollRecord.Employee, Query = query });
                    }

                    var alphalistRecord = alphalistRecordsDictionary[payrollRecord.EmployeeId.Value];
                    alphalistRecord.TotalHoursWorkedValue += payrollRecord.HoursWorkedValue.GetValueOrDefault();
                    alphalistRecord.TotalDaysWorkedValue += payrollRecord.DaysWorkedValue.GetValueOrDefault();
                    alphalistRecord.TotalCOLAHourlyValue += payrollRecord.COLAHourlyValue.GetValueOrDefault();
                    alphalistRecord.TotalCOLADailyValue += payrollRecord.COLADailyValue.GetValueOrDefault();
                    alphalistRecord.TotalCOLAMonthlyValue += payrollRecord.COLAMonthlyValue.GetValueOrDefault();
                    alphalistRecord.PRES_NT_TotalOvertimeValue += payrollRecord.OvertimeValue.GetValueOrDefault();
                    alphalistRecord.TotalSSSValue += payrollRecord.SSSValueEmployee.GetValueOrDefault();
                    alphalistRecord.TotalPHICValue += payrollRecord.PHICValueEmployee.GetValueOrDefault();
                    alphalistRecord.TotalPagIbigValue += payrollRecord.PagIbigValueEmployee.GetValueOrDefault();
                }
            }

            private static int GetWeekdays(int year)
            {
                var fromTime = new DateTime(year, 1, 1);
                var toTime = new DateTime(year, 12, 31);

                TimeSpan ts = toTime - fromTime;
                int weeks = ts.Days / 7;
                int weekdays = weeks * 5;
                for (int i = (weeks * 7) + 1; i <= ts.Days; i++)
                {
                    if (fromTime.AddDays(i).DayOfWeek != DayOfWeek.Sunday && fromTime.AddDays(i).DayOfWeek != DayOfWeek.Saturday)
                    { weekdays++; }
                }
                return weekdays;
            }
        }
    }
}
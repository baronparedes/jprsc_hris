using FluentValidation;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Infrastructure.NET;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.Infrastructure.CSV;
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
            public IList<Table> Tables { get; set; } = new List<Table>();
            public byte[] FileContent { get; set; }
            public string Filename { get; set; }
            public IList<AlphalistRecord> AlphalistRecords { get; set; } = new List<AlphalistRecord>();
            public Query Query { get; set; }

            public class AlphalistRecord
            {
                public AlphalistRecord()
                {                        
                }

                public AlphalistRecord(DateTime dateGenerated)
                {
                    DateGenerated = dateGenerated;
                }

                public DateTime DateGenerated { get; set; }
                public int Order { get; set; }

                public Employee Employee { get; set; }
                public Query Query { get; set; }
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
                public string PREV_T_ThirteenthMonthPayAndOtherBenefits { get; set; } = "NA";
                public string PREV_T_SalariesAndOtherFormsOfCompensations { get; set; } = "NA";
                public string PREV_T_TotalIncome { get; set; } = "NA";
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
                public string PRES_T_ThirteenthMonthPayAndOtherBenefits { get; set; } = "NA";
                public string PRES_T_SalariesAndOtherFormsOfCompensations { get; set; } = "NA";
                public string PRES_T_TotalCompensationPresent { get; set; } = "NA";
                public string PRES_T_TotalCompensationIncome { get; set; } = "NA";
                public string PRES_T_ExemptionCode { get; set; } = "NA";
                public string PRES_T_Amount { get; set; } = "NA";
                public string PRES_T_PremiumPaidHospital { get; set; } = "NA";
                public string PRES_T_NetTaxableCompensationIncome { get; set; } = "NA";
                public string PRES_T_TaxDue { get; set; } = "NA";
                public string PRES_TW_PreviousEmployer { get; set; } = "NA";
                public string PRES_TW_PresentEmployer { get; set; } = "NA";
                public string PRES_YEA_AmountWithheldPaidDecember { get; set; } = "NA";
                public string PRES_YEA_OverWithheldTax { get; set; } = "NA";
                public string PRES_YEA_AmountWithheldRequested { get; set; } = "NA";
            }
        }

        public class CSVTableBuilder : TableBuilder<QueryResult.AlphalistRecord>
        {
            public CSVTableBuilder(DateTime dateGenerated, string alphalistType)
            {
                string aType = alphalistType == "7.1" ? "2" : "2";
                Column(String.Empty, item => "D" + aType);
                Column("H" + BIRFormNumber, item => BIRFormNumber);
                Column(CompanyTIN, item => CompanyTIN);
                Column(U_0039, item => U_0039);
                Column(String.Format("{0:MM/dd/yyyy}", dateGenerated), item => String.Format("{0:MM/dd/yyyy}", dateGenerated));
                Column(U_N, item => item.Order.ToString());
                Column("0", item => String.IsNullOrWhiteSpace(item.Employee.TIN) ? "NO TIN": item.Employee.TIN.Replace("-", ""));
                Column(U_039, item => U_0039);
                Column(String.Empty, item => $"\"{item.Employee.LastName}\"");
                Column(String.Empty, item => $"\"{item.Employee.FirstName}\"");
                Column(String.Empty, item => $"\"{String.Format("{0}", String.IsNullOrWhiteSpace(item.Employee.MiddleName) ? String.Empty : $", {item.Employee.MiddleName}")}\"");
                Column(String.Empty, item => $"\"{item.Employee.Region}\"");

                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");

                Column(String.Empty, item => item.Employee.DateHired.HasValue ? $"{item.Employee.DateHired.Value:MM/dd/yyyy}" : String.Empty);
                Column(String.Empty, item => item.Employee.DateResigned.HasValue ? $"{item.Employee.DateResigned.Value:MM/dd/yyyy}" : $"12/31/{dateGenerated.Year - 1}");
                Column(String.Empty, item => String.Format("{0:f2}", item.PRES_NT_TotalEarnings));

                Column(String.Empty, item => String.Format("{0:f2}", item.Employee.DailyRate));

                Column(String.Empty, item => "0.00"); // Unknown

                Column(String.Empty, item => String.Format("{0}", "313")); //no of days in the year


                Column(String.Empty, item => "0.00");


                Column(String.Empty, item => String.Format("{0:f2}", item.PRES_NT_TotalOvertimeValue));

                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");

                Column(String.Empty, item => String.Format("{0:f2}", item.PRES_NT_TotalThirteenthMonth));

                Column(String.Empty, item => "0.00");


                Column(String.Empty, item => String.Format("{0:f2}", item.PRES_NT_TotalContributions));

                Column(String.Empty, item => String.Format("{0:f2}", item.PRES_T_SalariesAndOtherFormsOfCompensations));
                Column(String.Empty, item => String.Format("{0:f2}", item.PRES_NT_TotalEarnings - item.PRES_NT_TotalOvertimeValue - item.PRES_NT_TotalThirteenthMonth - item.PRES_NT_TotalContributions));


                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");
                Column(String.Empty, item => "0.00");


                Column(String.Empty, item => "FILIPINO");
                Column(String.Empty, item => "R");
                Column(String.Empty, item => "TR");
            }
        }

        public class PageTableBuilder : TableBuilder<QueryResult.AlphalistRecord>
        {
            public PageTableBuilder()
            {
                Column(String.Empty, "Employee Last Name", item => item.Employee.LastName);
                Column(String.Empty, "First Name", item => item.Employee.FirstName);
                Column(String.Empty, "Middle Name", item => String.Format("{0}", String.IsNullOrWhiteSpace(item.Employee.MiddleName) ? null : $"{item.Employee.MiddleName}"));
                Column(String.Empty, "Employee Code", item => item.Employee.EmployeeCode);
                Column("Gross Compensation Income Previous Employer Non-Taxable", "Employee TIN", item => item.Employee.TIN, items => "TOTAL");
                Column(String.Empty, "Employee Region", item => item.Employee.Region.HasValue ? EnumHelper.GetDisplayName(item.Employee.Region.Value) : String.Empty, items => String.Format("{0:n}", items.Sum(a => a.PRES_NT_TotalEarnings)));
                Column(String.Empty, "Permanent Addrress", item => item.Employee.PermanentAddress);
                Column(String.Empty, "Birth Date", item => item.Employee.DateOfBirth.HasValue ? $"{item.Employee.DateOfBirth.Value:MM/dd/yyyy}" : String.Empty);
                Column(String.Empty, "Contact Number", item => item.Employee.CelNo);
                Column(String.Empty, "Zip Code", item => item.Employee.ZipCode);
                Column(String.Empty, "Gross compensation income previous", item => item.PREV_NT__GrossCompensationIncomePrevious, items => String.Format("{0:n}", items.Sum(a => a.PRES_NT_TotalOvertimeValue)));
                Column(String.Empty, "Regular Pay", item => "NA", items => String.Format("{0:n}", items.Sum(a => a.PRES_NT_TotalThirteenthMonth + a.PRES_NT_TotalEarnings)));
                Column(String.Empty, "Total Earnings", item => "NA", items => String.Format("{0:n}", items.Sum(a => a.PRES_NT_TotalContributions)));
                Column(String.Empty, "Basic/SMW", item => item.PREV_NT_BasicSMW, items => String.Format("{0:n}", items.Sum(a => a.PRES_NT_TotalEarnings + a.PRES_NT_TotalOvertimeValue + a.PRES_NT_TotalThirteenthMonth + a.PRES_NT_TotalContributions)));
                Column(String.Empty, "Holiday Pay", item => item.PREV_NT_HolidayPay);
                Column("Taxable", "Overtime Pay", item => item.PREV_NT_OvertimePay);
                Column(String.Empty, "Night Shift Differential", item => item.PREV_NT_NightShiftDifferential);
                Column(String.Empty, "Hazard Pay", item => item.PREV_NT_HazardPay);
                Column("Gross Compensation Income Present Employer Non-Taxable", "13th month pay and other benefits", item => item.PREV_NT_ThirteenthMonthPayAndOtherBenefits);
                Column(String.Empty, "De minimis benefits", item => item.PREV_NT_DeMinimisBenefits);
                Column(String.Empty, "SSS, GSIS, PHIC and PAG-IBIG Contris and other union dues", item => item.PREV_NT_Contributions);
                Column(String.Empty, "Salaries and other forms of compensations", item => item.PREV_NT_SalariesAndOtherFormsOfCompensations);
                Column(String.Empty, "Total non-taxable/exempt compensation income", item => item.PREV_NT_TotalIncome);
                Column(String.Empty, "13th month pay and other benefits", item => item.PREV_T_ThirteenthMonthPayAndOtherBenefits);
                Column(String.Empty, "Salaries and other forms of compensation", item => item.PREV_T_SalariesAndOtherFormsOfCompensations);
                Column(String.Empty, "Total taxable (previous employer)", item => item.PREV_T_TotalIncome);
                Column(String.Empty, "Date Hired", item => item.Employee.DateHired.HasValue ? $"{item.Employee.DateHired.Value:MM/dd/yyyy}" : String.Empty);
                Column(String.Empty, "Date Resigned", item => item.Employee.DateResigned.HasValue ? $"{item.Employee.DateResigned.Value:MM/dd/yyyy}" : "present");
                Column(String.Empty, "Gross compensation present", item => String.Format("{0:n}", item.PRES_NT_TotalEarnings));
                Column(String.Empty, "Regular Pay", item => String.Format("{0:n}", item.PRES_NT_RegularPay));
                Column(String.Empty, "Total Earnings", item => String.Format("{0:n}", item.TotalEarningsValue));
                Column(String.Empty, "Basic SMW per day", item => item.Employee.DailyRate.HasValue ? String.Format("{0:n}", item.Employee.DailyRate) : String.Empty);
                Column("Taxable", "Basic SMW per month", item => item.Employee.MonthlyRate.HasValue ? String.Format("{0:n}", item.Employee.MonthlyRate) : String.Empty);
                Column(String.Empty, "Basic SMW per year", item => item.Employee.MonthlyRate.HasValue ? String.Format("{0:n}", item.Employee.MonthlyRate.Value * 12) : String.Empty);
                Column(String.Empty, "Factor used (no of days/year)", item => item.NumberOfWeekdaysInToYear.ToString());
                Column(String.Empty, "Holiday Pay", item => item.PRES_NT_HolidayPay);
                Column("Exemption", "Overtime Pay", item => String.Format("{0:n}", item.PRES_NT_TotalOvertimeValue));
                Column(String.Empty, "Night Shift Differential", item => item.PRES_NT_NightShiftDifferential);
                Column(String.Empty, "Hazard Pay", item => item.PRES_NT_HazardPay);
                Column(String.Empty, "13th month pay and other benefits", item => String.Format("{0:n}", item.PRES_NT_TotalThirteenthMonth));
                Column(String.Empty, "De minimis benefits", item => item.PRES_NT_DeMinimisBenefits);
                Column("Tax Withheld", "SSS, GSIS, PHIC and PAG-IBIG Contris and other union dues", item => String.Format("{0:n}", item.PRES_NT_TotalContributions));
                Column(String.Empty, "Salaries and other forms of compensations", item => String.Format("{0:n}", item.PRES_NT_TotalEarnings - item.PRES_NT_TotalOvertimeValue - item.PRES_NT_TotalThirteenthMonth - item.PRES_NT_TotalContributions));
                Column("Year-End Adjustment (11a or 11b)", "13th month pay and other benefits", item => item.PRES_T_ThirteenthMonthPayAndOtherBenefits);
                Column(String.Empty, "Salaries and other forms of compensations", item => item.PRES_T_SalariesAndOtherFormsOfCompensations);
                Column(String.Empty, "Total compensation present", item => item.PRES_T_TotalCompensationPresent);
                Column(String.Empty, "Total compensation income (previous and present employers)", item => item.PRES_T_TotalCompensationIncome);
                Column(String.Empty, "Exemption code", item => item.PRES_T_ExemptionCode);
                Column(String.Empty, "Amount", item => item.PRES_T_Amount);
                Column(String.Empty, "Premium paid on health and or hospital insurance", item => item.PRES_T_PremiumPaidHospital);
                Column(String.Empty, "Net taxable compensation income", item => item.PRES_T_NetTaxableCompensationIncome);
                Column(String.Empty, "Tax due (Jan - Dec)", item => item.PRES_T_TaxDue);
                Column(String.Empty, "Previous Employer", item => item.PRES_TW_PreviousEmployer);
                Column(String.Empty, "Present Employer", item => item.PRES_TW_PresentEmployer);
                Column(String.Empty, "Amt withheld and paid for in December", item => item.PRES_YEA_AmountWithheldPaidDecember);
                Column(String.Empty, "Over withheld tax employee", item => item.PRES_YEA_OverWithheldTax);
                Column(String.Empty, "Amount of tax withheld as adjusted", item => item.PRES_YEA_AmountWithheldRequested);
            }

            public PageTableBuilder(string title) : this()
            {
                Title(title);
            }
        }

        public class ExcelTableBuilder : PageTableBuilder
        {

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

                var allPayrollProcessBatches = await GetPayrollProcessBatches(query, clientIds);

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

                var reportFileNameBase = new StringBuilder(64)
                    .Append($"Alphalist Report - ")
                    .Append(query.ClientId == -1 ? "All Clients" : clients.Single().Name)
                    .Append(" - ")
                    .Append($"{query.PayrollPeriodFromYear} to {query.PayrollPeriodToYear}");

                if (query.Destination == "Excel")
                {
                    var excelTableBuilder = new ExcelTableBuilder();
                    var excelTable = excelTableBuilder.Build(alphalistRecords);

                    return new QueryResult
                    {
                        FileContent = _excelBuilder.BuildExcelFile(excelTable.AllLines),
                        Filename = reportFileNameBase.Append(".xlsx").ToString()
                    };
                }
                else if (query.Destination == "CSV")
                {
                    var csvTableBuilder = new CSVTableBuilder(query.DateGenerated, query.AlphalistType);
                    var csvTable = csvTableBuilder.Build(alphalistRecords);

                    return new QueryResult
                    {
                        FileContent = _csvBuilder.BuildCSVFile(csvTable.AllLines),
                        Filename = reportFileNameBase.Append(".csv").ToString()
                    };
                }
                else
                {
                    var clientName = String.Empty;
                    if (query.ClientId.HasValue && query.ClientId.Value > 0)
                    {
                        clientName = (await _db.Clients.AsNoTracking().SingleAsync(c => c.Id == query.ClientId)).Name;
                    }

                    var tables = new List<Table>();

                    if (String.IsNullOrWhiteSpace(query.DisplayMode))
                    {
                        var pageTableBuilder = new PageTableBuilder();
                        var pageTable = pageTableBuilder.Build(alphalistRecords);
                        tables.Add(pageTable);
                    }
                    else if (query.DisplayMode == "Branch")
                    {
                        var pageTableBuilders = new List<PageTableBuilder>();

                        foreach (var alphalistRecord in alphalistRecords.Where(pr => pr.Employee.Department == null))
                        {
                            alphalistRecord.Employee.Department = new Department { Id = 0, Name = "No Branch" };
                        }

                        var groupedByBranch = alphalistRecords.GroupBy(pr => new { pr.Employee.Department.Id, pr.Employee.Department.Name });

                        foreach (var group in groupedByBranch.OrderBy(g => g.Key.Name))
                        {
                            var pageTableBuilder = new PageTableBuilder(group.Key.Name);
                            var pageTable = pageTableBuilder.Build(group);
                            tables.Add(pageTable);
                        }
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
                        Query = query,
                        Tables = tables
                    };
                }
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
                return allPayrollProcessBatches;
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
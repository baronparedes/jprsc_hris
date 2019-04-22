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
    public class GenerateLoanLedger
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
            public IList<IList<string>> Lines { get; set; } = new List<IList<string>>();
            public int? PayrollPeriodMonth { get; set; }
            public Month? PayrollPeriodMonthMonth { get; set; }
            public int PayrollPeriodYear { get; set; }
            public IList<SSSRecord> SSSRecords { get; set; } = new List<SSSRecord>();

            public class SSSRecord
            {
                public Loan Loan { get; set; }
                public DateTime? DateDeducted { get; set; }
                public decimal AmountDeducted { get; set; }
                public string SSS { get; set; }
                public string FirstName { get; set; }
                public string LastName { get; set; }
                public string MiddleName { get; set; }

                public IList<string> DisplayLine
                {
                    get
                    {
                        var line = new List<string>();

                        line.Add(SSS);
                        line.Add(LastName);
                        line.Add(FirstName);
                        line.Add(String.IsNullOrWhiteSpace(MiddleName) ? null : MiddleName.Trim().First().ToString());
                        line.Add(String.Format("{0}", Loan.LoanType.Code));
                        line.Add(String.Format("{0:M/d/yyyy}", Loan.LoanDate));
                        line.Add(String.Format("{0:n}", Loan.PrincipalAmount));
                        line.Add(String.Empty);
                        line.Add(String.Format("{0:n}", Loan.RemainingBalanceForDisplay));
                        line.Add(String.Format("{0:n}", Loan.DeductionAmount));
                        line.Add(String.Format("{0:n}", AmountDeducted));
                        line.Add(String.Format("{0:M/d/yyyy}", DateDeducted));
                        line.Add(String.Format("{0:n}", Loan.AmountPaid));
                        line.Add(String.Empty);
                        line.Add(String.Empty);
                        line.Add(String.Empty);

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
                        .Where(ppb => !ppb.DeletedOn.HasValue && clientIds.Contains(ppb.ClientId.Value) && ppb.PayrollPeriodFrom.Value.Year == query.PayrollPeriodYear)
                        .ToListAsync() :
                    await _db.PayrollProcessBatches
                        .Include(ppb => ppb.PayrollRecords)
                        .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                        .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee.Department))
                        .Where(ppb => !ppb.DeletedOn.HasValue && clientIds.Contains(ppb.ClientId.Value) && ppb.PayrollPeriodMonth.HasValue && (int)ppb.PayrollPeriodMonth == query.PayrollPeriodMonth && ppb.PayrollPeriodFrom.Value.Year == query.PayrollPeriodYear)
                        .ToListAsync();

                var sssRecords = await GetLoanSSSRecords(payrollProcessBatches);

                if (query.Destination == "Excel")
                {
                    var excelLines = sssRecords.Select(pr => pr.DisplayLine).ToList();
                    excelLines.Insert(0, new List<string> { "Employee SSS No.", "Last Name", "First Name", "Middle Initial", "Loan Type", "Loan Date", "Loan Amount", "Penalty", "Amount Due", "Deduction Amount", "Amount Deducted", "Date Deducted", "Amount Paid", "AMPSDG", "Status", "Effective Date" });
                    excelLines.Add(new List<string> { String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Format("{0:n}", sssRecords.Sum(sr => sr.Loan.PrincipalAmount.GetValueOrDefault())), String.Empty, String.Format("{0:n}", sssRecords.Sum(sr => sr.Loan.RemainingBalanceForDisplay.GetValueOrDefault())), String.Format("{0:n}", sssRecords.Sum(sr => sr.Loan.DeductionAmount.GetValueOrDefault())), String.Format("{0:n}", sssRecords.Sum(sr => sr.AmountDeducted)), String.Empty, String.Format("{0:n}", sssRecords.Sum(sr => sr.Loan.AmountPaid.GetValueOrDefault())), String.Empty, String.Empty, String.Empty });

                    var reportFileContent = _excelBuilder.BuildExcelFile(excelLines);

                    var reportFileNameBuilder = new StringBuilder(64);
                    reportFileNameBuilder.Append($"Loan Ledger Report - ");

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

            private async Task<IList<QueryResult.SSSRecord>> GetLoanSSSRecords(IList<PayrollProcessBatch> payrollProcessBatches)
            {
                var records = new List<QueryResult.SSSRecord>();

                foreach (var payrollProcessBatch in payrollProcessBatches)
                {
                    var clientEmployeeIds = await _db
                        .Employees
                        .AsNoTracking()
                        .Where(e => !e.DeletedOn.HasValue && e.ClientId == payrollProcessBatch.ClientId && e.DailyRate.HasValue)
                        .Select(e => e.Id)
                        .ToListAsync();

                    var loans = await _db.Loans
                        .Include(l => l.Employee)
                        .Include(l => l.LoanType)
                        .Where(l => !l.DeletedOn.HasValue && clientEmployeeIds.Contains(l.EmployeeId.Value) && !l.ZeroedOutOn.HasValue && DbFunctions.TruncateTime(l.StartDeductionDate) <= DbFunctions.TruncateTime(payrollProcessBatch.PayrollPeriodTo))
                        .ToListAsync();

                    loans = loans.Where(l => l.LoanPayrollPeriods.Contains(payrollProcessBatch.PayrollPeriod.Value)).ToList();

                    var payrollRecordIds = payrollProcessBatch.PayrollRecords.Select(pr => pr.Id).ToList();
                    var loanDeductions = await _db
                        .LoanDeductions
                        .Where(ld => payrollRecordIds.Contains(ld.PayrollRecordId.Value))
                        .ToListAsync();

                    var recordsTemp = loans.Select(l => new QueryResult.SSSRecord
                    {
                        Loan = l,
                        DateDeducted = payrollProcessBatch.EndProcessedOn,
                        AmountDeducted = payrollProcessBatch.EndProcessedOn.HasValue ? (loanDeductions.SingleOrDefault(ld => ld.LoanId == l.Id)?.DeductionAmount).GetValueOrDefault() : 0m
                    });

                    records.AddRange(recordsTemp);
                }

                records = records
                    .OrderBy(r => r.Loan.Employee.LastName)
                    .ThenBy(r => r.Loan.Employee.FirstName)
                    .ThenBy(r => r.Loan.LoanType.Code)
                    .ToList();

                int currentEmployeeId = 0;

                foreach (var record in records)
                {
                    if (record.Loan.Employee.Id != currentEmployeeId)
                    {
                        currentEmployeeId = record.Loan.Employee.Id;
                        record.SSS = record.Loan.Employee.SSS;
                        record.FirstName = record.Loan.Employee.FirstName;
                        record.LastName = record.Loan.Employee.LastName;
                        record.MiddleName = record.Loan.Employee.MiddleName;
                    }
                }

                return records;
            }
        }
    }
}
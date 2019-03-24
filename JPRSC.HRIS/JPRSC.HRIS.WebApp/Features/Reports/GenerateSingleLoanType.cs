using AutoMapper;
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
    public class GenerateSingleLoanType
    {
        public class Query : IRequest<QueryResult>
        {
            public int? ClientId { get; set; }
            public string Destination { get; set; }
            public string DisplayMode { get; set; }
            public int LoanTypeId { get; set; }
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
            public int LoanTypeId { get; set; }
            public LoanType LoanTypeResult { get; set; }
            public int? PayrollPeriodMonth { get; set; }
            public Month? PayrollPeriodMonthMonth { get; set; }
            public IList<LoanRecord> LoanRecords { get; set; } = new List<LoanRecord>();

            public class LoanType
            {
                public string Code { get; set; }
                public string Description { get; set; }
                public int Id { get; set; }
            }

            public class LoanRecord
            {
                public Loan Loan { get; set; }
                public decimal? DeductionAmount { get; set; }

                public IList<string> DisplayLine
                {
                    get
                    {
                        var line = new List<string>();

                        line.Add(Loan.Employee.SSS);
                        line.Add(Loan.Employee.LastName);
                        line.Add(Loan.Employee.FirstName);
                        line.Add(String.IsNullOrWhiteSpace(Loan.Employee.MiddleName) ? null : Loan.Employee.MiddleName.Trim().First().ToString());
                        line.Add(String.Format("{0}", Loan.LoanType.Code));
                        line.Add(String.Format("{0:M/d/yyyy}", Loan.LoanDate));
                        line.Add(String.Format("{0:n}", Loan.PrincipalAmount));
                        line.Add(String.Empty);
                        line.Add(String.Format("{0:n}", Loan.RemainingBalanceForDisplay));
                        line.Add(String.Format("{0:n}", DeductionAmount));
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
                        .Where(ppb => !ppb.DeletedOn.HasValue && clientIds.Contains(ppb.ClientId.Value))
                        .ToListAsync() :
                    await _db.PayrollProcessBatches
                        .Include(ppb => ppb.PayrollRecords)
                        .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                        .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee.Department))
                        .Where(ppb => !ppb.DeletedOn.HasValue && clientIds.Contains(ppb.ClientId.Value) && ppb.PayrollPeriodMonth.HasValue && (int)ppb.PayrollPeriodMonth == query.PayrollPeriodMonth)
                        .ToListAsync();

                var loanType = await _db.LoanTypes.Where(l => l.Id == query.LoanTypeId).ProjectToSingleAsync<QueryResult.LoanType>();

                var loanRecords = await GetLoanRecords(query, payrollProcessBatches);

                if (query.Destination == "Excel")
                {
                    var excelLines = loanRecords.Select(pr => pr.DisplayLine).ToList();
                    excelLines.Insert(0, new List<string> { "Employee SSS No.", "Last Name", "First Name", "Middle Initial", "Loan Type", "Loan Date", "Loan Amount", "Penalty", "Amount Due", "Monthly Amortization", "AMPSDG", "Status", "Effective Date" });
                    excelLines.Add(new List<string> { String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Format("{0:n}", loanRecords.Sum(sr => sr.Loan.PrincipalAmount.GetValueOrDefault())), String.Empty, String.Format("{0:n}", loanRecords.Sum(sr => sr.Loan.RemainingBalanceForDisplay.GetValueOrDefault())), String.Format("{0:n}", loanRecords.Sum(sr => sr.DeductionAmount.GetValueOrDefault())), String.Empty, String.Empty, String.Empty });

                    var reportFileContent = _excelBuilder.BuildExcelFile(excelLines);

                    var reportFileNameBuilder = new StringBuilder(64);
                    reportFileNameBuilder.Append($"{loanType.Code} Report - ");

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
                        Filename = reportFileNameBuilder.ToString(),
                        LoanTypeResult = loanType
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
                        LoanTypeId = query.LoanTypeId,
                        LoanTypeResult = loanType,
                        LoanRecords = loanRecords,
                        PayrollPeriodMonth = query.PayrollPeriodMonth,
                        PayrollPeriodMonthMonth = payrollPeriodMonth
                    };
                }
            }

            private async Task<IList<QueryResult.LoanRecord>> GetLoanRecords(Query query, IList<PayrollProcessBatch> payrollProcessBatches)
            {
                var marchFour2019 = new DateTime(2019, 3, 4);

                var allLoanDeductions = new List<LoanDeduction>();

                foreach (var payrollProcessBatch in payrollProcessBatches)
                {
                    var clientEmployeeIds = await _db
                        .Employees
                        .AsNoTracking()
                        .Where(e => !e.DeletedOn.HasValue && e.ClientId == payrollProcessBatch.ClientId && e.DailyRate.HasValue)
                        .Select(e => e.Id)
                        .ToListAsync();

                    if (payrollProcessBatch.AddedOn < marchFour2019)
                    {
                        var loans = await _db.Loans
                            .Include(l => l.Employee)
                            .Include(l => l.LoanType)
                            .Where(l => l.LoanTypeId == query.LoanTypeId && !l.DeletedOn.HasValue && clientEmployeeIds.Contains(l.EmployeeId.Value) && DbFunctions.TruncateTime(l.StartDeductionDate) <= DbFunctions.TruncateTime(payrollProcessBatch.PayrollPeriodTo))
                            .ToListAsync();

                        loans = loans.Where(l => l.LoanPayrollPeriods.Contains(payrollProcessBatch.PayrollPeriod.Value)).ToList();

                        allLoanDeductions.AddRange(loans.Select(l => new LoanDeduction { Loan = l, DeductionAmount = l.DeductionAmount }));
                    }
                    else
                    {
                        foreach (var payrollRecord in payrollProcessBatch.PayrollRecords)
                        {
                            var loanDeductions = await _db.LoanDeductions
                                .Include(ld => ld.Loan)
                                .Include(ld => ld.Loan.Employee)
                                .Include(ld => ld.Loan.LoanType)
                                .Where(ld => ld.PayrollRecordId.HasValue && ld.PayrollRecordId == payrollRecord.Id && ld.Loan.EmployeeId.HasValue && clientEmployeeIds.Contains(ld.Loan.EmployeeId.Value) && ld.Loan.LoanTypeId == query.LoanTypeId)
                                .ToListAsync();

                            allLoanDeductions.AddRange(loanDeductions);
                        }
                    }
                }

                return allLoanDeductions
                    .OrderBy(l => l.Loan.Employee.LastName)
                    .ThenBy(l => l.Loan.Employee.FirstName)
                    .Select(l => new QueryResult.LoanRecord
                    {
                        Loan = l.Loan,
                        DeductionAmount = l.DeductionAmount
                    })
                    .ToList();
            }
        }
    }
}
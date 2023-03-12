using AutoMapper;
using AutoMapper.QueryableExtensions;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.Features.Payroll;
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
    public class GenerateSingleLoanType
    {
        private const int HDMFCALL_ID = 7;
        private const int HDMFL_ID = 10;

        public class Query : IRequest<QueryResult>
        {
            public int? ClientId { get; set; }
            public string Destination { get; set; }
            public string DisplayMode { get; set; }
            public int LoanTypeId { get; set; }
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
            public int LoanTypeId { get; set; }
            public LoanType LoanTypeResult { get; set; }
            public int? PayrollPeriodMonth { get; set; }
            public Month? PayrollPeriodMonthMonth { get; set; }
            public int PayrollPeriodYear { get; set; }
            public IList<LoanRecord> LoanRecords { get; set; } = new List<LoanRecord>();
            public string SSSOrPagibigHeader { get; set; }

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
                public string SSSOrPagIbigNumber { get; set; }

                public IList<string> DisplayLine
                {
                    get
                    {
                        var line = new List<string>();

                        line.Add(SSSOrPagIbigNumber);
                        line.Add(Loan.Employee.LastName);
                        line.Add(Loan.Employee.FirstName);
                        line.Add(String.IsNullOrWhiteSpace(Loan.Employee.MiddleName) ? null : Loan.Employee.MiddleName);
                        line.Add(String.IsNullOrWhiteSpace(Loan.Employee.MiddleName) ? null : Loan.Employee.MiddleName.Trim().First().ToString());
                        line.Add(String.Format("{0:M/d/yyyy}", Loan.Employee.DateOfBirth));
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

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<LoanType, QueryResult.LoanType>();
            }
        }

        public class QueryHandler : IRequestHandler<Query, QueryResult>
        {
            private readonly ApplicationDbContext _db;
            private readonly IExcelBuilder _excelBuilder;
            private readonly IMapper _mapper;
            private Models.SystemSettings _systemSettings;

            public QueryHandler(ApplicationDbContext db, IExcelBuilder excelBuilder, IMapper mapper)
            {
                _db = db;
                _excelBuilder = excelBuilder;
                _mapper = mapper;
            }

            public async Task<QueryResult> Handle(Query query, CancellationToken token)
            {
                _systemSettings = await _db.SystemSettings.AsNoTracking().SingleAsync();

                var clients = query.ClientId == -1 ?
                    await _db.Clients.AsNoTracking().Where(c => !c.DeletedOn.HasValue).ToListAsync() :
                    await _db.Clients.AsNoTracking().Where(c => !c.DeletedOn.HasValue && c.Id == query.ClientId.Value).ToListAsync();

                var clientIds = clients.Select(c => c.Id).ToList();

                var payrollProcessBatches = await _db.PayrollProcessBatchesByMonthAndYear(clientIds, query.PayrollPeriodMonth, query.PayrollPeriodYear);

                var loanType = await _db.LoanTypes.AsNoTracking().Where(l => l.Id == query.LoanTypeId).ProjectTo<QueryResult.LoanType>(_mapper).SingleAsync();

                var loanRecords = await GetLoanRecords(query, payrollProcessBatches);

                if (query.Destination == "Excel")
                {
                    var sssOrPagibig = query.LoanTypeId == HDMFCALL_ID || query.LoanTypeId == HDMFL_ID ? "Employee PagIbig No." : "Employee SSS No.";

                    var excelLines = loanRecords.Select(pr => pr.DisplayLine).ToList();
                    excelLines.Insert(0, new List<string> { sssOrPagibig, "Last Name", "First Name", "Middle Name", "Middle Initial", "Birth Date", "Loan Type", "Loan Date", "Loan Amount", "Penalty", "Amount Due", "Monthly Amortization", "AMPSDG", "Status", "Effective Date" });
                    excelLines.Add(new List<string> { String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Format("{0:n}", loanRecords.Sum(sr => sr.Loan.PrincipalAmount.GetValueOrDefault())), String.Empty, String.Format("{0:n}", loanRecords.Sum(sr => sr.Loan.RemainingBalanceForDisplay.GetValueOrDefault())), String.Format("{0:n}", loanRecords.Sum(sr => sr.DeductionAmount.GetValueOrDefault())), String.Empty, String.Empty, String.Empty });

                    var reportFileContent = _excelBuilder.BuildExcelFile(excelLines);

                    var reportFileNameBuilder = new StringBuilder(64)
                        .Append($"{loanType.Code} Report - ")
                        .Append(query.ClientId == -1 ? "All Clients" : clients.Single().Name)
                        .Append(" - ")
                        .Append(query.PayrollPeriodMonth == -1 ? "All Payroll Period Months" : $"{(Month)query.PayrollPeriodMonth.Value}")
                        .Append(".xlsx");

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
                        PayrollPeriodMonthMonth = payrollPeriodMonth,
                        PayrollPeriodYear = query.PayrollPeriodYear,
                        SSSOrPagibigHeader = query.LoanTypeId == HDMFCALL_ID || query.LoanTypeId == HDMFL_ID ? "Employee PagIbig No." : "Employee SSS No."
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
                            .AsNoTracking()
                            .Include(l => l.Employee)
                            .Include(l => l.LoanType)
                            .Where(l => l.LoanTypeId == query.LoanTypeId && !l.DeletedOn.HasValue && clientEmployeeIds.Contains(l.EmployeeId.Value) && DbFunctions.TruncateTime(l.StartDeductionDate) <= DbFunctions.TruncateTime(payrollProcessBatch.PayrollPeriodTo))
                            .ToListAsync();

                        loans = loans.Where(l => l.LoanPayrollPeriods.Contains(payrollProcessBatch.PayrollPeriod.Value)).ToList();

                        allLoanDeductions.AddRange(loans.Select(l => new LoanDeduction { Loan = l, DeductionAmount = l.DeductionAmount }));
                    }
                    else
                    {
                        var payrollRecordIds = payrollProcessBatch.PayrollRecords.Select(pr => pr.Id).ToList();

                        var loanDeductions = await _db.LoanDeductions
                            .AsNoTracking()
                            .Include(ld => ld.Loan)
                            .Include(ld => ld.Loan.Employee)
                            .Include(ld => ld.Loan.LoanType)
                            .Where(ld => ld.PayrollRecordId.HasValue && payrollRecordIds.Contains(ld.PayrollRecordId.Value) && ld.Loan.EmployeeId.HasValue && clientEmployeeIds.Contains(ld.Loan.EmployeeId.Value) && ld.Loan.LoanTypeId == query.LoanTypeId)
                            .ToListAsync();

                        allLoanDeductions.AddRange(loanDeductions);
                    }
                }

                return allLoanDeductions
                    .OrderBy(l => l.Loan.Employee.LastName)
                    .ThenBy(l => l.Loan.Employee.FirstName)
                    .Select(l => new QueryResult.LoanRecord
                    {
                        Loan = l.Loan,
                        SSSOrPagIbigNumber = l.Loan.LoanTypeId == HDMFCALL_ID || l.Loan.LoanTypeId == HDMFL_ID ? l.Loan.Employee.PagIbig : l.Loan.Employee.SSS,
                        DeductionAmount = l.DeductionAmount
                    })
                    .ToList();
            }
        }
    }
}
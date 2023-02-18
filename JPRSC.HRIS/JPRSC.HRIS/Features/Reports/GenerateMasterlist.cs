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
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace JPRSC.HRIS.Features.Reports
{
    public class GenerateMasterlist
    {
        public class Query : IRequest<QueryResult>
        {
            public int? ClientId { get; set; }
            public string Destination { get; set; }
            public string DisplayMode { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(q => q.ClientId)
                    .GreaterThan(0);
            }
        }

        public class QueryResult
        {
            public int? ClientId { get; set; }
            public string ClientName { get; set; }
            public string DisplayMode { get; set; }
            public byte[] FileContent { get; set; }
            public string Filename { get; set; }
            public IList<Employee> Employees { get; set; } = new List<Employee>();

            public class Employee
            {
                public AccountType? AccountType { get; set; }
                public DateTime AddedOn { get; set; }
                public string ATMAccountNumber { get; set; }
                public string CelNo { get; set; }
                public Citizenship? Citizenship { get; set; }
                public string CityAddress { get; set; }
                public CivilStatus? CivilStatus { get; set; }
                //public Client Client { get; set; }
                public int? ClientId { get; set; }
                public decimal? COLADaily { get; set; }
                public decimal? COLAHourly { get; set; }
                public decimal? COLAMonthly { get; set; }
                //public Company Company { get; set; }
                public int? CompanyId { get; set; }
                public decimal? DailyRate { get; set; }
                //public ICollection<DailyTimeRecord> DailyTimeRecords { get; set; } = new List<DailyTimeRecord>();
                public DateTime? DateHired { get; set; }
                public DateTime? DateOfBirth { get; set; }
                public DateTime? DateResigned { get; set; }
                public DateTime? DeletedOn { get; set; }
                public Department Department { get; set; }
                public int? DepartmentId { get; set; }
                public string Email { get; set; }
                public string EmployeeCode { get; set; }
                public string EmployeeStatus { get; set; }
                public string FirstName { get; set; }
                public Gender? Gender { get; set; }
                public decimal? HourlyRate { get; set; }
                public int Id { get; set; }
                public bool? IsActive { get; set; }
                public string LastName { get; set; }
                public bool? LoanExempt { get; set; }
                //public ICollection<Loan> Loans { get; set; } = new List<Loan>();
                public string MiddleName { get; set; }
                public DateTime? ModifiedOn { get; set; }
                public decimal? MonthlyRate { get; set; }
                public string Nickname { get; set; }
                //public ICollection<Overtime> Overtimes { get; set; } = new List<Overtime>();
                //public PagIbigRecord PagIbigRecord { get; set; }
                public int? PagIbigRecordId { get; set; }
                public string PagIbig { get; set; }
                public bool? PagIbigExempt { get; set; }
                public string PhilHealth { get; set; }
                public bool? PhilHealthExempt { get; set; }
                public string Position { get; set; }
                public Region? Region { get; set; }
                //public ICollection<RehireTransferEvent> RehireTransferEvents { get; set; } = new List<RehireTransferEvent>();
                //public Religion Religion { get; set; }
                public int? ReligionId { get; set; }
                public string ResignStatus { get; set; }
                public string SalaryStatus { get; set; }
                public string SSS { get; set; }
                public bool? SSSExempt { get; set; }
                public bool? TaxExempt { get; set; }
                //public TaxStatus TaxStatus { get; set; }
                public int? TaxStatusId { get; set; }
                public string TelNo { get; set; }
                public bool? ThirteenthMonthExempt { get; set; }
                public string TIN { get; set; }
                public string ZipCode { get; set; }

                public bool? SubmittedBiodata { get; set; }
                public bool? SubmittedIdPictures { get; set; }
                public bool? SubmittedNBIClearance { get; set; }
                public bool? SubmittedPoliceClearance { get; set; }
                public bool? SubmittedBarangayClearance { get; set; }
                public bool? SubmittedSSSIdOrED1Form { get; set; }
                public bool? SubmittedPhilHealthIdOrMDRForm { get; set; }
                public bool? SubmittedPagIbigIdOrMIDNo { get; set; }
                public bool? SubmittedTINIdOr1902Form { get; set; }
                public bool? SubmittedBirthCertificate { get; set; }
                public bool? SubmittedMarriageCertification { get; set; }
                public bool? SubmittedBirthCertificateOfChildren { get; set; }
                public bool? SubmittedDiplomaOrTCR { get; set; }
                public bool? SubmittedPreEmploymentMedicalResult { get; set; }
                public bool? SubmittedSSSLoanVerification { get; set; }

                //public JobTitle JobTitle { get; set; }
                public int? JobTitleId { get; set; }
                public string CompanyIdNumber { get; set; }
                public string PermanentAddress { get; set; }
                public string PlaceOfBirth { get; set; }

                //public string FullName { get; set; }

                public IList<string> DisplayLine
                {
                    get
                    {
                        var line = new List<string>();

                        //line.Add(SSS);
                        //line.Add(LastName);
                        //line.Add(FirstName);
                        //line.Add(String.IsNullOrWhiteSpace(MiddleName) ? null : MiddleName.Trim().First().ToString());
                        //line.Add(String.Format("{0}", Loan.LoanType.Code));
                        //line.Add(String.Format("{0:M/d/yyyy}", Loan.LoanDate));
                        //line.Add(String.Format("{0:n}", Loan.PrincipalAmount));
                        //line.Add(String.Empty);
                        //line.Add(String.Format("{0:n}", Loan.RemainingBalanceForDisplay));
                        //line.Add(String.Format("{0:n}", Loan.DeductionAmount));
                        //line.Add(String.Format("{0:n}", AmountDeducted));
                        //line.Add(String.Format("{0:M/d/yyyy}", DateDeducted));
                        //line.Add(String.Format("{0:n}", Loan.AmountPaid));
                        //line.Add(String.Empty);
                        //line.Add(String.Empty);
                        //line.Add(String.Empty);

                        return line;
                    }
                }
            }

            public class Department
            {
                public int Id { get; set; }
                public string Code { get; set; }
                public string Name { get; set; }
            }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Employee, QueryResult.Employee>();
                CreateMap<Department, QueryResult.Department>();
            }
        }

        public class QueryHandler : IRequestHandler<Query, QueryResult>
        {
            private readonly ApplicationDbContext _db;
            private readonly IExcelBuilder _excelBuilder;
            private readonly IMapper _mapper;

            public QueryHandler(ApplicationDbContext db, IExcelBuilder excelBuilder, IMapper mapper)
            {
                _db = db;
                _excelBuilder = excelBuilder;
                _mapper = mapper;
            }

            public async Task<QueryResult> Handle(Query query, CancellationToken cancellationToken)
            {
                var clients = query.ClientId == -1 ?
                    await _db.Clients.Where(c => !c.DeletedOn.HasValue).ToListAsync() :
                    await _db.Clients.Where(c => !c.DeletedOn.HasValue && c.Id == query.ClientId.Value).ToListAsync();

                var employees = await _db.Employees
                    .AsNoTracking()
                    .Include(e => e.Department)
                    .ProjectTo<QueryResult.Employee>(_mapper)
                    .ToListAsync();

                if (query.Destination == "Excel")
                {
                    var excelLines = employees.Select(pr => pr.DisplayLine).ToList();
                    //excelLines.Insert(0, new List<string> { "Company PagIbig No.", String.Empty, "Employee PagIbig No.", "Last Name", "First Name", String.Empty, String.Empty, "Middle Initial", "Net pay", String.Empty, "Date Generated", String.Empty, "PagIbig Employer Share", "PagIbig Employee Share", "Share Total" });
                    //excelLines.Add(new List<string> { String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Format("{0:n}", pagIbigRecords.Sum(sr => sr.PagIbigDeductionBasis)), String.Empty, String.Empty, String.Empty, String.Format("{0:n}", pagIbigRecords.Sum(sr => sr.TotalPagIbigEmployer)), String.Format("{0:n}", pagIbigRecords.Sum(sr => sr.TotalPagIbigEmployee)), String.Format("{0:n}", pagIbigRecords.Sum(sr => sr.ShareTotal)) });

                    var reportFileContent = _excelBuilder.BuildExcelFile(excelLines);

                    var reportFileNameBuilder = new StringBuilder(64)
                        .Append($"Masterlist Report - ")
                        .Append(query.ClientId == -1 ? "All Clients" : clients.Single().Name)
                        .Append(".xlsx");

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

                    return new QueryResult
                    {
                        ClientId = query.ClientId,
                        ClientName = clientName,
                        DisplayMode = query.DisplayMode,
                        Employees = employees
                    };
                }
            }
        }
    }
}

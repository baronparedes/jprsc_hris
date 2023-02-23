using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Infrastructure.Excel;
using JPRSC.HRIS.Infrastructure.NET;
using JPRSC.HRIS.Models;
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
            public IList<Table> Tables { get; set; } = new List<Table>();
            public Query Query { get; set; }

            public class Employee
            {
                public AccountType? AccountType { get; set; }
                public DateTime AddedOn { get; set; }
                public string ATMAccountNumber { get; set; }
                public string CelNo { get; set; }
                public Citizenship? Citizenship { get; set; }
                public string CityAddress { get; set; }
                public CivilStatus? CivilStatus { get; set; }
                public Client Client { get; set; }
                public decimal? COLADaily { get; set; }
                public decimal? COLAHourly { get; set; }
                public decimal? COLAMonthly { get; set; }
                public Company Company { get; set; }
                public decimal? DailyRate { get; set; }
                public DateTime? DateHired { get; set; }
                public DateTime? DateOfBirth { get; set; }
                public DateTime? DateResigned { get; set; }
                public DateTime? DeletedOn { get; set; }
                public Department Department { get; set; }
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
                public string MiddleName { get; set; }
                public DateTime? ModifiedOn { get; set; }
                public decimal? MonthlyRate { get; set; }
                public string Nickname { get; set; }
                public PagIbigRecord PagIbigRecord { get; set; }
                public string PagIbig { get; set; }
                public bool? PagIbigExempt { get; set; }
                public string PhilHealth { get; set; }
                public bool? PhilHealthExempt { get; set; }
                public string Position { get; set; }
                public Region? Region { get; set; }
                public Religion Religion { get; set; }
                public string ResignStatus { get; set; }
                public string SalaryStatus { get; set; }
                public string SSS { get; set; }
                public bool? SSSExempt { get; set; }
                public bool? TaxExempt { get; set; }
                public TaxStatus TaxStatus { get; set; }
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

                public JobTitle JobTitle { get; set; }
                //public int? JobTitleId { get; set; }
                public string CompanyIdNumber { get; set; }
                public string PermanentAddress { get; set; }
                public string PlaceOfBirth { get; set; }
            }

            public class Department
            {
                public int Id { get; set; }
                public string Code { get; set; }
                public string Name { get; set; }
            }

            public class Client
            {
                public string Code { get; set; }
                public int Id { get; set; }
                public string Name { get; set; }
            }

            public class Company
            {
                public string Code { get; set; }
                public int Id { get; set; }
                public string Name { get; set; }
            }

            public class PagIbigRecord
            {
                public string Code { get; set; }
                public int Id { get; set; }
                public string Name { get; set; }
            }

            public class Religion
            {
                public string Code { get; set; }
                public int Id { get; set; }
                public string Description { get; set; }
            }

            public class TaxStatus
            {
                public string Code { get; set; }
                public int Id { get; set; }
                public string Name { get; set; }
            }

            public class JobTitle
            {
                public int Id { get; set; }
                public string Name { get; set; }
            }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Employee, QueryResult.Employee>();
                CreateMap<Department, QueryResult.Department>();
                CreateMap<Client, QueryResult.Client>();
                CreateMap<Company, QueryResult.Company>();
                CreateMap<PagIbigRecord, QueryResult.PagIbigRecord>();
                CreateMap<Religion, QueryResult.Religion>();
                CreateMap<TaxStatus,QueryResult.TaxStatus>();
                CreateMap<JobTitle, QueryResult.JobTitle>();
            }
        }

        public class PageTableBuilder : TableBuilder<QueryResult.Employee>
        {
            public PageTableBuilder()
            {
                Column("Employee Code", e => e.EmployeeCode);
                Column("First Name", e => e.FirstName);
                Column("Middle Name", e => e.MiddleName);
                Column("Last Name", e => e.LastName);
                Column("Nickname", e => e.Nickname);
                Column("Client", e => e.Client?.Name);
                Column("Account Type", e => e.AccountType.HasValue ? EnumHelper.GetDisplayName(e.AccountType.Value) : String.Empty);
                Column("ATM Account Number", e => e.ATMAccountNumber);
                Column("Cell No", e => e.CelNo);
                Column("Tel No", e => e.TelNo);
                Column("Citizenship", e => e.Citizenship.HasValue ? EnumHelper.GetDisplayName(e.Citizenship.Value) : String.Empty);
                Column("City Address", e => e.CityAddress);
                Column("Permanent Address", e => e.PermanentAddress);
                Column("Place of Birth", e => e.PlaceOfBirth);
                Column("Zip Code", e => e.ZipCode);
                Column("Civil Status", e => e.CivilStatus.HasValue ? EnumHelper.GetDisplayName(e.CivilStatus.Value) : String.Empty);
                Column("COLA Daily", e => e.COLADaily.HasValue ? $"{e.COLADaily:n}" : String.Empty);
                Column("COLA Hourly", e => e.COLAHourly.HasValue ? $"{e.COLAHourly:n}" : String.Empty);
                Column("COLA Monthly", e => e.COLAMonthly.HasValue ? $"{e.COLAMonthly:n}" : String.Empty);
                Column("Company", e => e.Company?.Name);
                Column("Daily Rate", e => e.DailyRate.HasValue ? $"{e.DailyRate:n}" : String.Empty);
                Column("Hourly Rate", e => e.HourlyRate.HasValue ? $"{e.HourlyRate:n}" : String.Empty);
                Column("Monthly Rate", e => e.MonthlyRate.HasValue ? $"{e.MonthlyRate:n}" : String.Empty);
                Column("Date Hired", e => e.DateHired.HasValue ? $"{e.DateHired.Value:MM/dd/yyyy}" : String.Empty);
                Column("Date of Birth", e => e.DateOfBirth.HasValue ? $"{e.DateOfBirth.Value:MM/dd/yyyy}" : String.Empty);
                Column("Date Resigned", e => e.DateResigned.HasValue ? $"{e.DateResigned.Value:MM/dd/yyyy}" : String.Empty);
                Column("DepartMent", e => e.Department?.Name);
                Column("Email", e => e.Email);
                Column("Employee Status", e => e.EmployeeStatus);
                Column("Gender", e => e.Gender.HasValue ? EnumHelper.GetDisplayName(e.Gender.Value) : String.Empty);
                Column("Id", e => e.Id.ToString());
                Column("PagIbig Record", e => e.PagIbigRecord?.Name);
                Column("Position", e => e.Position);
                Column("Region", e => e.Region.HasValue ? EnumHelper.GetDisplayName(e.Region.Value) : String.Empty);                
                Column("Religion", e => e.Religion?.Code);
                Column("Resign Status", e => e.ResignStatus);
                Column("Salary Status", e => e.SalaryStatus);
                Column("Tax Status", e => e.TaxStatus.Name);
                Column("Job Title", e => e.JobTitle?.Name);
                Column("Company Id Number", e => e.CompanyIdNumber);
                Column("PagIbig", e => e.PagIbig);
                Column("PhilHealth", e => e.PhilHealth);
                Column("SSS", e => e.SSS);
                Column("TIN", e => e.TIN);
                Column("Pag Ibig Exempt", e => e.PagIbigExempt.HasValue ? e.PagIbigExempt.Value.ToString() : String.Empty);
                Column("Loan Exempt", e => e.LoanExempt.HasValue ? e.LoanExempt.Value.ToString() : String.Empty);
                Column("PhilHealth Exempt", e => e.PhilHealthExempt.HasValue ? e.PhilHealthExempt.Value.ToString() : String.Empty);
                Column("Thirteenth Month Exempt", e => e.ThirteenthMonthExempt.HasValue ? e.ThirteenthMonthExempt.Value.ToString() : String.Empty);
                Column("SSS Exempt", e => e.SSSExempt.HasValue ? e.SSSExempt.Value.ToString() : String.Empty);
                Column("Tax Exempt", e => e.TaxExempt.HasValue ? e.TaxExempt.Value.ToString() : String.Empty);
                Column("Submitted Biodata", e => e.SubmittedBiodata.HasValue ? e.SubmittedBiodata.Value.ToString() : String.Empty);
                Column("Submitted Id Pictures", e => e.SubmittedIdPictures.HasValue ? e.SubmittedIdPictures.Value.ToString() : String.Empty);
                Column("Submitted NBI Clearance", e => e.SubmittedNBIClearance.HasValue ? e.SubmittedNBIClearance.Value.ToString() : String.Empty);
                Column("Submitted Police Clearance", e => e.SubmittedPoliceClearance.HasValue ? e.SubmittedPoliceClearance.Value.ToString() : String.Empty);
                Column("Submitted Barangay Clearance", e => e.SubmittedBarangayClearance.HasValue ? e.SubmittedBarangayClearance.Value.ToString() : String.Empty);
                Column("Submitted SSS Id or ED1 Form", e => e.SubmittedSSSIdOrED1Form.HasValue ? e.SubmittedSSSIdOrED1Form.Value.ToString() : String.Empty);
                Column("Submitted PhilHealth Id or MDR Form", e => e.SubmittedPhilHealthIdOrMDRForm.HasValue ? e.SubmittedPhilHealthIdOrMDRForm.Value.ToString() : String.Empty);
                Column("Submitted PagIbig Id or MID No", e => e.SubmittedPagIbigIdOrMIDNo.HasValue ? e.SubmittedPagIbigIdOrMIDNo.Value.ToString() : String.Empty);
                Column("Submitted TIN Id or 1902 Form", e => e.SubmittedTINIdOr1902Form.HasValue ? e.SubmittedTINIdOr1902Form.Value.ToString() : String.Empty);
                Column("Submitted Birth Certificate", e => e.SubmittedBirthCertificate.HasValue ? e.SubmittedBirthCertificate.Value.ToString() : String.Empty);
                Column("Submitted Marriage Certificate", e => e.SubmittedMarriageCertification.HasValue ? e.SubmittedMarriageCertification.Value.ToString() : String.Empty);
                Column("Submitted Birth Certificate of Children", e => e.SubmittedBirthCertificateOfChildren.HasValue ? e.SubmittedBirthCertificateOfChildren.Value.ToString() : String.Empty);
                Column("Submitted Diploma or TCR", e => e.SubmittedDiplomaOrTCR.HasValue ? e.SubmittedDiplomaOrTCR.Value.ToString() : String.Empty);
                Column("Submitted Pre-employment Medical Result", e => e.SubmittedPreEmploymentMedicalResult.HasValue ? e.SubmittedPreEmploymentMedicalResult.Value.ToString() : String.Empty);
                Column("Submitted SSS Locan Verification", e => e.SubmittedSSSLoanVerification.HasValue ? e.SubmittedSSSLoanVerification.Value.ToString() : String.Empty);
                Column("Added On", e => $"{e.AddedOn:MM/dd/yyyy}");
                Column("Deleted On", e => e.DeletedOn.HasValue ? $"{e.DeletedOn.Value:MM/dd/yyyy}" : String.Empty);
                Column("Modified On", e => e.ModifiedOn.HasValue ? $"{e.ModifiedOn.Value:MM/dd/yyyy}" : String.Empty);
                Column("Is Active", e => e.IsActive.HasValue ? e.IsActive.Value.ToString() : String.Empty);
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
                    .Include(e => e.Client)
                    .Include(e => e.Company)
                    .Include(e => e.Department)
                    .Include(e => e.JobTitle)
                    .Include(e => e.PagIbigRecord)
                    .Include(e => e.Religion)
                    .Include(e => e.TaxStatus)
                    .ProjectTo<QueryResult.Employee>(_mapper)
                    .ToListAsync();

                var reportFileNameBase = new StringBuilder(64)
                    .Append($"Masterlist Report - ")
                    .Append(query.ClientId == -1 ? "All Clients" : clients.Single().Name);

                if (query.Destination == "Excel")
                {
                    var excelTableBuilder = new ExcelTableBuilder();
                    var excelTable = excelTableBuilder.Build(employees);

                    return new QueryResult
                    {
                        FileContent = _excelBuilder.BuildExcelFile(excelTable.AllLines),
                        Filename = reportFileNameBase.Append(".xlsx").ToString()
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
                        var pageTable = pageTableBuilder.Build(employees);
                        tables.Add(pageTable);
                    }
                    else if (query.DisplayMode == "Branch")
                    {
                        var pageTableBuilders = new List<PageTableBuilder>();

                        foreach (var employee in employees.Where(e => e.Department == null))
                        {
                            employee.Department = new QueryResult.Department { Id = 0, Name = "No Branch" };
                        }

                        var groupedByBranch = employees.GroupBy(e => new { e.Department.Id, e.Department.Name });

                        foreach (var group in groupedByBranch.OrderBy(g => g.Key.Name))
                        {
                            var pageTableBuilder = new PageTableBuilder(group.Key.Name);
                            var pageTable = pageTableBuilder.Build(group);
                            tables.Add(pageTable);
                        }
                    }

                    return new QueryResult
                    {
                        ClientId = query.ClientId,
                        ClientName = clientName,
                        DisplayMode = query.DisplayMode,
                        Employees = employees,
                        Query = query,
                        Tables = tables
                    };
                }
            }
        }
    }
}

using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Dependency;
using JPRSC.HRIS.WebApp.Infrastructure.Security;
using MediatR;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.Employees
{
    public class Add
    {
        public class Query : IRequest<Command>
        {
        }

        public class Command : IRequest
        {
            public IList<SelectListItem> ReligionsList { get; set; } = new List<SelectListItem>();
            public IList<SelectListItem> ClientsList { get; set; } = new List<SelectListItem>();
            public IList<SelectListItem> DepartmentsList { get; set; } = new List<SelectListItem>();
            public IList<SelectListItem> TaxStatusesList { get; set; } = new List<SelectListItem>();
            public IList<SelectListItem> JobTitlesList { get; set; } = new List<SelectListItem>();

            // Employee Info
            public string CompanyIdNumber { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string Nickname { get; set; }
            public string Email { get; set; }
            public string CityAddress { get; set; }
            public string PermanentAddress { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string PlaceOfBirth { get; set; }
            public string ZipCode { get; set; }
            public string TelNo { get; set; }
            public string CelNo { get; set; }
            public Gender? Gender { get; set; }
            public Religion Religion { get; set; }
            public int? ReligionId { get; set; }
            public Citizenship? Citizenship { get; set; }
            public CivilStatus? CivilStatus { get; set; }
            public string SSS { get; set; }
            public string TIN { get; set; }
            public string PagIbig { get; set; }
            public string PhilHealth { get; set; }

            // Company Info
            public Client Client { get; set; }
            public int? ClientId { get; set; }
            public DateTime? DateHired { get; set; }
            public DateTime? DateResigned { get; set; }
            public JobTitle JobTitle { get; set; }
            public int? JobTitleId { get; set; }
            public Department Department { get; set; }
            public int? DepartmentId { get; set; }
            public Region? Region { get; set; }
            public string EmployeeStatus { get; set; }
            public string ResignStatus { get; set; }
            public bool? IsActive { get; set; } = true;

            // Pay Info
            public string ATMAccountNumber { get; set; }
            public AccountType? AccountType { get; set; }
            public TaxStatus TaxStatus { get; set; }
            public int? TaxStatusId { get; set; }
            public decimal? HourlyRate { get; set; }
            public decimal? DailyRate { get; set; }
            public decimal? COLAHourly { get; set; }
            public decimal? COLADaily { get; set; }
            public decimal? COLAMonthly { get; set; }
            public decimal? MonthlyRate { get; set; }
            public bool? TaxExempt { get; set; }
            public bool? PagIbigExempt { get; set; }
            public bool? ThirteenthMonthExempt { get; set; }
            public bool? PhilHealthExempt { get; set; }
            public string SalaryStatus { get; set; }

            // Submitted Files
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
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<Command> Handle(Query query, CancellationToken token)
            {
                var command = new Command();

                command.ReligionsList = await GetReligionsList();
                command.ClientsList = await GetClientsList();
                command.DepartmentsList = await GetDepartmentsList();
                command.TaxStatusesList = await GetTaxStatusesList();
                command.JobTitlesList = await GetJobTitlesList();

                return command;
            }

            private async Task<IList<SelectListItem>> GetReligionsList()
            {
                var religions = await _db.Religions.Where(r => !r.DeletedOn.HasValue).ToListAsync();

                return religions
                    .Select(r => new SelectListItem
                    {
                        Text = r.Code,
                        Value = r.Id.ToString()
                    })
                    .ToList();
            }

            private async Task<IList<SelectListItem>> GetClientsList()
            {
                var clients = await _db.Clients.Where(c => !c.DeletedOn.HasValue).ToListAsync();

                return clients
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    })
                    .ToList();
            }

            private async Task<IList<SelectListItem>> GetDepartmentsList()
            {
                var departments = await _db.Departments.Where(d => !d.DeletedOn.HasValue).ToListAsync();

                return departments
                    .Select(d => new SelectListItem
                    {
                        Text = d.Name,
                        Value = d.Id.ToString()
                    })
                    .ToList();
            }

            private async Task<IList<SelectListItem>> GetTaxStatusesList()
            {
                var taxStatuses = await _db.TaxStatuses.Where(ts => !ts.DeletedOn.HasValue).ToListAsync();

                return taxStatuses
                    .Select(ts => new SelectListItem
                    {
                        Text = ts.Code,
                        Value = ts.Id.ToString()
                    })
                    .ToList();
            }

            private async Task<IList<SelectListItem>> GetJobTitlesList()
            {
                var jobTitles = await _db.JobTitles.Where(d => !d.DeletedOn.HasValue).ToListAsync();

                return jobTitles
                    .Select(d => new SelectListItem
                    {
                        Text = d.Name,
                        Value = d.Id.ToString()
                    })
                    .ToList();
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly ApplicationDbContext _db = DependencyConfig.Instance.Container.GetInstance<ApplicationDbContext>();

            public CommandValidator()
            {
                RuleFor(c => c.FirstName)
                    .NotEmpty();

                RuleFor(c => c.LastName)
                    .NotEmpty();

                RuleFor(c => c.Email)
                    .EmailAddress()
                    .When(c => !String.IsNullOrWhiteSpace(c.Email));

                When(c => !String.IsNullOrWhiteSpace(c.CompanyIdNumber), () =>
                {
                    RuleFor(c => c.CompanyIdNumber)
                        .Must(BeUniqueCompanyIdNumber)
                        .WithMessage("Company Id {PropertyValue} is already taken.");
                });

                When(c => !String.IsNullOrWhiteSpace(c.SSS), () =>
                {
                    RuleFor(c => c.SSS)
                        .Must(BeUniqueSSS)
                        .WithMessage("SSS {PropertyValue} is already taken.");
                });

                When(c => !String.IsNullOrWhiteSpace(c.TIN), () =>
                {
                    RuleFor(c => c.TIN)
                        .Must(BeUniqueTIN)
                        .WithMessage("TIN {PropertyValue} is already taken.");
                });

                When(c => !String.IsNullOrWhiteSpace(c.PhilHealth), () =>
                {
                    RuleFor(c => c.PhilHealth)
                        .Must(BeUniquePhilHealth)
                        .WithMessage("PhilHealth {PropertyValue} is already taken.");
                });

                When(c => !String.IsNullOrWhiteSpace(c.PagIbig), () =>
                {
                    RuleFor(c => c.PagIbig)
                        .Must(BeUniquePagIbig)
                        .WithMessage("PagIbig {PropertyValue} is already taken.");
                });

                When(c => !String.IsNullOrWhiteSpace(c.ATMAccountNumber) && c.ATMAccountNumber != "0", () =>
                {
                    RuleFor(c => c.ATMAccountNumber)
                        .Must(BeUniqueATMAccountNumber)
                        .WithMessage("ATM Account Number {PropertyValue} is already taken.");
                });
            }

            private bool BeUniquePagIbig(string pagIbig)
            {
                return !_db.Employees.Any(e => e.PagIbig == pagIbig);
            }

            private bool BeUniquePhilHealth(string philHealth)
            {
                return !_db.Employees.Any(e => e.PhilHealth == philHealth);
            }

            private bool BeUniqueTIN(string tin)
            {
                return !_db.Employees.Any(e => e.TIN == tin);
            }

            private bool BeUniqueSSS(string sss)
            {
                return !_db.Employees.Any(e => e.SSS == sss);
            }

            private bool BeUniqueCompanyIdNumber(string companyIdNumber)
            {
                return !_db.Employees.Any(e => e.CompanyIdNumber == companyIdNumber);
            }

            private bool BeUniqueATMAccountNumber(string atmAccountNumber)
            {
                if (atmAccountNumber == "0") return true;

                return !_db.Employees.Any(e => e.ATMAccountNumber == atmAccountNumber);
            }
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command command, CancellationToken token)
            {
                var currentUserId = HttpContext.Current.User.Identity.GetUserId();
                var currentUser = await _db
                    .Users
                    .Include(u => u.Company)
                    .SingleAsync(u => u.Id == currentUserId);

                var employee = new Employee
                {
                    AccountType = command.AccountType,
                    AddedOn = DateTime.UtcNow,
                    CelNo = command.CelNo,
                    Citizenship = command.Citizenship,
                    CityAddress = command.CityAddress,
                    CivilStatus = command.CivilStatus,
                    ClientId = command.ClientId,
                    COLADaily = command.COLADaily.GetValueOrDefault(),
                    COLAHourly = command.COLAHourly.GetValueOrDefault(),
                    COLAMonthly = command.COLAMonthly.GetValueOrDefault(),
                    CompanyId = currentUser.Company?.Id,
                    CompanyIdNumber = command.CompanyIdNumber,
                    DailyRate = command.DailyRate.GetValueOrDefault(),
                    DateHired = command.DateHired,
                    DateOfBirth = command.DateOfBirth,
                    DateResigned = command.DateResigned,
                    DepartmentId = command.DepartmentId,
                    Email = command.Email,
                    EmployeeCode = await GetNextEmployeeCode(),
                    EmployeeStatus = command.EmployeeStatus,
                    FirstName = command.FirstName,
                    Gender = command.Gender,
                    HourlyRate = command.HourlyRate.GetValueOrDefault(),
                    IsActive = command.IsActive,
                    JobTitleId = command.JobTitleId,
                    LastName = command.LastName,
                    MiddleName = command.MiddleName,
                    MonthlyRate = command.MonthlyRate.GetValueOrDefault(),
                    Nickname = command.Nickname,
                    PagIbig = command.PagIbig,
                    PagIbigExempt = command.PagIbigExempt,
                    PermanentAddress = command.PermanentAddress,
                    PhilHealth = command.PhilHealth,
                    PhilHealthExempt = command.PhilHealthExempt,
                    PlaceOfBirth = command.PlaceOfBirth,
                    Region = command.Region,
                    ReligionId = command.ReligionId,
                    ResignStatus = command.ResignStatus,
                    SalaryStatus = command.SalaryStatus,
                    SSS = command.SSS,
                    SubmittedBarangayClearance = command.SubmittedBarangayClearance.GetValueOrDefault(),
                    SubmittedBirthCertificate = command.SubmittedBirthCertificate.GetValueOrDefault(),
                    SubmittedBirthCertificateOfChildren = command.SubmittedBirthCertificateOfChildren.GetValueOrDefault(),
                    SubmittedDiplomaOrTCR = command.SubmittedDiplomaOrTCR.GetValueOrDefault(),
                    SubmittedIdPictures = command.SubmittedIdPictures.GetValueOrDefault(),
                    SubmittedMarriageCertification = command.SubmittedMarriageCertification.GetValueOrDefault(),
                    SubmittedNBIClearance = command.SubmittedNBIClearance.GetValueOrDefault(),
                    SubmittedPagIbigIdOrMIDNo = command.SubmittedPagIbigIdOrMIDNo.GetValueOrDefault(),
                    SubmittedPhilHealthIdOrMDRForm = command.SubmittedPhilHealthIdOrMDRForm.GetValueOrDefault(),
                    SubmittedPoliceClearance = command.SubmittedPoliceClearance.GetValueOrDefault(),
                    SubmittedPreEmploymentMedicalResult = command.SubmittedPreEmploymentMedicalResult.GetValueOrDefault(),
                    SubmittedSSSIdOrED1Form = command.SubmittedSSSIdOrED1Form.GetValueOrDefault(),
                    SubmittedSSSLoanVerification = command.SubmittedSSSLoanVerification.GetValueOrDefault(),
                    SubmittedTINIdOr1902Form = command.SubmittedTINIdOr1902Form.GetValueOrDefault(),
                    SubmittedBiodata = command.SubmittedBiodata.GetValueOrDefault(),
                    TaxExempt = command.TaxExempt,
                    TaxStatusId = command.TaxStatusId,
                    TelNo = command.TelNo,
                    ThirteenthMonthExempt = command.ThirteenthMonthExempt,
                    TIN = command.TIN,
                    ZipCode = command.ZipCode
                };

                if (AuthorizeHelper.IsAuthorized(Permission.EmployeeEditATM))
                {
                    employee.ATMAccountNumber = command.ATMAccountNumber;
                }

                _db.Employees.Add(employee);
                await _db.SaveChangesAsync();

                return Unit.Value;
            }

            private async Task<string> GetNextEmployeeCode()
            {
                var employeeCodes = await _db
                    .Employees
                    .Where(e => !e.DeletedOn.HasValue && e.EmployeeCode != null)
                    .Select(e => e.EmployeeCode.Trim())
                    .ToListAsync();

                if (!employeeCodes.Any()) return "0001";

                var employeeCodeNumbers = new List<int>(employeeCodes.Count);

                foreach (var employeeCode in employeeCodes.Where(ec => !String.IsNullOrWhiteSpace(ec)))
                {
                    if (int.TryParse(employeeCode, out int employeeCodeNumber))
                    {
                        employeeCodeNumbers.Add(employeeCodeNumber);
                    }
                }

                var maxEmployeeCode = employeeCodeNumbers
                    .Max();

                return (maxEmployeeCode + 1).ToString(maxEmployeeCode + 1 < 1000 ? "D4" : null);
            }
        }
    }
}
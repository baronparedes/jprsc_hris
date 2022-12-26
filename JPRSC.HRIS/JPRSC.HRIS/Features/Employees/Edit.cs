using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Infrastructure.Security;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.Features.Employees
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int EmployeeId { get; set; }
        }

        public class Command : IRequest
        {
            public IList<SelectListItem> ReligionsList { get; set; } = new List<SelectListItem>();
            public IList<SelectListItem> ClientsList { get; set; } = new List<SelectListItem>();
            public IList<SelectListItem> DepartmentsList { get; set; } = new List<SelectListItem>();
            public IList<SelectListItem> TaxStatusesList { get; set; } = new List<SelectListItem>();
            public IList<SelectListItem> JobTitlesList { get; set; } = new List<SelectListItem>();
            public IList<SelectListItem> PagIbigRecordsList { get; set; } = new List<SelectListItem>();

            public string EmployeeCode { get; set; }
            public string CompanyIdNumber { get; set; }
            public int Id { get; set; }

            // Employee Info
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
            public bool? IsActive { get; set; }

            // Pay Info
            public string ATMAccountNumber { get; set; }
            public AccountType? AccountType { get; set; }
            public TaxStatus TaxStatus { get; set; }
            public int? TaxStatusId { get; set; }
            public PagIbigRecord PagIbigRecord { get; set; }
            public int? PagIbigRecordId { get; set; }
            public decimal? HourlyRate { get; set; }
            public decimal? DailyRate { get; set; }
            public decimal? COLAHourly { get; set; }
            public decimal? COLADaily { get; set; }
            public decimal? COLAMonthly { get; set; }
            public decimal? MonthlyRate { get; set; }
            public bool? SSSExempt { get; set; }
            public bool? TaxExempt { get; set; }
            public bool? PagIbigExempt { get; set; }
            public bool? ThirteenthMonthExempt { get; set; }
            public bool? PhilHealthExempt { get; set; }
            public bool? LoanExempt { get; set; }
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

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Employee, Command>();
            }
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly ApplicationDbContext _db;
            private readonly IMapper _mapper;

            public QueryHandler(ApplicationDbContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Command> Handle(Query query, CancellationToken token)
            {
                var command = await _db.Employees.Where(r => r.Id == query.EmployeeId && !r.DeletedOn.HasValue).ProjectTo<Command>(_mapper).SingleAsync();
                var employee = await _db.Employees.SingleAsync(e => e.Id == query.EmployeeId);

                command.ReligionsList = await GetReligionsList(query, employee);
                command.ClientsList = await GetClientsList(query, employee);
                command.DepartmentsList = await GetDepartmentsList(query, employee);
                command.TaxStatusesList = await GetTaxStatusesList(query, employee);
                command.JobTitlesList = await GetJobTitlesList(query, employee);
                command.PagIbigRecordsList = await GetPagIbigRecordsList(query, employee);

                return command;
            }

            private async Task<IList<SelectListItem>> GetReligionsList(Query query, Employee employee)
            {
                var religions = await _db.Religions.Where(r => !r.DeletedOn.HasValue).ToListAsync();

                return religions
                    .Select(r => new SelectListItem
                    {
                        Text = r.Code,
                        Value = r.Id.ToString(),
                        Selected = employee.ReligionId == r.Id
                    })
                    .ToList();
            }

            private async Task<IList<SelectListItem>> GetClientsList(Query query, Employee employee)
            {
                var clients = await _db.Clients.Where(c => !c.DeletedOn.HasValue).OrderBy(c => c.Name).ToListAsync();

                return clients
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                        Selected = employee.ClientId == c.Id
                    })
                    .ToList();
            }

            private async Task<IList<SelectListItem>> GetDepartmentsList(Query query, Employee employee)
            {
                var departments = await _db.Departments.Where(d => !d.DeletedOn.HasValue).ToListAsync();

                return departments
                    .Select(d => new SelectListItem
                    {
                        Text = d.Name,
                        Value = d.Id.ToString(),
                        Selected = employee.DepartmentId == d.Id
                    })
                    .ToList();
            }

            private async Task<IList<SelectListItem>> GetTaxStatusesList(Query query, Employee employee)
            {
                var taxStatuses = await _db.TaxStatuses.Where(ts => !ts.DeletedOn.HasValue).ToListAsync();

                return taxStatuses
                    .Select(ts => new SelectListItem
                    {
                        Text = ts.Code,
                        Value = ts.Id.ToString(),
                        Selected = employee.TaxStatusId == ts.Id
                    })
                    .ToList();
            }

            private async Task<IList<SelectListItem>> GetJobTitlesList(Query query, Employee employee)
            {
                var jobTitles = await _db.JobTitles.Where(d => !d.DeletedOn.HasValue).ToListAsync();

                return jobTitles
                    .Select(j => new SelectListItem
                    {
                        Text = j.Name,
                        Value = j.Id.ToString(),
                        Selected = employee.JobTitleId == j.Id
                    })
                    .ToList();
            }

            private async Task<IList<SelectListItem>> GetPagIbigRecordsList(Query query, Employee employee)
            {
                var pagIbigRecords = await _db.PagIbigRecords.Where(d => !d.DeletedOn.HasValue).ToListAsync();

                return pagIbigRecords
                    .Select(p => new SelectListItem
                    {
                        Text = p.Code,
                        Value = p.Id.ToString(),
                        Selected = employee.PagIbigRecordId == p.Id
                    })
                    .ToList();
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly ApplicationDbContext _db;

            public CommandValidator(ApplicationDbContext db)
            {
                _db = db;

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

                    RuleFor(c => c.ATMAccountNumber)
                        .Matches("^[a-zA-Z0-9]*$")
                        .WithMessage("Special characters are not allowed.");
                });
            }

            private bool BeUniquePagIbig(Command command, string pagIbig)
            {
                return !_db.Employees.Any(e => e.Id != command.Id && !e.DeletedOn.HasValue && e.PagIbig == pagIbig);
            }

            private bool BeUniquePhilHealth(Command command, string philHealth)
            {
                return !_db.Employees.Any(e => e.Id != command.Id && !e.DeletedOn.HasValue && e.PhilHealth == philHealth);
            }

            private bool BeUniqueTIN(Command command, string tin)
            {
                return !_db.Employees.Any(e => e.Id != command.Id && !e.DeletedOn.HasValue && e.TIN == tin);
            }

            private bool BeUniqueSSS(Command command, string sss)
            {
                return !_db.Employees.Any(e => e.Id != command.Id && !e.DeletedOn.HasValue && e.SSS == sss);
            }

            private bool BeUniqueCompanyIdNumber(Command command, string companyIdNumber)
            {
                return !_db.Employees.Any(e => e.Id != command.Id && !e.DeletedOn.HasValue && e.CompanyIdNumber == companyIdNumber);
            }

            private bool BeUniqueATMAccountNumber(Command command, string atmAccountNumber)
            {
                return !_db.Employees.Any(e => e.Id != command.Id && !e.DeletedOn.HasValue && e.ATMAccountNumber == atmAccountNumber);
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
                var employee = await _db.Employees.SingleAsync(r => r.Id == command.Id);

                employee.AccountType = command.AccountType;
                employee.CelNo = command.CelNo;
                employee.Citizenship = command.Citizenship;
                employee.CityAddress = command.CityAddress;
                employee.CivilStatus = command.CivilStatus;
                employee.COLADaily = command.COLADaily.GetValueOrDefault();
                employee.COLAHourly = command.COLAHourly.GetValueOrDefault();
                employee.COLAMonthly = command.COLAMonthly.GetValueOrDefault();
                employee.CompanyIdNumber = command.CompanyIdNumber;
                employee.DailyRate = command.DailyRate.GetValueOrDefault();
                employee.DateHired = command.DateHired;
                employee.DateOfBirth = command.DateOfBirth;
                employee.DateResigned = command.DateResigned;
                employee.DepartmentId = command.DepartmentId;
                employee.Email = command.Email;
                employee.EmployeeStatus = command.EmployeeStatus;
                employee.FirstName = command.FirstName;
                employee.Gender = command.Gender;
                employee.HourlyRate = command.HourlyRate.GetValueOrDefault();
                employee.IsActive = command.IsActive;
                employee.JobTitleId = command.JobTitleId;
                employee.LastName = command.LastName;
                employee.LoanExempt = command.LoanExempt;
                employee.MiddleName = command.MiddleName;
                employee.ModifiedOn = DateTime.UtcNow;
                employee.MonthlyRate = command.MonthlyRate.GetValueOrDefault();
                employee.Nickname = command.Nickname;
                employee.PagIbig = command.PagIbig;
                employee.PagIbigExempt = command.PagIbigExempt;
                employee.PagIbigRecordId = command.PagIbigRecordId;
                employee.PermanentAddress = command.PermanentAddress;
                employee.PhilHealth = command.PhilHealth;
                employee.PhilHealthExempt = command.PhilHealthExempt;
                employee.PlaceOfBirth = command.PlaceOfBirth;
                employee.Region = command.Region;
                employee.ReligionId = command.ReligionId;
                employee.ResignStatus = command.ResignStatus;
                employee.SalaryStatus = command.SalaryStatus;
                employee.SSS = command.SSS;
                employee.SSSExempt = command.SSSExempt;
                employee.SubmittedBarangayClearance = command.SubmittedBarangayClearance.GetValueOrDefault();
                employee.SubmittedBirthCertificate = command.SubmittedBirthCertificate.GetValueOrDefault();
                employee.SubmittedBirthCertificateOfChildren = command.SubmittedBirthCertificateOfChildren.GetValueOrDefault();
                employee.SubmittedDiplomaOrTCR = command.SubmittedDiplomaOrTCR.GetValueOrDefault();
                employee.SubmittedIdPictures = command.SubmittedIdPictures.GetValueOrDefault();
                employee.SubmittedMarriageCertification = command.SubmittedMarriageCertification.GetValueOrDefault();
                employee.SubmittedNBIClearance = command.SubmittedNBIClearance.GetValueOrDefault();
                employee.SubmittedPagIbigIdOrMIDNo = command.SubmittedPagIbigIdOrMIDNo.GetValueOrDefault();
                employee.SubmittedPhilHealthIdOrMDRForm = command.SubmittedPhilHealthIdOrMDRForm.GetValueOrDefault();
                employee.SubmittedPoliceClearance = command.SubmittedPoliceClearance.GetValueOrDefault();
                employee.SubmittedPreEmploymentMedicalResult = command.SubmittedPreEmploymentMedicalResult.GetValueOrDefault();
                employee.SubmittedSSSIdOrED1Form = command.SubmittedSSSIdOrED1Form.GetValueOrDefault();
                employee.SubmittedSSSLoanVerification = command.SubmittedSSSLoanVerification.GetValueOrDefault();
                employee.SubmittedTINIdOr1902Form = command.SubmittedTINIdOr1902Form.GetValueOrDefault();
                employee.SubmittedBiodata = command.SubmittedBiodata.GetValueOrDefault();
                employee.TaxExempt = command.TaxExempt;
                employee.TaxStatusId = command.TaxStatusId;
                employee.TelNo = command.TelNo;
                employee.ThirteenthMonthExempt = command.ThirteenthMonthExempt;
                employee.TIN = command.TIN;
                employee.ZipCode = command.ZipCode;

                if (AuthorizeHelper.IsAuthorized(_db, Permission.EmployeeEditATM))
                {
                    employee.ATMAccountNumber = command.ATMAccountNumber;
                }

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
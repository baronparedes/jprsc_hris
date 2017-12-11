using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string Nickname { get; set; }
            public string CityAddress { get; set; }
            public DateTime? DateOfBirth { get; set; }
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
            public string Email { get; set; }

            // Company Information
            public DateTime? DateHired { get; set; }
            public DateTime? DateResigned { get; set; }
            public Client Client { get; set; }
            public int? ClientId { get; set; }
            public string Position { get; set; }
            public Department Department { get; set; }
            public int? DepartmentId { get; set; }
            public TaxStatus TaxStatus { get; set; }
            public int? TaxStatusId { get; set; }
            public string EmployeeStatus { get; set; }
            public string PagIbig { get; set; }
            public string PhilHealth { get; set; }
            public AccountType? AccountType { get; set; }
            public string ATMAccountNumber { get; set; }
        }

        public class QueryHandler : IAsyncRequestHandler<Query, Command>
        {
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<Command> Handle(Query query)
            {
                var command = new Command();

                command.ReligionsList = await GetReligionsList();
                command.ClientsList = await GetClientsList();
                command.DepartmentsList = await GetDepartmentsList();
                command.TaxStatusesList = await GetTaxStatusesList();

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
                        Text = ts.Name,
                        Value = ts.Id.ToString()
                    })
                    .ToList();
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.FirstName)
                    .NotEmpty();

                RuleFor(c => c.LastName)
                    .NotEmpty();

                RuleFor(c => c.Email)
                    .EmailAddress()
                    .When(c => !String.IsNullOrWhiteSpace(c.Email));
            }
        }

        public class CommandHandler : IAsyncRequestHandler<Command>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task Handle(Command command)
            {
                var currentUserId = HttpContext.Current.User.Identity.GetUserId();
                var currentUser = await _db
                    .Users
                    .Include(u => u.CompanyProfile)
                    .SingleAsync(u => u.Id == currentUserId);

                var employee = new Employee
                {
                    AccountType = command.AccountType,
                    AddedOn = DateTime.UtcNow,
                    ATMAccountNumber = command.ATMAccountNumber,
                    CelNo = command.CelNo,
                    Citizenship = command.Citizenship,
                    CityAddress = command.CityAddress,
                    CivilStatus = command.CivilStatus,
                    ClientId = command.ClientId,
                    CompanyProfileId = currentUser.CompanyProfile?.Id,
                    DateHired = command.DateHired,
                    DateOfBirth = command.DateOfBirth,
                    DateResigned = command.DateResigned,
                    DepartmentId = command.DepartmentId,
                    Email = command.Email,
                    EmployeeCode = await GetNextEmployeeCode(),
                    EmployeeStatus = command.EmployeeStatus,
                    FirstName = command.FirstName,
                    Gender = command.Gender,
                    LastName = command.LastName,
                    MiddleName = command.MiddleName,
                    Nickname = command.Nickname,
                    PagIbig = command.PagIbig,
                    PhilHealth = command.PhilHealth,
                    Position = command.Position,
                    ReligionId = command.ReligionId,
                    SSS = command.SSS,
                    TaxStatusId = command.TaxStatusId,
                    TelNo = command.TelNo,
                    TIN = command.TIN,
                    ZipCode = command.ZipCode
                };

                _db.Employees.Add(employee);
                await _db.SaveChangesAsync();
            }

            private async Task<string> GetNextEmployeeCode()
            {
                var employeeCodes = await _db
                    .Employees
                    .Where(e => !e.DeletedOn.HasValue && e.EmployeeCode != null)
                    .Select(e => e.EmployeeCode)
                    .ToListAsync();

                if (!employeeCodes.Any()) return "0001";

                var maxEmployeeCode = employeeCodes
                    .ConvertAll(Convert.ToInt32)
                    .Max();

                return (maxEmployeeCode + 1).ToString("D4");
            }
        }
    }
}
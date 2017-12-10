using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Employees
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int EmployeeId { get; set; }
        }

        public class Command : IRequest
        {
            public int? EmployeeCode { get; set; }
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string MiddleName { get; set; }
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
                return await _db.Employees.Where(r => r.Id == query.EmployeeId && !r.DeletedOn.HasValue).ProjectToSingleAsync<Command>();
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
                var employee = await _db.Employees.SingleAsync(r => r.Id == command.Id);

                employee.AccountType = command.AccountType;
                employee.ATMAccountNumber = command.ATMAccountNumber;
                employee.CelNo = command.CelNo;
                employee.Citizenship = command.Citizenship;
                employee.CityAddress = command.CityAddress;
                employee.CivilStatus = command.CivilStatus;
                employee.ClientId = command.ClientId;
                employee.DateHired = command.DateHired;
                employee.DateOfBirth = command.DateOfBirth;
                employee.DateResigned = command.DateResigned;
                employee.DepartmentId = command.DepartmentId;
                employee.Email = command.Email;
                employee.EmployeeCode = command.EmployeeCode;
                employee.EmployeeStatus = command.EmployeeStatus;
                employee.FirstName = command.FirstName;
                employee.Gender = command.Gender;
                employee.LastName = command.LastName;
                employee.MiddleName = command.MiddleName;
                employee.ModifiedOn = DateTime.UtcNow;
                employee.Nickname = command.Nickname;
                employee.PagIbig = command.PagIbig;
                employee.PhilHealth = command.PhilHealth;
                employee.Position = command.Position;
                employee.ReligionId = command.ReligionId;
                employee.SSS = command.SSS;
                employee.TaxStatusId = command.TaxStatusId;
                employee.TelNo = command.TelNo;
                employee.TIN = command.TIN;
                employee.ZipCode = command.ZipCode;

                await _db.SaveChangesAsync();
            }
        }
    }
}
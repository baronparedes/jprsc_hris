using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

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
            public IList<SelectListItem> ReligionsList { get; set; } = new List<SelectListItem>();
            public IList<SelectListItem> ClientsList { get; set; } = new List<SelectListItem>();
            public IList<SelectListItem> DepartmentsList { get; set; } = new List<SelectListItem>();
            public IList<SelectListItem> TaxStatusesList { get; set; } = new List<SelectListItem>();

            public string EmployeeCode { get; set; }
            public int Id { get; set; }
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

            // Employee Rate
            public decimal? HourlyRate { get; set; }
            public decimal? DailyRate { get; set; }
            public decimal? COLAHourly { get; set; }
            public decimal? COLADaily { get; set; }
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
                var command = await _db.Employees.Where(r => r.Id == query.EmployeeId && !r.DeletedOn.HasValue).ProjectToSingleAsync<Command>();

                command.ReligionsList = await GetReligionsList(query);
                command.ClientsList = await GetClientsList(query);
                command.DepartmentsList = await GetDepartmentsList(query);
                command.TaxStatusesList = await GetTaxStatusesList(query);

                return command;
            }

            private async Task<IList<SelectListItem>> GetReligionsList(Query query)
            {
                var employee = await _db.Employees.SingleAsync(e => e.Id == query.EmployeeId);
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

            private async Task<IList<SelectListItem>> GetClientsList(Query query)
            {
                var employee = await _db.Employees.SingleAsync(e => e.Id == query.EmployeeId);
                var clients = await _db.Clients.Where(c => !c.DeletedOn.HasValue).ToListAsync();

                return clients
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                        Selected = employee.ClientId == c.Id
                    })
                    .ToList();
            }

            private async Task<IList<SelectListItem>> GetDepartmentsList(Query query)
            {
                var employee = await _db.Employees.SingleAsync(e => e.Id == query.EmployeeId);
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

            private async Task<IList<SelectListItem>> GetTaxStatusesList(Query query)
            {
                var employee = await _db.Employees.SingleAsync(e => e.Id == query.EmployeeId);
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
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.EmployeeCode)
                    .MustBeANumber();

                RuleFor(c => c.FirstName)
                    .NotEmpty();

                RuleFor(c => c.LastName)
                    .NotEmpty();

                RuleFor(c => c.Email)
                    .EmailAddress()
                    .When(c => !String.IsNullOrWhiteSpace(c.Email));
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
                employee.ATMAccountNumber = command.ATMAccountNumber;
                employee.CelNo = command.CelNo;
                employee.Citizenship = command.Citizenship;
                employee.CityAddress = command.CityAddress;
                employee.CivilStatus = command.CivilStatus;
                employee.ClientId = command.ClientId;
                employee.COLADaily = command.COLADaily;
                employee.COLAHourly = command.COLAHourly;
                employee.DailyRate = command.DailyRate;
                employee.DateHired = command.DateHired;
                employee.DateOfBirth = command.DateOfBirth;
                employee.DateResigned = command.DateResigned;
                employee.DepartmentId = command.DepartmentId;
                employee.Email = command.Email;
                employee.EmployeeCode = command.EmployeeCode;
                employee.EmployeeStatus = command.EmployeeStatus;
                employee.FirstName = command.FirstName;
                employee.Gender = command.Gender;
                employee.HourlyRate = command.HourlyRate;
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

                return Unit.Value;
            }
        }
    }
}
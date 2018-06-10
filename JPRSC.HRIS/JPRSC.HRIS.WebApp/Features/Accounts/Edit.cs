using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.WebApp.Infrastructure.Dependency;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.Accounts
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public string UserId { get; set; }
        }

        public class Command : IRequest
        {
            public int? CompanyId { get; set; }
            public string Name { get; set; }
            public string Id { get; set; }
            public int? JobTitleId { get; set; }
            public string UserName { get; set; }
            public IList<SelectListItem> RolesList { get; set; } = new List<SelectListItem>();
            public IList<SelectListItem> CompaniesList { get; set; } = new List<SelectListItem>();
            public IList<SelectListItem> JobTitlesList { get; set; } = new List<SelectListItem>();
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
                var command = await _db
                    .Users
                    .Where(u => u.Id == query.UserId && !u.DeletedOn.HasValue)
                    .ProjectToSingleAsync<Command>();

                command.RolesList = await GetRolesList(query);
                command.CompaniesList = await GetCompaniesList(query);
                command.JobTitlesList = await GetJobTitlesList(query);

                return command;
            }

            private async Task<IList<SelectListItem>> GetRolesList(Query query)
            {
                var user = await _db.Users.Include(u => u.CustomRoles).SingleAsync(u => u.Id == query.UserId && !u.DeletedOn.HasValue);
                var customRoles = await _db.CustomRoles.Where(cr => !cr.DeletedOn.HasValue).ToListAsync();

                return customRoles
                    .Select(cr => new SelectListItem
                    {
                        Text = cr.Name,
                        Value = cr.Id.ToString(),
                        Selected = user.CustomRoles.Any(ucr => ucr.Id == cr.Id)
                    })
                    .ToList();
            }

            private async Task<IList<SelectListItem>> GetCompaniesList(Query query)
            {
                var user = await _db.Users.Include(u => u.CustomRoles).SingleAsync(u => u.Id == query.UserId && !u.DeletedOn.HasValue);
                var Companies = await _db.Companies.Where(cr => !cr.DeletedOn.HasValue).ToListAsync();

                return Companies
                    .Select(cp => new SelectListItem
                    {
                        Text = cp.Name,
                        Value = cp.Id.ToString(),
                        Selected = user.AllowedCompanies.Any(uac => uac.Id == cp.Id)
                    })
                    .ToList();
            }

            private async Task<IList<SelectListItem>> GetJobTitlesList(Query query)
            {
                var user = await _db.Users.SingleAsync(u => u.Id == query.UserId && !u.DeletedOn.HasValue);
                var jobTitles = await _db.JobTitles.Where(jt => !jt.DeletedOn.HasValue).ToListAsync();

                return jobTitles
                    .Select(jt => new SelectListItem
                    {
                        Text = jt.Name,
                        Value = jt.Id.ToString(),
                        Selected = user.JobTitleId == jt.Id
                    })
                    .ToList();
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly ApplicationDbContext _db = DependencyConfig.Instance.Container.GetInstance<ApplicationDbContext>();

            public CommandValidator()
            {
                RuleFor(c => c.UserName)
                    .NotEmpty();

                RuleFor(c => c.UserName)
                    .Must(BeUnique)
                    .WithMessage("That username is already taken.");
            }

            private bool BeUnique(Command command, string userName)
            {
                return !_db.Users.Any(u => u.Id != command.Id && u.UserName == userName);
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
                var user = await _db
                    .Users
                    .Include(u => u.CustomRoles)
                    .Include(u => u.AllowedCompanies)
                    .SingleAsync(u => u.Id == command.Id);

                user.CompanyId = command.CompanyId;
                user.Name = command.Name;
                user.JobTitleId = command.JobTitleId;
                user.ModifiedOn = DateTime.UtcNow;
                user.UserName = command.UserName;

                foreach (var roleItem in command.RolesList)
                {
                    var customRoleId = Convert.ToInt32(roleItem.Value);
                    var customRole = await _db.CustomRoles.SingleAsync(cr => cr.Id == customRoleId);

                    if (roleItem.Selected)
                    {
                        user.CustomRoles.Add(customRole);
                    }
                    else
                    {
                        user.CustomRoles.Remove(customRole);
                    }
                }

                foreach (var companyItem in command.CompaniesList)
                {
                    var companyId = Convert.ToInt32(companyItem.Value);
                    var company = await _db.Companies.SingleAsync(cp => cp.Id == companyId);

                    if (companyItem.Selected)
                    {
                        user.AllowedCompanies.Add(company);
                    }
                    else
                    {
                        user.AllowedCompanies.Remove(company);
                    }
                }

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
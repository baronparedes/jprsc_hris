using FluentValidation;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Infrastructure.Identity;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Dependency;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.Accounts
{
    public class Add
    {
        public class Query: IRequest<Command>
        {
        }

        public class Command : IRequest
        {
            public int? CompanyId { get; set; }
            public string Name { get; set; }
            public int? JobTitleId { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string RepeatPassword { get; set; }
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

            public async Task<Command> Handle(Query query, System.Threading.CancellationToken token)
            {
                var command = new Command();

                command.RolesList = await GetRolesList();
                command.CompaniesList = await GetCompaniesList();
                command.JobTitlesList = await GetJobTitlesList();

                return command;
            }

            private async Task<IList<SelectListItem>> GetRolesList()
            {
                var customRoles = await _db.CustomRoles.Where(cr => !cr.DeletedOn.HasValue).ToListAsync();

                return customRoles
                    .Select(cr => new SelectListItem
                    {
                        Text = cr.Name,
                        Value = cr.Id.ToString(),
                        Selected = AppSettings.Ints("DefaultCustomRoleIds").Contains(cr.Id)
                    })
                    .ToList();
            }

            private async Task<IList<SelectListItem>> GetCompaniesList()
            {
                var Companies = await _db.Companies.Where(cr => !cr.DeletedOn.HasValue).ToListAsync();

                return Companies
                    .Select(cp => new SelectListItem
                    {
                        Text = cp.Name,
                        Value = cp.Id.ToString()
                    })
                    .ToList();
            }

            private async Task<IList<SelectListItem>> GetJobTitlesList()
            {
                var jobTitles = await _db.JobTitles.Where(jt => !jt.DeletedOn.HasValue).ToListAsync();

                return jobTitles
                    .Select(jt => new SelectListItem
                    {
                        Text = jt.Name,
                        Value = jt.Id.ToString()
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

                RuleFor(c => c.Password)
                    .NotEmpty();

                RuleFor(c => c.RepeatPassword)
                    .NotEmpty();

                RuleFor(c => c.RepeatPassword)
                    .Must(BeTheSameAsPassword)
                    .WithMessage("Repeat Password must be the same as Password.");

                RuleFor(c => c.UserName)
                    .Must(BeUnique)
                    .WithMessage("That username is already taken.");
            }

            private bool BeUnique(string userName)
            {
                return !_db.Users.Any(u => u.UserName == userName);
            }

            private bool BeTheSameAsPassword(Command command, string repeatPassword)
            {
                return repeatPassword == command.Password;
            }
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly ApplicationDbContext _db;
            private readonly UserManager _userManager;
            private readonly SignInManager _signInManager;

            public CommandHandler(ApplicationDbContext db, UserManager userManager, SignInManager signInManager)
            {
                _db = db;
                _userManager = userManager;
                _signInManager = signInManager;
            }

            public async Task<Unit> Handle(Command command, System.Threading.CancellationToken token)
            {
                var user = new User
                {
                    AddedOn = DateTime.UtcNow,
                    CompanyId = command.CompanyId,
                    Name = command.Name,
                    JobTitleId = command.JobTitleId,
                    UserName = command.UserName
                };

                var createUserResult = await _userManager.CreateAsync(user, command.Password);
                if (!createUserResult.Succeeded)
                {
                    throw new Exception($"Unable to create user. Errors: {createUserResult.Errors.Join(",")}");
                }
                
                var attachedUser = await _db.Users.Include(u => u.CustomRoles).SingleAsync(u => u.Id == user.Id);

                foreach (var roleItem in command.RolesList.Where(ri => ri.Selected))
                {
                    var customRoleId = Convert.ToInt32(roleItem.Value);
                    var customRole = await _db.CustomRoles.SingleAsync(cr => cr.Id == customRoleId);
                    attachedUser.CustomRoles.Add(customRole);
                }

                foreach (var companyItem in command.CompaniesList.Where(ci => ci.Selected))
                {
                    var companyId = Convert.ToInt32(companyItem.Value);
                    var company = await _db.Companies.SingleAsync(cp => cp.Id == companyId);
                    attachedUser.AllowedCompanies.Add(company);
                }
                
                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
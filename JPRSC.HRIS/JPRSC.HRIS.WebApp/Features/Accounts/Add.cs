using FluentValidation;
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

namespace JPRSC.HRIS.WebApp.Features.Accounts
{
    public class Add
    {
        public class Command : IRequest
        {
            public string Name { get; set; }
            public string JobTitle { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string RepeatPassword { get; set; }
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

        public class CommandHandler : IAsyncRequestHandler<Command>
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

            public async Task Handle(Command command)
            {
                var user = new User
                {
                    AddedOn = DateTime.UtcNow,
                    Name = command.Name,
                    JobTitle = command.JobTitle,
                    UserName = command.UserName
                };

                var createUserResult = await _userManager.CreateAsync(user, command.Password);
                if (!createUserResult.Succeeded)
                {
                    throw new Exception($"Unable to create user. Errors: {createUserResult.Errors.Join(",")}");
                }

                var defaultCustomRoleId = 2;
                var attachedUser = _db.Users.Single(u => u.Id == user.Id);
                attachedUser.CustomRoles.Add(await _db.CustomRoles.SingleAsync(cr => cr.Id == defaultCustomRoleId));
                await _db.SaveChangesAsync();
            }
        }
    }
}
using FluentValidation;
using MediatR;
using Microsoft.AspNet.Identity.Owin;
using JPRSC.HRIS.Infrastructure.Identity;
using System.Threading.Tasks;
using JPRSC.HRIS.WebApp.Infrastructure.Dependency;
using JPRSC.HRIS.Infrastructure.Data;
using System;
using System.Linq;

namespace JPRSC.HRIS.WebApp.Features.Account
{
    public class Login
    {
        public class Query : IRequest<Command>
        {
            public string ReturnUrl { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            public Command Handle(Query query)
            {
                return new Command
                {
                    ReturnUrl = query.ReturnUrl
                };
            }
        }

        public class Command : IRequest<SignInStatus>
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public bool RememberMe { get; set; }
            public string ReturnUrl { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly ApplicationDbContext _db = DependencyConfig.Instance.Container.GetInstance<ApplicationDbContext>();

            public CommandValidator()
            {
                RuleFor(c => c.UserName)
                    .NotEmpty();

                RuleFor(c => c.UserName)
                    .Must(NotBeDeleted)
                    .WithMessage("User not found.");

                RuleFor(c => c.Password)
                    .NotEmpty();
            }

            private bool NotBeDeleted(string userName)
            {
                return _db.Users.Any(u => u.UserName == userName && !u.DeletedOn.HasValue);
            }
        }

        public class CommandHandler : IAsyncRequestHandler<Command, SignInStatus>
        {
            private readonly SignInManager _signInManager;

            public CommandHandler(SignInManager signInManager)
            {
                _signInManager = signInManager;
            }

            public async Task<SignInStatus> Handle(Command command)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                return await _signInManager.PasswordSignInAsync(command.UserName, command.Password, command.RememberMe, shouldLockout: false);
            }
        }
    }
}
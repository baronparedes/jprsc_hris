using FluentValidation;
using MediatR;
using Microsoft.AspNet.Identity.Owin;
using JPRSC.HRIS.Infrastructure.Identity;
using System.Threading.Tasks;

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
            public string Email { get; set; }
            public string Password { get; set; }
            public bool RememberMe { get; set; }
            public string ReturnUrl { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.Email)
                    .NotEmpty();

                RuleFor(c => c.Email)
                    .EmailAddress();

                RuleFor(c => c.Password)
                    .NotEmpty();
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
                return await _signInManager.PasswordSignInAsync(command.Email, command.Password, command.RememberMe, shouldLockout: false);
            }
        }
    }
}
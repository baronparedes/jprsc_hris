using FluentValidation;
using MediatR;
using Microsoft.AspNet.Identity;
using JPRSC.HRIS.Infrastructure.Identity;
using JPRSC.HRIS.WebApp.Infrastructure.Dependency;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace JPRSC.HRIS.WebApp.Features.Account
{
    public class ResetPassword
    {
        public class Query : IRequest<Command>
        {
            public string Code { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(c => c.Code)
                    .NotEmpty();
            }
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            public async Task<Command> Handle(Query query, CancellationToken token)
            {
                return new Command
                {
                    Code = query.Code
                };
            }
        }

        public class Command : IRequest
        {
            public string Code { get; set; }
            public string ConfirmPassword { get; set; }
            public string Email { get; set; }
            public string NewPassword { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly PasswordValidator _passwordValidator;
            private readonly UserManager _userManager;

            public CommandValidator()
            {
                _passwordValidator = UserManager.CreatePasswordValidator();
                _userManager = DependencyConfig.Instance.Container.GetInstance<UserManager>();

                RuleFor(c => c.NewPassword)
                    .NotEmpty();

                RuleFor(c => c.NewPassword)
                    .Must(c => _passwordValidator.ValidateAsync(c).Result.Succeeded)
                    .WithMessage(c => _passwordValidator.ValidateAsync(c.NewPassword).Result.Errors.Join(","));

                RuleFor(c => c.ConfirmPassword)
                    .NotEmpty();

                RuleFor(c => c.ConfirmPassword)
                    .Must(IsNewPasswordAndConfirmPasswordTheSame)
                    .WithMessage("The new password and confirm password values must match.");
            }

            private bool IsNewPasswordAndConfirmPasswordTheSame(Command command, string confirmPassword)
            {
                return String.Equals(command.NewPassword, confirmPassword);
            }
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly UserManager _userManager;

            public CommandHandler(UserManager userManager)
            {
                _userManager = userManager;
            }

            public async Task<Unit> Handle(Command command, CancellationToken token)
            {
                var user = await _userManager.FindByNameAsync(command.Email);
                if (user == null)
                {
                    // Do nothing: Don't reveal that the user does not exist
                }
                else
                {
                    var result = await _userManager.ResetPasswordAsync(user.Id, command.Code, command.NewPassword);
                    if (!result.Succeeded)
                    {
                        throw new Exception("Unable to reset password. The link may have expired.");
                    }
                }

                return Unit.Value;
            }
        }
    }
}
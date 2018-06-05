using FluentValidation;
using MediatR;
using JPRSC.HRIS.Infrastructure.Identity;
using System;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Account
{
    public class ConfirmEmail
    {
        public class Command : IRequest
        {
            public string Code { get; set; }
            public string UserId { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(c => c.Code)
                    .NotEmpty();

                RuleFor(c => c.UserId)
                    .NotEmpty();
            }
        }

        public class Handler : AsyncRequestHandler<Command>
        {
            private readonly UserManager _userManager;

            public Handler(UserManager userManager)
            {
                _userManager = userManager;
            }

            protected override async Task HandleCore(Command command)
            {
                var confirmEmailResult = await _userManager.ConfirmEmailAsync(command.UserId, command.Code);
                if (!confirmEmailResult.Succeeded)
                {
                    throw new Exception($"Unable to confirm email. Errors: {confirmEmailResult.Errors.Join(",")}");
                }
            }
        }
    }
}
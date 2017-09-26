using FluentValidation;
using MediatR;
using Microsoft.AspNet.Identity;
using JPRSC.HRIS.Infrastructure.Identity;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web;

namespace JPRSC.HRIS.WebApp.Features.Manage
{
    public class AddPhoneNumber
    {
        public class Command : IRequest
        {
            [Phone]
            [Display(Name = "Phone Number")]
            public string Number { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(c => c.Number)
                    .NotEmpty();
            }
        }

        public class Handler : IAsyncRequestHandler<Command>
        {
            private readonly UserManager _userManager;

            public Handler(UserManager userManager)
            {
                _userManager = userManager;
            }

            public async Task Handle(Command command)
            {
                // Generate the token and send it
                var code = await _userManager.GenerateChangePhoneNumberTokenAsync(HttpContext.Current.User.Identity.GetUserId(), command.Number);
                if (_userManager.SmsService != null)
                {
                    var message = new IdentityMessage
                    {
                        Destination = command.Number,
                        Body = "Your security code is: " + code
                    };

                    await _userManager.SmsService.SendAsync(message);
                }
            }
        }
    }
}
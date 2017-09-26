using FluentValidation;
using MediatR;
using JPRSC.HRIS.WebApp.Controllers;
using JPRSC.HRIS.Infrastructure.Identity;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using JPRSC.HRIS.Infrastructure.Email;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.Account
{
    public class ForgotPassword
    {
        public class Command : IRequest
        {
            public string Email { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(c => c.Email)
                    .NotEmpty();

                RuleFor(c => c.Email)
                    .EmailAddress();
            }
        }

        public class Handler : IAsyncRequestHandler<Command>
        {
            private readonly UserManager _userManager;

            public Handler(UserManager userManager)
            {
                _userManager = userManager;
            }

            public async Task Handle(Command model)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Do nothing: Don't reveal that the user does not exist or is not confirmed
                }
                else
                {
                    var forgotPasswordEmail = await GetForgotPasswordEmail(user);

                    // No await means fire and forget
                    _userManager.SendEmailAsync(user.Id, forgotPasswordEmail);
                }
            }

            private async Task<EmailMessage> GetForgotPasswordEmail(User user)
            {
                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                string code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = urlHelper.Action(nameof(AccountController.ResetPassword), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Current.Request.Url.Scheme);

                return new EmailMessage
                {
                    Body = "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>. If you did not request for a password reset, no action is required.",
                    Subject = "Reset Password"
                };
            }
        }
    }
}
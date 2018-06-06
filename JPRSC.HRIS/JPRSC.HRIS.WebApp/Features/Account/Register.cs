using FluentValidation;
using MediatR;
using Microsoft.AspNet.Identity;
using JPRSC.HRIS.WebApp.Controllers;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Identity;
using JPRSC.HRIS.Models;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using JPRSC.HRIS.Infrastructure.Email;
using System.Threading;

namespace JPRSC.HRIS.WebApp.Features.Account
{
    public class Register
    {
        public class Command : IRequest
        {
            public string Email { get; set; }
            public string Name { get; set; }
            public string Password { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly PasswordValidator _passwordValidator;

            public Validator()
            {
                _passwordValidator = UserManager.CreatePasswordValidator();

                RuleFor(c => c.Email)
                    .NotEmpty();

                RuleFor(c => c.Email)
                    .EmailAddress();

                RuleFor(c => c.Name)
                    .NotEmpty();

                RuleFor(c => c.Password)
                    .NotEmpty();

                RuleFor(c => c.Password)
                    .Must(c => _passwordValidator.ValidateAsync(c).Result.Succeeded)
                    .WithMessage(c => _passwordValidator.ValidateAsync(c.Password).Result.Errors.Join(","));
            }

            private bool IsPasswordValid(string password)
            {
                var passwordValidator = UserManager.CreatePasswordValidator();
                return passwordValidator.ValidateAsync(password).Result.Succeeded;
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly ApplicationDbContext _db;
            private readonly UserManager _userManager;
            private readonly SignInManager _signInManager;

            public Handler(ApplicationDbContext db, UserManager userManager, SignInManager signInManager)
            {
                _db = db;
                _userManager = userManager;
                _signInManager = signInManager;
            }

            public async Task<Unit> Handle(Command command, System.Threading.CancellationToken token)
            {
                var user = new User { Email = command.Email, UserName = command.Email, };
                var createUserResult = await _userManager.CreateAsync(user, command.Password);
                if (!createUserResult.Succeeded)
                {
                    throw new Exception($"Unable to create user. Errors: {createUserResult.Errors.Join(",")}");
                }

                await _signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                var postRegistrationEmailMessage = await GetPostRegistrationEmailMessage(user);

                // No await means fire and forget
                _userManager.SendEmailAsync(user.Id, postRegistrationEmailMessage);

                return Unit.Value;
            }

            private async Task<EmailMessage> GetPostRegistrationEmailMessage(User user)
            {
                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = urlHelper.Action(nameof(AccountController.ConfirmEmail), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Current.Request.Url.Scheme);

                return new EmailMessage
                {
                    Body = $"Welcome to {AppSettings.String("ApplicationName")}! Please confirm your email by clicking <a href=\"" + callbackUrl + "\">here</a>.",
                    Subject = "Confirm your email"
                };
            }
        }
    }
}
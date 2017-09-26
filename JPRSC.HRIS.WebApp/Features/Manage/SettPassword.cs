using FluentValidation;
using MediatR;
using Microsoft.AspNet.Identity;
using JPRSC.HRIS.Infrastructure.Identity;
using JPRSC.HRIS.WebApp.Infrastructure.Dependency;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace JPRSC.HRIS.WebApp.Features.Manage
{
    public class SetPassword
    {
        public class Command : IRequest
        {
            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            public string ConfirmPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly PasswordValidator _passwordValidator;
            private readonly UserManager _userManager;

            public Validator()
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

        public class Handler : IAsyncRequestHandler<Command>
        {
            private readonly UserManager _userManager;
            private readonly SignInManager _signInManager;

            public Handler(UserManager userManager, SignInManager signInManager)
            {
                _userManager = userManager;
                _signInManager = signInManager;
            }

            public async Task Handle(Command command)
            {
                var addPasswordResult = await _userManager.AddPasswordAsync(HttpContext.Current.User.Identity.GetUserId(), command.NewPassword);
                if (!addPasswordResult.Succeeded)
                {
                    throw new Exception($"Unable to add password. Errors: {addPasswordResult.Errors.Join(",")}");
                }

                var user = await _userManager.FindByIdAsync(HttpContext.Current.User.Identity.GetUserId());
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
            }
        }
    }
}
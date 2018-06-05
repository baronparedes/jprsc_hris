using MediatR;
using System;
using System.Web;
using System.Threading.Tasks;
using JPRSC.HRIS.Infrastructure.Identity;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using JPRSC.HRIS.WebApp.Infrastructure.Dependency;

namespace JPRSC.HRIS.WebApp.Features.Manage
{
    public class ChangePassword
    {
        public class Command : IRequest
        {
            [Display(Name = "Confirm new password")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [Display(Name = "Current password")]
            public string OldPassword { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly PasswordValidator _passwordValidator;
            private readonly UserManager _userManager;

            public Validator()
            {
                _passwordValidator = UserManager.CreatePasswordValidator();
                _userManager = DependencyConfig.Instance.Container.GetInstance<UserManager>();

                RuleFor(c => c.OldPassword)
                    .NotEmpty();

                RuleFor(c => c.OldPassword)
                    .Must(IsValidPasswordForUser)
                    .WithMessage("Old password is invalid.");

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

            private bool IsValidPasswordForUser(string oldPassword)
            {
                var currentUser = _userManager.FindByName(HttpContext.Current.User.Identity.Name);
                return _userManager.CheckPassword(currentUser, oldPassword);
            }
        }

        public class Handler : AsyncRequestHandler<Command>
        {
            private readonly SignInManager _signInManager;
            private readonly UserManager _userManager;

            public Handler(UserManager userManager, SignInManager signInManager)
            {
                _userManager = userManager;
                _signInManager = signInManager;
            }

            protected override async Task HandleCore(Command command)
            {
                var currentUser = _userManager.FindByName(HttpContext.Current.User.Identity.Name);
                var changePasswordResult = await _userManager.ChangePasswordAsync(currentUser.Id, command.OldPassword, command.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    throw new Exception($"Unable to change password. Errors: {changePasswordResult.Errors.Join(",")}");
                }

                var user = await _userManager.FindByIdAsync(currentUser.Id);
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
            }
        }
    }
}
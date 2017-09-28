using MediatR;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using JPRSC.HRIS.Infrastructure.Identity;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Features.Account;
using JPRSC.HRIS.WebApp.Infrastructure.Mvc;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Controllers
{
    public class AccountController : AppController
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IMediator _mediator;
        private readonly SignInManager _signInManager;
        private readonly UserManager _userManager;

        public AccountController(UserManager userManager, SignInManager signInManager, IAuthenticationManager authenticationManager, IMediator mediator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authenticationManager = authenticationManager;
            _mediator = mediator;
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(ConfirmEmail.Command command)
        {
            if (!ModelState.IsValid)
            {
                return View("Error");
            }

            await _mediator.Send(command);

            return View(nameof(ConfirmEmail));
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await _authenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await _signInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });

                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _authenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new User { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                ModelState.AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPassword.Command command)
        {
            if (!ModelState.IsValid)
            {
                return View(command);
            }

            await _mediator.Send(command);

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Login(Login.Query query)
        {
            var command = await _mediator.Send(query);

            return View(command);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(Login.Command command)
        {
            if (!ModelState.IsValid)
            {
                return View(command);
            }

            var signInStatus = await _mediator.Send(command);

            switch (signInStatus)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(command.ReturnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = command.ReturnUrl, RememberMe = command.RememberMe });

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(command);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOff(LogOff.Command command)
        {
            await _mediator.Send(command);

            return RedirectToAction(nameof(AccountController.Login), nameof(AccountController));
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(Register.Command command)
        {
            if (!ModelState.IsValid)
            {
                return View(command);
            }

            await _mediator.Send(command);

            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController));
        }

        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(ResetPassword.Query query)
        {
            if (!ModelState.IsValid)
            {
                return View("Error");
            }

            var command = await _mediator.Send(query);

            return View(command);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPassword.Command command)
        {
            if (!ModelState.IsValid)
            {
                return View(command);
            }

            await _mediator.Send(command);

            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> SendCode(SendCode.Query query)
        {
            var command = await _mediator.Send(query);

            return View(command);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCode.Command command)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var query = await _mediator.Send(command);

            return RedirectToAction("VerifyCode", query);
        }

        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(VerifyCode.Query query)
        {
            var command = await _mediator.Send(query);

            return View(command);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCode.Command command)
        {
            if (!ModelState.IsValid)
            {
                return View(command);
            }

            var signInStatus = await _mediator.Send(command);

            switch (signInStatus)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(command.ReturnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(command);
            }
        }
    }
}
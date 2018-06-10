using MediatR;
using JPRSC.HRIS.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Threading;

namespace JPRSC.HRIS.WebApp.Features.Account
{
    public class SendCode
    {
        public class Query : IRequest<Command>
        {
            public bool RememberMe { get; set; }
            public string ReturnUrl { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly SignInManager _signInManager;
            private readonly UserManager _userManager;

            public QueryHandler(SignInManager signInManager, UserManager userManager)
            {
                _signInManager = signInManager;
                _userManager = userManager;
            }

            public async Task<Command> Handle(Query query, CancellationToken token)
            {
                var userId = await _signInManager.GetVerifiedUserIdAsync();
                if (userId == null)
                {
                    throw new Exception("Unable to verify user id.");
                }

                var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(userId);
                var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();

                return new Command
                {
                    Providers = factorOptions,
                    RememberMe = query.RememberMe,
                    ReturnUrl = query.ReturnUrl
                };
            }
        }

        public class Command : IRequest<VerifyCode.Query>
        {
            public ICollection<SelectListItem> Providers { get; set; } = new List<SelectListItem>();
            public bool RememberMe { get; set; }
            public string ReturnUrl { get; set; }
            public string SelectedProvider { get; set; }
        }

        public class CommandHandler : IRequestHandler<Command, VerifyCode.Query>
        {
            private readonly SignInManager _signInManager;

            public CommandHandler(SignInManager signInManager)
            {
                _signInManager = signInManager;
            }

            public async Task<VerifyCode.Query> Handle(Command command, CancellationToken token)
            {
                // Generate the token and send it
                if (!await _signInManager.SendTwoFactorCodeAsync(command.SelectedProvider))
                {
                    throw new Exception("Unable to send two factor code.");
                }

                return new VerifyCode.Query
                {
                    Provider = command.SelectedProvider,
                    RememberMe = command.RememberMe,
                    ReturnUrl = command.ReturnUrl
                };
            }
        }
    }
}
using MediatR;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Account
{
    public class LogOff
    {
        public class Command : IRequest
        {
        }

        public class Handler : AsyncRequestHandler<Command>
        {
            private readonly IAuthenticationManager _authenticationManager;

            public Handler(IAuthenticationManager authenticationManager)
            {
                _authenticationManager = authenticationManager;
            }

            protected override async Task HandleCore(Command command)
            {
                await Task.Factory.StartNew(() =>
                {
                    _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                });
            }
        }
    }
}
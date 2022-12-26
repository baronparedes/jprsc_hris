using MediatR;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.Account
{
    public class LogOff
    {
        public class Command : IRequest
        {
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IAuthenticationManager _authenticationManager;

            public Handler(IAuthenticationManager authenticationManager)
            {
                _authenticationManager = authenticationManager;
            }

            public async Task<Unit> Handle(Command command, CancellationToken token)
            {
                await Task.Factory.StartNew(() =>
                {
                    _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                });

                return Unit.Value;
            }
        }
    }
}
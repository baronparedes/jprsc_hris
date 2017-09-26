using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Infrastructure.Email
{
    public class NoopEmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }
}
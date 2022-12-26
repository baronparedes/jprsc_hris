using System;
using System.Data.Entity;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;

namespace JPRSC.HRIS.Features.SystemSettings
{
    public class EditSMTP
    {
        public class Command : IRequest
        {
            public int Id { get; set; }

            public string EmailAddress { get; set; }
            public string Password { get; set; }
            public string Port { get; set; }
            public string Host { get; set; }
            public string TestEmailAddress { get; set; }
            public bool? EnableSendingEmails { get; set; }

            public bool IsTesting { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
            }
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command command, CancellationToken token)
            {
                if (command.IsTesting)
                {
                    using (var client = new SmtpClient())
                    {
                        var credentials = new NetworkCredential
                        {
                            UserName = command.EmailAddress,
                            Password = command.Password
                        };

                        client.Credentials = credentials;
                        client.Port = Convert.ToInt32(command.Port);
                        client.Host = command.Host;
                        client.EnableSsl = true;

                        using (var email = new MailMessage())
                        {
                            email.To.Add(new MailAddress(command.TestEmailAddress));
                            email.From = new MailAddress(command.EmailAddress);
                            email.Subject = "Test Subject";
                            email.Body = "Test Body";

                            client.Send(email);
                        }
                    }
                }
                else
                {
                    var systemSettings = await _db.SystemSettings.SingleOrDefaultAsync(r => r.Id == command.Id);
                    if (systemSettings == null) throw new Exception("Unable to find system settings");

                    systemSettings.EmailAddress = command.EmailAddress;
                    systemSettings.Password = command.Password;
                    systemSettings.Port = command.Port;
                    systemSettings.Host = command.Host;
                    systemSettings.TestEmailAddress = command.TestEmailAddress;
                    systemSettings.EnableSendingEmails = command.EnableSendingEmails;

                    await _db.SaveChangesAsync();
                }

                return Unit.Value;
            }
        }
    }
}
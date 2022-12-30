using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;

namespace JPRSC.HRIS.Features.Payroll
{
    public class SendPayslip
    {
        public class Command : IRequest<CommandResult>
        {
            public int? PayrollProcessBatchId { get; set; }
        }

        public class CommandResult
        {
            public bool? EnableSendingEmails { get; set; }
        }

        public class CommandHandler : IRequestHandler<Command, CommandResult>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<CommandResult> Handle(Command command, CancellationToken token)
            {
                var systemSettings = await _db.SystemSettings.SingleAsync();

                var commandResult = new CommandResult
                {
                    EnableSendingEmails = systemSettings.EnableSendingEmails
                };

                if (!systemSettings.EnableSendingEmails.HasValue || !systemSettings.EnableSendingEmails.Value) return commandResult;

                var payrollProcessBatch = await _db
                    .PayrollProcessBatches
                    .Include(ppb => ppb.PayrollRecords)
                    .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                    .SingleAsync(ppb => ppb.Id == command.PayrollProcessBatchId);

                // Fire and forget
                SendEmails(systemSettings, payrollProcessBatch);

                return commandResult;
            }

            private async Task SendEmails(Models.SystemSettings systemSettings, Models.PayrollProcessBatch payrollProcessBatch)
            {
                var saveDirectoryBase = HttpContext.Current.Server.MapPath(AppSettings.String("PayslipsPath"));
                var saveDirectoryForBatch = Path.Combine(saveDirectoryBase, $"{payrollProcessBatch.Id}/");

                foreach (var payrollRecord in payrollProcessBatch.PayrollRecords)
                {
                    if (String.IsNullOrWhiteSpace(payrollRecord.Employee.Email)) continue;

                    using (var client = new SmtpClient())
                    {
                        var credentials = new NetworkCredential
                        {
                            UserName = systemSettings.EmailAddress,
                            Password = systemSettings.Password
                        };

                        client.Credentials = credentials;
                        client.Port = Convert.ToInt32(systemSettings.Port);
                        client.Host = systemSettings.Host;
                        client.EnableSsl = true;

                        using (var email = new MailMessage())
                        {
                            email.To.Add(payrollRecord.Employee.Email);
                            email.From = new MailAddress(systemSettings.EmailAddress);
                            email.Subject = $"Payslip for {payrollRecord.Employee.FirstName} {payrollRecord.Employee.LastName}";
                            email.Body = $"Payslip for {payrollRecord.Employee.FirstName} {payrollRecord.Employee.LastName}";

                            var saveFileName = Path.Combine(saveDirectoryForBatch, $"{payrollRecord.EmployeeId}.pdf");
                            email.Attachments.Add(new Attachment(saveFileName));

                            client.Send(email);
                        }
                    }
                }
            }
        }
    }
}
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Employees
{
    public class Transfer
    {
        public class Command : IRequest<CommandResult>
        {
            public int? EmployeeId { get; set; }
            public int? ClientId { get; set; }
        }

        public class CommandResult
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
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
                var employee = await _db.Employees.SingleAsync(r => r.Id == command.EmployeeId);
                employee.ClientId = command.ClientId;
                employee.ModifiedOn = DateTime.UtcNow;

                var rehireTransferEvent = new RehireTransferEvent
                {
                    AddedOn = DateTime.UtcNow,
                    ClientId = command.ClientId,
                    EmployeeId = command.EmployeeId,
                    RehireTransferDateLocal = DateTime.Now,
                    Type = RehireTransferEventType.Transfer
                };
                _db.RehireTransferEvents.Add(rehireTransferEvent);

                await _db.SaveChangesAsync();

                return new CommandResult
                {
                    FirstName = employee.FirstName,
                    LastName = employee.LastName
                };
            }
        }
    }
}
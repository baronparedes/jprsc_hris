using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.Employees
{
    public class Rehire
    {
        public class Command : IRequest<CommandResult>
        {
            public int? EmployeeId { get; set; }
            public int? ClientId { get; set; }
            public DateTime? RehireDate { get; set; }
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
                var employee = await _db.Employees.Include(e => e.RehireTransferEvents).SingleAsync(r => r.Id == command.EmployeeId);

                if (employee.RehireTransferEvents.Count == 0)
                {
                    var originalRehireTransferEvent = new RehireTransferEvent
                    {
                        AddedOn = employee.AddedOn,
                        ClientId = employee.ClientId,
                        EmployeeId = employee.Id,
                        RehireTransferDateLocal = employee.AddedOn.AddHours(8), // all employees so far are based in the PH
                        Type = RehireTransferEventType.New
                    };
                    _db.RehireTransferEvents.Add(originalRehireTransferEvent);
                }

                employee.ClientId = command.ClientId;
                employee.ModifiedOn = DateTime.UtcNow;
                employee.DateHired = command.RehireDate;
                employee.ResignStatus = ResignStatus.None;
                employee.IsActive = true;

                var rehireTransferEvent = new RehireTransferEvent
                {
                    AddedOn = DateTime.UtcNow,
                    ClientId = command.ClientId,
                    EmployeeId = command.EmployeeId,
                    RehireTransferDateLocal = DateTime.Now,
                    Type = RehireTransferEventType.Rehire
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
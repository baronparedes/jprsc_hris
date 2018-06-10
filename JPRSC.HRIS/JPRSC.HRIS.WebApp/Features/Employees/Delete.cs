using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Employees
{
    public class Delete
    {
        public class Command : IRequest<CommandResult>
        {
            public int? EmployeeId { get; set; }
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
                employee.DeletedOn = DateTime.UtcNow;

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
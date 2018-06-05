using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.CustomRoles
{
    public class Delete
    {
        public class Command : IRequest<CommandResult>
        {
            public int? CustomRoleId { get; set; }
        }

        public class CommandResult
        {
            public string Name { get; set; }
        }

        public class CommandHandler : AsyncRequestHandler<Command, CommandResult>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            protected override async Task<CommandResult> HandleCore(Command command)
            {
                var customRole = await _db.CustomRoles.SingleAsync(cr => cr.Id == command.CustomRoleId);
                customRole.DeletedOn = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                return new CommandResult
                {
                    Name = customRole.Name
                };
            }
        }
    }
}
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
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

        public class CommandHandler : IAsyncRequestHandler<Command, CommandResult>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<CommandResult> Handle(Command command)
            {
                var customRole = _db.CustomRoles.Single(cr => cr.Id == command.CustomRoleId);
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
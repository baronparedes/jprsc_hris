using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Clients
{
    public class Delete
    {
        public class Command : IRequest<CommandResult>
        {
            public int? ClientId { get; set; }
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
                var client = _db.Clients.Single(c => c.Id == command.ClientId);
                client.DeletedOn = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                return new CommandResult
                {
                    Name = client.Name
                };
            }
        }
    }
}
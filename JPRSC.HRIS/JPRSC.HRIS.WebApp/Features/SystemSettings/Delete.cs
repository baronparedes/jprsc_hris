using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.SystemSettings
{
    public class Delete
    {
        public class Command : IRequest<CommandResult>
        {
            public int? SystemSettingsId { get; set; }
        }

        public class CommandResult
        {

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
                var systemSettings = await _db.SystemSettings.SingleAsync(r => r.Id == command.SystemSettingsId);
                _db.SystemSettings.Remove(systemSettings);

                await _db.SaveChangesAsync();

                return new CommandResult();
            }
        }
    }
}
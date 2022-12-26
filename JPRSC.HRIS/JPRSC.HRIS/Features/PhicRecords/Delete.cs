using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.PhicRecords
{
    public class Delete
    {
        public class Command : IRequest<CommandResult>
        {
            public int? PhicRecordId { get; set; }
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
                var phicRecord = await _db.PhicRecords.SingleAsync(r => r.Id == command.PhicRecordId);
                phicRecord.DeletedOn = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                return new CommandResult();
            }
        }
    }
}
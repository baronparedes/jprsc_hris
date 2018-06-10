using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.PagIbigRecords
{
    public class Delete
    {
        public class Command : IRequest<CommandResult>
        {
            public int? PagIbigRecordId { get; set; }
        }

        public class CommandResult
        {
            public string Code { get; set; }
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
                var pagIbigRecord = await _db.PagIbigRecords.SingleAsync(r => r.Id == command.PagIbigRecordId);
                pagIbigRecord.DeletedOn = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                return new CommandResult
                {
                    Code = pagIbigRecord.Code
                };
            }
        }
    }
}
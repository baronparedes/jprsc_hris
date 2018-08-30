using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Payroll
{
    public class DeleteForProcessingBatch
    {
        public class Command : IRequest<CommandResult>
        {
            public int? ForProcessingBatchId { get; set; }
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
                var forProcessingBatch = await _db
                    .ForProcessingBatches
                    .SingleAsync(fpb => fpb.Id == command.ForProcessingBatchId);

                _db.ForProcessingBatches.Remove(forProcessingBatch);

                await _db.SaveChangesAsync();

                return new CommandResult();
            }
        }
    }
}
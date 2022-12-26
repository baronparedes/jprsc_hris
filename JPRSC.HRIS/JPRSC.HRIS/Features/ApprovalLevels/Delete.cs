using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.ApprovalLevels
{
    public class Delete
    {
        public class Command : IRequest<CommandResult>
        {
            public int? ApprovalLevelId { get; set; }
        }

        public class CommandResult
        {
            public int? Level { get; set; }
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
                var approvalLevel = await _db.ApprovalLevels.SingleAsync(r => r.Id == command.ApprovalLevelId);
                approvalLevel.DeletedOn = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                return new CommandResult
                {
                    Level = approvalLevel.Level
                };
            }
        }
    }
}
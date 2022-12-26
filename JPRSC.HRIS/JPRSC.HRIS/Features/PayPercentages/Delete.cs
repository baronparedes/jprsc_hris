using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.PayPercentages
{
    public class Delete
    {
        public class Command : IRequest<CommandResult>
        {
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
                return new CommandResult();
            }
        }
    }
}
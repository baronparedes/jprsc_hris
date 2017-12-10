using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.TaxStatuses
{
    public class Delete
    {
        public class Command : IRequest<CommandResult>
        {
            public int? TaxStatusId { get; set; }
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
                var taxStatus = await _db.TaxStatuses.SingleAsync(r => r.Id == command.TaxStatusId);
                taxStatus.DeletedOn = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                return new CommandResult
                {
                    Name = taxStatus.Name
                };
            }
        }
    }
}
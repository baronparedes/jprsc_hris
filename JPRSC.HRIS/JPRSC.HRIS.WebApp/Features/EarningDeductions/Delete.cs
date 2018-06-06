using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.EarningDeductions
{
    public class Delete
    {
        public class Command : IRequest<CommandResult>
        {
            public int? EarningDeductionId { get; set; }
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

            public async Task<CommandResult> Handle(Command command, System.Threading.CancellationToken token)
            {
                var earningDeduction = await _db.EarningDeductions.SingleAsync(r => r.Id == command.EarningDeductionId);
                earningDeduction.DeletedOn = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                return new CommandResult
                {
                    Code = earningDeduction.Code
                };
            }
        }
    }
}
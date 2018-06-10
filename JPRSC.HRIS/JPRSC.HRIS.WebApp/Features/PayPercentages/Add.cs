using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.PayPercentages
{
    public class Add
    {
        public class Command : IRequest
        {
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
            }
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command command, CancellationToken token)
            {
                return Unit.Value;
            }
        }
    }
}
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.Religions
{
    public class Add
    {
        public class Command : IRequest
        {
            public string Code { get; set; }
            public string Description { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.Code)
                    .NotEmpty();
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
                var religion = new Religion
                {
                    AddedOn = DateTime.UtcNow,
                    Code = command.Code,
                    Description = command.Description
                };

                _db.Religions.Add(religion);
                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
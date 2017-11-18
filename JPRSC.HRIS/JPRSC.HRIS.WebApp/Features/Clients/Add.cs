using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Clients
{
    public class Add
    {
        public class Command : IRequest
        {
            public CutOffPeriod? CutOffPeriod { get; set; }
            public int? DaysPerWeek { get; set; }
            public int? HoursPerDay { get; set; }
            public string Name { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.Name)
                    .NotEmpty();
            }
        }

        public class CommandHandler : IAsyncRequestHandler<Command>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task Handle(Command command)
            {
                var client = new Client
                {
                    AddedOn = DateTime.UtcNow,
                    CutOffPeriod = command.CutOffPeriod,
                    DaysPerWeek = command.DaysPerWeek,
                    HoursPerDay = command.HoursPerDay,
                    Name = command.Name
                };

                _db.Clients.Add(client);
                await _db.SaveChangesAsync();
            }
        }
    }
}
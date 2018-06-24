using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.SSSRecords
{
    public class Add
    {
        public class Command : IRequest
        {
            public decimal? ECC { get; set; }
            public decimal? Employee { get; set; }
            public decimal? Employer { get; set; }
            public int? Number { get; set; }
            public decimal? PhilHealthEmployee { get; set; }
            public decimal? PhilHealthEmployer { get; set; }
            public decimal? Range1 { get; set; }
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
                var sssRecord = new SSSRecord
                {
                    AddedOn = DateTime.UtcNow,
                    ECC = command.ECC,
                    Employee = command.Employee,
                    Employer = command.Employer,
                    Number = command.Number,
                    Range1 = command.Range1
                };

                _db.SSSRecords.Add(sssRecord);
                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
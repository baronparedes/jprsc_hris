using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.DailyTimeRecords
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
                var dailyTimeRecord = new DailyTimeRecord
                {
                    AddedOn = DateTime.UtcNow
                };

                _db.DailyTimeRecords.Add(dailyTimeRecord);
                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
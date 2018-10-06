using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.SystemSettings
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int SystemSettingsId { get; set; }
        }

        public class Command : IRequest
        {
            public int Id { get; set; }
            public decimal? MinimumNetPay { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<Command> Handle(Query query, CancellationToken token)
            {
                if (query.SystemSettingsId == default(int)) query.SystemSettingsId = _db.SystemSettings.Select(pr => pr.Id).FirstOrDefault();

                var command = await _db.SystemSettings.Where(r => r.Id == query.SystemSettingsId).ProjectToSingleOrDefaultAsync<Command>();

                if (command == null)
                {
                    command = new Command
                    {
                        MinimumNetPay = 2000
                    };
                }
                else if (!command.MinimumNetPay.HasValue) command.MinimumNetPay = 2000;

                return command;
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.MinimumNetPay)
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
                var systemSettings = await _db.SystemSettings.SingleOrDefaultAsync(r => r.Id == command.Id);
                if (systemSettings != null)
                {
                    systemSettings.MinimumNetPay = command.MinimumNetPay;
                }
                else
                {
                    _db.SystemSettings.Add(new Models.SystemSettings
                    {
                        MinimumNetPay = command.MinimumNetPay
                    });
                }

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
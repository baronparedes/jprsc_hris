using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.Features.SystemSettings
{
    public class Add
    {
        public class Query : IRequest<Command>
        {
        }

        public class Command : IRequest<CommandResult>
        {
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
                var command = new Command();

                return command;
            }
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
                var systemSettings = new JPRSC.HRIS.Models.SystemSettings
                {
                    MinimumNetPay = command.MinimumNetPay
                };

                _db.SystemSettings.Add(systemSettings);
                await _db.SaveChangesAsync();

                return new CommandResult();
            }
        }
    }
}
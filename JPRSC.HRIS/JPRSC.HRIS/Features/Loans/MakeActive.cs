﻿using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.Loans
{
    public class MakeActive
    {
        public class Command : IRequest<CommandResult>
        {
            public int? LoanId { get; set; }
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
                var loan = await _db.Loans.SingleAsync(r => r.Id == command.LoanId);
                loan.ZeroedOutOn = null;

                await _db.SaveChangesAsync();

                return new CommandResult();
            }
        }
    }
}
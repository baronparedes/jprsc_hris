using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace JPRSC.HRIS.WebApp.Features.Payroll
{
    public class Process
    {
        public class Command : IRequest<CommandResult>
        {
            public int? ClientId { get; set; }
            public DateTime? PayrollPeriodFrom { get; set; }
            public DateTime? PayrollPeriodTo { get; set; }
            public int? PayrollPeriod { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.ClientId)
                    .NotEmpty();

                RuleFor(c => c.PayrollPeriodFrom)
                    .NotEmpty();

                RuleFor(c => c.PayrollPeriodTo)
                    .NotEmpty();

                RuleFor(c => c.PayrollPeriod)
                    .NotEmpty();
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

            public async Task<CommandResult> Handle(Command command, CancellationToken cancellationToken)
            {


                return new CommandResult();
            }
        }
    }
}
﻿using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.Clients
{
    public class Add
    {
        public class Command : IRequest
        {
            public string Code { get; set; }
            public int? CurrentPayrollPeriod { get; set; }
            public int? DaysPerWeek { get; set; }
            public string Description { get; set; }
            public int? HoursPerDay { get; set; }
            public string Name { get; set; }
            public int? NumberOfPayrollPeriodsAMonth { get; set; }
            public int? NumberOfWorkingDaysForThisPayrollPeriod { get; set; }
            public PayrollCode? PayrollCode { get; set; }
            public DateTime? PayrollPeriodFrom { get; set; }
            public Month? PayrollPeriodMonth { get; set; }
            public DateTime? PayrollPeriodTo { get; set; }
            public TaxTable? TaxTable { get; set; }
            public bool? ZeroBasic { get; set; }

            public string SSSPayrollPeriod { get; set; }
            public bool? SSSBasic { get; set; }
            public bool? SSSOvertime { get; set; }
            public bool? SSSCola { get; set; }
            public bool? SSSEarnings { get; set; }
            public bool? SSSDeductions { get; set; }
            public bool? SSSUndertime { get; set; }
            public int? SSSRangeOffset { get; set; }

            public string PHICPayrollPeriod { get; set; }
            public bool? PHICBasic { get; set; }
            public bool? PHICOvertime { get; set; }
            public bool? PHICCola { get; set; }
            public bool? PHICEarnings { get; set; }
            public bool? PHICDeductions { get; set; }
            public bool? PHICUndertime { get; set; }

            public string PagIbigPayrollPeriod { get; set; }
            public bool? PagIbigBasic { get; set; }
            public bool? PagIbigOvertime { get; set; }
            public bool? PagIbigCola { get; set; }
            public bool? PagIbigEarnings { get; set; }
            public bool? PagIbigDeductions { get; set; }
            public bool? PagIbigUndertime { get; set; }

            public string TaxPayrollPeriod { get; set; }
            public bool? TaxBasic { get; set; }
            public bool? TaxOvertime { get; set; }
            public bool? TaxCola { get; set; }
            public bool? TaxEarnings { get; set; }
            public bool? TaxDeductions { get; set; }
            public bool? TaxUndertime { get; set; }

            public bool? LoanExempt { get; set; }
            public bool? PagIbigExempt { get; set; }
            public bool? PHICExempt { get; set; }
            public bool? SSSExempt { get; set; }
            public bool? TaxExempt { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.Name)
                    .NotEmpty();

                When(c => c.SSSRangeOffset.HasValue, () =>
                {
                    RuleFor(c => c.SSSRangeOffset)
                        .GreaterThanOrEqualTo(-100);

                    RuleFor(c => c.SSSRangeOffset)
                        .LessThanOrEqualTo(100);
                });
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
                var client = new Client
                {
                    AddedOn = DateTime.UtcNow,
                    Code = command.Code,
                    CurrentPayrollPeriod = command.CurrentPayrollPeriod,
                    DaysPerWeek = command.DaysPerWeek,
                    Description = command.Description,
                    HoursPerDay = command.HoursPerDay,
                    LoanExempt = command.LoanExempt,
                    Name = command.Name,
                    NumberOfPayrollPeriodsAMonth = command.NumberOfPayrollPeriodsAMonth,
                    NumberOfWorkingDaysForThisPayrollPeriod = command.NumberOfWorkingDaysForThisPayrollPeriod,
                    PagIbigExempt = command.PagIbigExempt,
                    PagIbigBasic = command.PagIbigBasic.GetValueOrDefault(),
                    PagIbigCola = command.PagIbigCola.GetValueOrDefault(),
                    PagIbigOvertime = command.PagIbigOvertime.GetValueOrDefault(),
                    PagIbigEarnings = command.PagIbigEarnings.GetValueOrDefault(),
                    PagIbigDeductions = command.PagIbigDeductions.GetValueOrDefault(),
                    PagIbigUndertime = command.PagIbigUndertime.GetValueOrDefault(),
                    PagIbigPayrollPeriod = command.PagIbigPayrollPeriod,
                    PayrollCode = command.PayrollCode,
                    PayrollPeriodFrom = command.PayrollPeriodFrom,
                    PayrollPeriodMonth = command.PayrollPeriodMonth,
                    PayrollPeriodTo = command.PayrollPeriodTo,
                    PHICExempt = command.PHICExempt,
                    PHICBasic = command.PHICBasic.GetValueOrDefault(),
                    PHICCola = command.PHICCola.GetValueOrDefault(),
                    PHICOvertime = command.PHICOvertime.GetValueOrDefault(),
                    PHICEarnings = command.PHICEarnings.GetValueOrDefault(),
                    PHICDeductions = command.PHICDeductions.GetValueOrDefault(),
                    PHICUndertime = command.PHICUndertime.GetValueOrDefault(),
                    PHICPayrollPeriod = command.PHICPayrollPeriod,
                    SSSExempt = command.SSSExempt,
                    SSSBasic = command.SSSBasic.GetValueOrDefault(),
                    SSSCola = command.SSSCola.GetValueOrDefault(),
                    SSSOvertime = command.SSSOvertime.GetValueOrDefault(),
                    SSSEarnings = command.SSSEarnings.GetValueOrDefault(),
                    SSSDeductions = command.SSSDeductions.GetValueOrDefault(),
                    SSSUndertime = command.SSSUndertime.GetValueOrDefault(),
                    SSSRangeOffset = command.SSSRangeOffset.GetValueOrDefault(),
                    SSSPayrollPeriod = command.SSSPayrollPeriod,
                    TaxExempt = command.TaxExempt,
                    TaxBasic = command.TaxBasic.GetValueOrDefault(),
                    TaxCola = command.TaxCola.GetValueOrDefault(),
                    TaxOvertime = command.TaxOvertime.GetValueOrDefault(),
                    TaxEarnings = command.TaxEarnings.GetValueOrDefault(),
                    TaxDeductions = command.TaxDeductions.GetValueOrDefault(),
                    TaxUndertime = command.TaxUndertime.GetValueOrDefault(),
                    TaxPayrollPeriod = command.TaxPayrollPeriod,
                    TaxTable = command.TaxTable,
                    ZeroBasic = command.ZeroBasic
                };

                _db.Clients.Add(client);
                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
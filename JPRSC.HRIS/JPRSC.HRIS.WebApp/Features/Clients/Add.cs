using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Clients
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
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.Name)
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
                var client = new Client
                {
                    AddedOn = DateTime.UtcNow,
                    Code = command.Code,
                    CurrentPayrollPeriod = command.CurrentPayrollPeriod,
                    DaysPerWeek = command.DaysPerWeek,
                    Description = command.Description,
                    HoursPerDay = command.HoursPerDay,
                    Name = command.Name,
                    NumberOfPayrollPeriodsAMonth = command.NumberOfPayrollPeriodsAMonth,
                    NumberOfWorkingDaysForThisPayrollPeriod = command.NumberOfWorkingDaysForThisPayrollPeriod,
                    PayrollCode = command.PayrollCode,
                    PayrollPeriodFrom = command.PayrollPeriodFrom,
                    PayrollPeriodMonth = command.PayrollPeriodMonth,
                    PayrollPeriodTo = command.PayrollPeriodTo,
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
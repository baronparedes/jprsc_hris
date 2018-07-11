using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Clients
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int ClientId { get; set; }
        }

        public class Command : IRequest
        {
            public string Code { get; set; }
            public int? CurrentPayrollPeriod { get; set; }
            public int? DaysPerWeek { get; set; }
            public string Description { get; set; }
            public int? HoursPerDay { get; set; }
            public int Id { get; set; }
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

            public string PHICPayrollPeriod { get; set; }
            public bool? PHICBasic { get; set; }
            public bool? PHICOvertime { get; set; }
            public bool? PHICCola { get; set; }

            public string PagIbigPayrollPeriod { get; set; }
            public bool? PagIbigBasic { get; set; }
            public bool? PagIbigOvertime { get; set; }
            public bool? PagIbigCola { get; set; }

            public string TaxPayrollPeriod { get; set; }
            public bool? TaxBasic { get; set; }
            public bool? TaxOvertime { get; set; }
            public bool? TaxCola { get; set; }
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
                return await _db.Clients.Where(c => c.Id == query.ClientId && !c.DeletedOn.HasValue).ProjectToSingleAsync<Command>();
            }
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
                var client = await _db.Clients.SingleAsync(c => c.Id == command.Id);

                client.Code = command.Code;
                client.CurrentPayrollPeriod = command.CurrentPayrollPeriod;
                client.DaysPerWeek = command.DaysPerWeek;
                client.Description = command.Description;
                client.HoursPerDay = command.HoursPerDay;
                client.ModifiedOn = DateTime.UtcNow;
                client.Name = command.Name;
                client.NumberOfWorkingDaysForThisPayrollPeriod = command.NumberOfWorkingDaysForThisPayrollPeriod;
                client.PagIbigBasic = command.PagIbigBasic.HasValue ? command.PagIbigBasic.Value : false;
                client.PagIbigCola = command.PagIbigCola.HasValue ? command.PagIbigCola.Value : false;
                client.PagIbigOvertime = command.PagIbigOvertime.HasValue ? command.PagIbigOvertime.Value : false;
                client.PagIbigPayrollPeriod = command.PagIbigPayrollPeriod;
                client.PayrollCode = command.PayrollCode;
                client.PayrollPeriodFrom = command.PayrollPeriodFrom;
                client.PayrollPeriodMonth = command.PayrollPeriodMonth;
                client.PayrollPeriodTo = command.PayrollPeriodTo;
                client.PHICBasic = command.PHICBasic.HasValue ? command.PHICBasic.Value : false;
                client.PHICCola = command.PHICCola.HasValue ? command.PHICCola.Value : false;
                client.PHICOvertime = command.PHICOvertime.HasValue ? command.PHICOvertime.Value : false;
                client.PHICPayrollPeriod = command.PHICPayrollPeriod;
                client.SSSBasic = command.SSSBasic.HasValue ? command.SSSBasic.Value : false;
                client.SSSCola = command.SSSCola.HasValue ? command.SSSCola.Value : false;
                client.SSSOvertime = command.SSSOvertime.HasValue ? command.SSSOvertime.Value : false;
                client.SSSPayrollPeriod = command.SSSPayrollPeriod;
                client.TaxBasic = command.TaxBasic.HasValue ? command.TaxBasic.Value : false;
                client.TaxCola = command.TaxCola.HasValue ? command.TaxCola.Value : false;
                client.TaxOvertime = command.TaxOvertime.HasValue ? command.TaxOvertime.Value : false;
                client.TaxPayrollPeriod = command.TaxPayrollPeriod;
                client.TaxTable = command.TaxTable;
                client.ZeroBasic = command.ZeroBasic;

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
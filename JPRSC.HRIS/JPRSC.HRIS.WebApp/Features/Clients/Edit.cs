using AutoMapper;
using AutoMapper.QueryableExtensions;
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
            public bool? SSSEarnings { get; set; }
            public bool? SSSDeductions { get; set; }
            public bool? SSSUndertime { get; set; }

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

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Client, Command>();
            }
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly ApplicationDbContext _db;
            private readonly IMapper _mapper;

            public QueryHandler(ApplicationDbContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Command> Handle(Query query, CancellationToken token)
            {
                return await _db.Clients.AsNoTracking().Where(c => c.Id == query.ClientId && !c.DeletedOn.HasValue).ProjectTo<Command>(_mapper).SingleAsync();
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
                client.LoanExempt = command.LoanExempt;
                client.ModifiedOn = DateTime.UtcNow;
                client.Name = command.Name;
                client.NumberOfWorkingDaysForThisPayrollPeriod = command.NumberOfWorkingDaysForThisPayrollPeriod;
                client.PagIbigExempt = command.PagIbigExempt;
                client.PagIbigBasic = command.PagIbigBasic.GetValueOrDefault();
                client.PagIbigCola = command.PagIbigCola.GetValueOrDefault();
                client.PagIbigOvertime = command.PagIbigOvertime.GetValueOrDefault();
                client.PagIbigEarnings = command.PagIbigEarnings.GetValueOrDefault();
                client.PagIbigDeductions = command.PagIbigDeductions.GetValueOrDefault();
                client.PagIbigUndertime = command.PagIbigUndertime.GetValueOrDefault();
                client.PagIbigPayrollPeriod = command.PagIbigPayrollPeriod;
                client.PayrollCode = command.PayrollCode;
                client.PayrollPeriodFrom = command.PayrollPeriodFrom;
                client.PayrollPeriodMonth = command.PayrollPeriodMonth;
                client.PayrollPeriodTo = command.PayrollPeriodTo;
                client.PHICExempt = command.PHICExempt;
                client.PHICBasic = command.PHICBasic.GetValueOrDefault();
                client.PHICCola = command.PHICCola.GetValueOrDefault();
                client.PHICOvertime = command.PHICOvertime.GetValueOrDefault();
                client.PHICEarnings = command.PHICEarnings.GetValueOrDefault();
                client.PHICDeductions = command.PHICDeductions.GetValueOrDefault();
                client.PHICUndertime = command.PHICUndertime.GetValueOrDefault();
                client.PHICPayrollPeriod = command.PHICPayrollPeriod;
                client.SSSExempt = command.SSSExempt;
                client.SSSBasic = command.SSSBasic.GetValueOrDefault();
                client.SSSCola = command.SSSCola.GetValueOrDefault();
                client.SSSOvertime = command.SSSOvertime.GetValueOrDefault();
                client.SSSEarnings = command.SSSEarnings.GetValueOrDefault();
                client.SSSDeductions = command.SSSDeductions.GetValueOrDefault();
                client.SSSUndertime = command.SSSUndertime.GetValueOrDefault();
                client.SSSPayrollPeriod = command.SSSPayrollPeriod;
                client.TaxExempt = command.TaxExempt;
                client.TaxBasic = command.TaxBasic.GetValueOrDefault();
                client.TaxCola = command.TaxCola.GetValueOrDefault();
                client.TaxOvertime = command.TaxOvertime.GetValueOrDefault();
                client.TaxEarnings = command.TaxEarnings.GetValueOrDefault();
                client.TaxDeductions = command.TaxDeductions.GetValueOrDefault();
                client.TaxUndertime = command.TaxUndertime.GetValueOrDefault();
                client.TaxPayrollPeriod = command.TaxPayrollPeriod;
                client.TaxTable = command.TaxTable;
                client.ZeroBasic = command.ZeroBasic;

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
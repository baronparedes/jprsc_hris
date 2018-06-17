using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.SSSRecords
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int SSSRecordId { get; set; }
        }

        public class Command : IRequest
        {
            public decimal? ECC { get; set; }
            public decimal? Employee { get; set; }
            public decimal? Employer { get; set; }
            public int Id { get; set; }
            public int? Number { get; set; }
            public decimal? PhilHealthEmployee { get; set; }
            public decimal? PhilHealthEmployer { get; set; }
            public decimal? Range1 { get; set; }
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
                return await _db.SSSRecords.Where(r => r.Id == query.SSSRecordId && !r.DeletedOn.HasValue).ProjectToSingleAsync<Command>();
            }
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
                var sssRecord = await _db.SSSRecords.SingleAsync(r => r.Id == command.Id);

                sssRecord.ECC = command.ECC;
                sssRecord.Employee = command.Employee;
                sssRecord.Employer = command.Employer;
                sssRecord.ModifiedOn = DateTime.UtcNow;
                sssRecord.Number = command.Number;
                sssRecord.PhilHealthEmployee = command.PhilHealthEmployee;
                sssRecord.PhilHealthEmployer = command.PhilHealthEmployer;
                sssRecord.Range1 = command.Range1;

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
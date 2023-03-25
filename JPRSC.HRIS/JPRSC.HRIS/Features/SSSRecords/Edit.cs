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

namespace JPRSC.HRIS.Features.SSSRecords
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
            public decimal? Range1 { get; set; }
            public decimal? Range1End { get; set; }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<SSSRecord, Command>();
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
                return await _db.SSSRecords.AsNoTracking().Where(r => r.Id == query.SSSRecordId && !r.DeletedOn.HasValue).ProjectTo<Command>(_mapper).SingleAsync();
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                When(r => r.Range1.HasValue && r.Range1End.HasValue, () =>
                {
                    RuleFor(r => r.Range1End)
                        .Must((r, range1End) =>
                        {
                            return range1End >= r.Range1;
                        })
                        .WithMessage("Invalid range.");
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
                var sssRecord = await _db.SSSRecords.SingleAsync(r => r.Id == command.Id);

                sssRecord.ECC = command.ECC;
                sssRecord.Employee = command.Employee;
                sssRecord.Employer = command.Employer;
                sssRecord.ModifiedOn = DateTime.UtcNow;
                sssRecord.Number = command.Number;
                sssRecord.Range1 = command.Range1;
                sssRecord.Range1End = command.Range1End;

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
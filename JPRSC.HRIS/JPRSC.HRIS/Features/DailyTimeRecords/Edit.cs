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

namespace JPRSC.HRIS.Features.DailyTimeRecords
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int DailyTimeRecordId { get; set; }
        }

        public class Command : IRequest
        {
            public int Id { get; set; }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<DailyTimeRecord, Command>();
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
                return await _db.DailyTimeRecords.AsNoTracking().Where(r => r.Id == query.DailyTimeRecordId && !r.DeletedOn.HasValue).ProjectTo<Command>(_mapper).SingleAsync();
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
                var dailyTimeRecord = await _db.DailyTimeRecords.SingleAsync(r => r.Id == command.Id);

                dailyTimeRecord.ModifiedOn = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
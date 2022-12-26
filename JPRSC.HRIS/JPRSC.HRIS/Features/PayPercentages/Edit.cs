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

namespace JPRSC.HRIS.Features.PayPercentages
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int PayPercentageId { get; set; }
        }

        public class Command : IRequest
        {
            public string Code { get; set; }
            public int Id { get; set; }
            public bool? IncludeCOLA { get; set; }
            public string Name { get; set; }
            public double? Percentage { get; set; }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<PayPercentage, Command>();
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
                return await _db.PayPercentages.AsNoTracking().Where(r => r.Id == query.PayPercentageId).ProjectTo<Command>(_mapper).SingleAsync();
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.Code)
                    .NotEmpty();

                RuleFor(c => c.Percentage)
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
                var payPercentage = await _db.PayPercentages.SingleAsync(r => r.Id == command.Id);

                payPercentage.Code = command.Code;
                payPercentage.IncludeCOLA = command.IncludeCOLA;
                payPercentage.ModifiedOn = DateTime.UtcNow;
                payPercentage.Name = command.Name;
                payPercentage.Percentage = command.Percentage;

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
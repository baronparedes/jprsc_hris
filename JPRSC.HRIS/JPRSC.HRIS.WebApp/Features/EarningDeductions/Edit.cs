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

namespace JPRSC.HRIS.WebApp.Features.EarningDeductions
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int EarningDeductionId { get; set; }
        }

        public class Command : IRequest
        {
            public string Code { get; set; }
            public int Id { get; set; }
            public string Description { get; set; }
            public EarningDeductionType? EarningDeductionType { get; set; }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<EarningDeduction, Command>();
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
                return await _db.EarningDeductions.AsNoTracking().Where(r => r.Id == query.EarningDeductionId && !r.DeletedOn.HasValue).ProjectTo<Command>(_mapper).SingleAsync();
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.Code)
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
                var earningDeduction = await _db.EarningDeductions.SingleAsync(r => r.Id == command.Id);

                earningDeduction.Code = command.Code;
                earningDeduction.Description = command.Description;
                earningDeduction.EarningDeductionType = command.EarningDeductionType;
                earningDeduction.ModifiedOn = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
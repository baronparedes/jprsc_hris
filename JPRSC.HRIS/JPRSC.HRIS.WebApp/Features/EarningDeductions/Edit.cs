using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
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

        public class QueryHandler : IAsyncRequestHandler<Query, Command>
        {
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<Command> Handle(Query query)
            {
                return await _db.EarningDeductions.Where(r => r.Id == query.EarningDeductionId && !r.DeletedOn.HasValue).ProjectToSingleAsync<Command>();
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

        public class CommandHandler : IAsyncRequestHandler<Command>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task Handle(Command command)
            {
                var earningDeduction = await _db.EarningDeductions.SingleAsync(r => r.Id == command.Id);

                earningDeduction.Code = command.Code;
                earningDeduction.Description = command.Description;
                earningDeduction.EarningDeductionType = command.EarningDeductionType;
                earningDeduction.ModifiedOn = DateTime.UtcNow;

                await _db.SaveChangesAsync();
            }
        }
    }
}
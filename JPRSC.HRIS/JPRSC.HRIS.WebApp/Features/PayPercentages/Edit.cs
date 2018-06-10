using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.PayPercentages
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int PayPercentageId { get; set; }
        }

        public class Command : IRequest
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double? Percentage { get; set; }
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
                return await _db.PayPercentages.Where(r => r.Id == query.PayPercentageId).ProjectToSingleAsync<Command>();
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
                var payPercentage = await _db.PayPercentages.SingleAsync(r => r.Id == command.Id);

                payPercentage.ModifiedOn = DateTime.UtcNow;
                payPercentage.Name = command.Name;
                payPercentage.Percentage = command.Percentage;

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
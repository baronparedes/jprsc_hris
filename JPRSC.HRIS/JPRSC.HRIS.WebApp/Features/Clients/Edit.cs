using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
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
            public CutOffPeriod? CutOffPeriod { get; set; }
            public int? DaysPerWeek { get; set; }
            public int? HoursPerDay { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
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

        public class CommandHandler : IAsyncRequestHandler<Command>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task Handle(Command command)
            {
                var client = await _db.Clients.SingleAsync(c => c.Id == command.Id);

                client.CutOffPeriod = command.CutOffPeriod;
                client.DaysPerWeek = command.DaysPerWeek;
                client.HoursPerDay = command.HoursPerDay;
                client.ModifiedOn = DateTime.UtcNow;
                client.Name = command.Name;

                await _db.SaveChangesAsync();
            }
        }
    }
}
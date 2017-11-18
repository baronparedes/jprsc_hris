using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.CustomRoles
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int CustomRoleId { get; set; }
        }

        public class Command : IRequest
        {
            public string Name { get; set; }
            public int Id { get; set; }
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
                return await _db.CustomRoles.Where(cr => cr.Id == query.CustomRoleId && !cr.DeletedOn.HasValue).ProjectToSingleAsync<Command>();
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
                var customRole = _db.CustomRoles.Single(cr => cr.Id == command.Id);

                customRole.Name = command.Name;
                customRole.ModifiedOn = DateTime.UtcNow;

                await _db.SaveChangesAsync();
            }
        }
    }
}
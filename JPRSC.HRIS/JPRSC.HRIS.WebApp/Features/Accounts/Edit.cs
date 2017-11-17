using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.WebApp.Infrastructure.Dependency;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Accounts
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public string UserId { get; set; }
        }

        public class Command : IRequest
        {
            public string Name { get; set; }
            public string Id { get; set; }
            public string JobTitle { get; set; }
            public string UserName { get; set; }
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
                return await _db.Users.Where(u => u.Id == query.UserId && !u.DeletedOn.HasValue).ProjectToSingleAsync<Command>();
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly ApplicationDbContext _db = DependencyConfig.Instance.Container.GetInstance<ApplicationDbContext>();

            public CommandValidator()
            {
                RuleFor(c => c.UserName)
                    .NotEmpty();

                RuleFor(c => c.UserName)
                    .Must(BeUnique)
                    .WithMessage("That username is already taken.");
            }

            private bool BeUnique(Command command, string userName)
            {
                return !_db.Users.Any(u => u.Id != command.Id && u.UserName == userName);
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
                var user = _db.Users.Single(u => u.Id == command.Id);
                user.Name = command.Name;
                user.JobTitle = command.JobTitle;
                user.ModifiedOn = DateTime.UtcNow;
                user.UserName = command.UserName;

                await _db.SaveChangesAsync();
            }
        }
    }
}
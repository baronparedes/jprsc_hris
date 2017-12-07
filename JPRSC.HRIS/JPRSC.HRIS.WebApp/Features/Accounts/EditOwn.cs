using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.WebApp.Infrastructure.Dependency;
using MediatR;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace JPRSC.HRIS.WebApp.Features.Accounts
{
    public class EditOwn
    {
        public class Query : IRequest<Command>
        {
        }

        public class Command : IRequest
        {
            public string Name { get; set; }
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
                var currentUserId = HttpContext.Current.User.Identity.GetUserId();

                var command = await _db
                    .Users
                    .Where(u => u.Id == currentUserId && !u.DeletedOn.HasValue)
                    .ProjectToSingleAsync<Command>();

                return command;
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
                var currentUserId = HttpContext.Current.User.Identity.GetUserId();

                return !_db.Users.Any(u => u.Id != currentUserId && u.UserName == userName);
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
                var currentUserId = HttpContext.Current.User.Identity.GetUserId();

                var user = await _db.Users.Include(u => u.CustomRoles).SingleAsync(u => u.Id == currentUserId);
                user.Name = command.Name;
                user.ModifiedOn = DateTime.UtcNow;
                user.UserName = command.UserName;

                await _db.SaveChangesAsync();
            }
        }
    }
}
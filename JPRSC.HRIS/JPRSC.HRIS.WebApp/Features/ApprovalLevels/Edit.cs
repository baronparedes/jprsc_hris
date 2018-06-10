using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.ApprovalLevels
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int ApprovalLevelId { get; set; }
        }

        public class Command : IRequest
        {
            public IList<SelectListItem> UsersList { get; set; } = new List<SelectListItem>();

            public int Id { get; set; }
            public int? Level { get; set; }
            public string UserId { get; set; }
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
                var command = await _db.ApprovalLevels.Where(r => r.Id == query.ApprovalLevelId && !r.DeletedOn.HasValue).ProjectToSingleAsync<Command>();

                command.UsersList = await GetUsersList(query);

                return command;
            }

            private async Task<IList<SelectListItem>> GetUsersList(Query query)
            {
                var approvalLevel = await _db.ApprovalLevels.SingleAsync(al => al.Id == query.ApprovalLevelId);
                var users = await _db.Users.Where(u => !u.DeletedOn.HasValue).ToListAsync();

                return users
                    .Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id,
                        Selected = approvalLevel.UserId == u.Id
                    })
                    .ToList();
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.Level)
                    .MustBeANumber();

                RuleFor(c => c.UserId)
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
                var approvalLevel = await _db.ApprovalLevels.SingleAsync(r => r.Id == command.Id);
                
                approvalLevel.ModifiedOn = DateTime.UtcNow;
                approvalLevel.Level = command.Level;
                approvalLevel.UserId = command.UserId;

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
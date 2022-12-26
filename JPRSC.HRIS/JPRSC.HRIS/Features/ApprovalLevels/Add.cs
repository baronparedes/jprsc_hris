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
using System.Web.Mvc;

namespace JPRSC.HRIS.Features.ApprovalLevels
{
    public class Add
    {
        public class Query : IRequest<Command>
        {
        }

        public class Command : IRequest<CommandResult>
        {
            public IList<SelectListItem> UsersList { get; set; } = new List<SelectListItem>();

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
                var command = new Command();

                command.UsersList = await GetUsersList();

                return command;
            }

            private async Task<IList<SelectListItem>> GetUsersList()
            {
                var users = await _db.Users.Where(u => !u.DeletedOn.HasValue).ToListAsync();

                return users
                    .Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id
                    })
                    .ToList();
            }
        }

        public class CommandResult
        {
            public int? Level { get; set; }
        }

        public class CommandHandler : IRequestHandler<Command, CommandResult>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<CommandResult> Handle(Command command, CancellationToken token)
            {
                var approvalLevel = new ApprovalLevel
                {
                    AddedOn = DateTime.UtcNow,
                    Level = await GetNextLevel(),
                    UserId = command.UserId
                };

                _db.ApprovalLevels.Add(approvalLevel);
                await _db.SaveChangesAsync();

                return new CommandResult
                {
                    Level = approvalLevel.Level
                };
            }

            private async Task<int> GetNextLevel()
            {
                var approvals = await _db
                    .ApprovalLevels
                    .Where(al => !al.DeletedOn.HasValue && al.Level != null)
                    .ToListAsync();

                if (!approvals.Any()) return 1;

                return approvals.Max(al => al.Level.Value) + 1;
            }
        }
    }
}
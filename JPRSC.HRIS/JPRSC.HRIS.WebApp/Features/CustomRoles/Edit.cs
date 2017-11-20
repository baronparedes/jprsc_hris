using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;

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
            public int Id { get; set; }
            public string Name { get; set; }
            public IList<SelectListItem> PermissionsList { get; set; } = EnumHelper.GetSelectList(typeof(Permission));
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
                var command = await _db
                    .CustomRoles
                    .Where(cr => cr.Id == query.CustomRoleId && !cr.DeletedOn.HasValue)
                    .ProjectToSingleAsync<Edit.Command>();

                command.PermissionsList = EnumHelper.GetSelectList(typeof(Permission));

                var customRole = await _db
                    .CustomRoles
                    .SingleAsync(cr => cr.Id == query.CustomRoleId && !cr.DeletedOn.HasValue);

                foreach (var permission in customRole.Permissions)
                {
                    var correspondingPermissionListItem = command.PermissionsList.Single(p => p.Value == ((int)permission).ToString());
                    correspondingPermissionListItem.Selected = true;
                }

                return command;
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

                foreach (var permissionListItem in command.PermissionsList)
                {
                    Enum.TryParse(permissionListItem.Value, out Permission permission);

                    if (permissionListItem.Selected)
                    {
                        customRole.AddPermission(permission);
                    }
                    else
                    {
                        customRole.RemovePermission(permission);
                    }
                }

                await _db.SaveChangesAsync();
            }
        }
    }
}
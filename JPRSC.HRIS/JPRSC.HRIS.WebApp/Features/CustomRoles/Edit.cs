using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Domain;
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

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly ApplicationDbContext _db;
            private IList<int> _permissionValuesNotShownInMenu = new List<int> { (int)Permission.OvertimeDefault, (int)Permission.EarningDeductionRecordDefault };

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<Command> Handle(Query query, CancellationToken token)
            {
                var command = await _db
                    .CustomRoles
                    .Where(cr => cr.Id == query.CustomRoleId && !cr.DeletedOn.HasValue)
                    .ProjectToSingleAsync<Edit.Command>();

                command.PermissionsList = EnumHelper.GetSelectList(typeof(Permission)).Where(sli => !PermissionHelper.PermissionValuesNotShownInMenu.Contains(Convert.ToInt32(sli.Value))).ToList();

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

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command command, CancellationToken token)
            {
                var customRole = await _db.CustomRoles.SingleAsync(cr => cr.Id == command.Id);

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

                return Unit.Value;
            }
        }
    }
}
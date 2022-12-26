using FluentValidation;
using JPRSC.HRIS.Domain;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace JPRSC.HRIS.Features.CustomRoles
{
    public class Add
    {
        public class Command : IRequest
        {
            private IList<int> _permissionValuesNotShownInMenu = new List<int> { (int)Permission.OvertimeDefault, (int)Permission.EarningDeductionRecordDefault };

            public string Name { get; set; }
            public IList<SelectListItem> PermissionsList { get; set; }

            public Command()
            {
                PermissionsList = EnumHelper.GetSelectList(typeof(Permission)).Where(sli => !PermissionHelper.PermissionValuesNotShownInMenu.Contains(Convert.ToInt32(sli.Value))).ToList();
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
                var customRole = new CustomRole
                {
                    AddedOn = DateTime.UtcNow,
                    Name = command.Name
                };

                foreach (var permissionListItem in command.PermissionsList.Where(pli => pli.Selected))
                {
                    Enum.TryParse(permissionListItem.Value, out Permission permission);
                    customRole.AddPermission(permission);
                }

                _db.CustomRoles.Add(customRole);
                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
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

namespace JPRSC.HRIS.Features.CustomRoles
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

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<CustomRole, Command>();
            }
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly ApplicationDbContext _db;
            private readonly IMapper _mapper;
            private IList<int> _permissionValuesNotShownInMenu = new List<int> { (int)Permission.OvertimeDefault, (int)Permission.EarningDeductionRecordDefault };

            public QueryHandler(ApplicationDbContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Command> Handle(Query query, CancellationToken token)
            {
                var command = await _db
                    .CustomRoles
                    .AsNoTracking()
                    .Where(cr => cr.Id == query.CustomRoleId && !cr.DeletedOn.HasValue)
                    .ProjectTo<Command>(_mapper)
                    .SingleAsync();

                command.PermissionsList = EnumHelper.GetSelectList(typeof(Permission)).Where(sli => !PermissionHelper.PermissionValuesNotShownInMenu.Contains(Convert.ToInt32(sli.Value))).ToList();

                var customRole = await _db
                    .CustomRoles
                    .AsNoTracking()
                    .SingleAsync(cr => cr.Id == query.CustomRoleId && !cr.DeletedOn.HasValue);

                foreach (var permission in customRole.Permissions)
                {
                    var correspondingPermissionListItem = command.PermissionsList.SingleOrDefault(p => p.Value == ((int)permission).ToString());

                    if (correspondingPermissionListItem != null)
                    {
                        correspondingPermissionListItem.Selected = true;
                    }
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
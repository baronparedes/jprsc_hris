﻿using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.WebApp.Infrastructure.Dependency;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

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
            public IList<SelectListItem> RolesList { get; set; } = new List<SelectListItem>();
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
                    .Users
                    .Where(u => u.Id == query.UserId && !u.DeletedOn.HasValue)
                    .ProjectToSingleAsync<Command>();

                var user = await _db
                    .Users
                    .Include(u => u.CustomRoles)
                    .SingleAsync(u => u.Id == query.UserId && !u.DeletedOn.HasValue);

                var customRoles = await _db.CustomRoles.Where(cr => !cr.DeletedOn.HasValue).ToListAsync();

                foreach (var customRole in customRoles)
                {
                    command.RolesList.Add(new SelectListItem
                    {
                        Text = customRole.Name,
                        Value = customRole.Id.ToString(),
                        Selected = user.CustomRoles.Any(cr => cr.Id == customRole.Id)
                    });
                }

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
                var user = await _db.Users.Include(u => u.CustomRoles).SingleAsync(u => u.Id == command.Id);
                user.Name = command.Name;
                user.JobTitle = command.JobTitle;
                user.ModifiedOn = DateTime.UtcNow;
                user.UserName = command.UserName;

                foreach (var roleItem in command.RolesList)
                {
                    var customRoleId = Convert.ToInt32(roleItem.Value);
                    var customRole = await _db.CustomRoles.SingleAsync(cr => cr.Id == customRoleId);

                    if (roleItem.Selected)
                    {
                        user.CustomRoles.Add(customRole);
                    }
                    else
                    {
                        user.CustomRoles.Remove(customRole);
                    }
                }

                await _db.SaveChangesAsync();
            }
        }
    }
}
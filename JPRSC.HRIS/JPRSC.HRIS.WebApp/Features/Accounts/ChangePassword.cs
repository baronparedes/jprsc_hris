﻿using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Infrastructure.Identity;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Accounts
{
    public class ChangePassword
    {
        public class Query : IRequest<Command>
        {
            public string UserId { get; set; }
        }

        public class Command : IRequest<CommandResult>
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
            public string RepeatNewPassword { get; set; }
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
                return await _db.Users.Where(u => u.Id == query.UserId).ProjectToSingleAsync<Command>();
            }
        }

        public class CommandResult
        {
            public string Name { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.NewPassword)
                    .NotEmpty();

                RuleFor(c => c.RepeatNewPassword)
                    .NotEmpty();

                RuleFor(c => c.RepeatNewPassword)
                    .Must(BeTheSameAsPassword)
                    .WithMessage("Repeat New Password must be the same as New Password.");
            }

            private bool BeTheSameAsPassword(Command command, string repeatPassword)
            {
                return repeatPassword == command.NewPassword;
            }
        }

        public class CommandHandler : IAsyncRequestHandler<Command, CommandResult>
        {
            private readonly ApplicationDbContext _db;
            private readonly SignInManager _signInManager;
            private readonly UserManager _userManager;

            public CommandHandler(ApplicationDbContext db, UserManager userManager, SignInManager signInManager)
            {
                _db = db;
                _userManager = userManager;
                _signInManager = signInManager;
            }

            public async Task<CommandResult> Handle(Command command)
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(command.Id, command.OldPassword, command.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    throw new Exception($"Unable to change password. Errors: {changePasswordResult.Errors.Join(",")}");
                }

                return new CommandResult
                {
                    Name = _db.Users.Single(u => u.Id == command.Id).Name
                };
            }
        }
    }
}
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Infrastructure.Identity;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Security;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
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

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<User, Command>().ForAllOtherMembers(opts => opts.Ignore());
            }
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly ApplicationDbContext _db;
            private readonly IMapper _mapper;

            public QueryHandler(ApplicationDbContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Command> Handle(Query query, CancellationToken token)
            {
                return await _db.Users.AsNoTracking().Where(u => u.Id == query.UserId).ProjectTo<Command>(_mapper).SingleAsync();
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

        public class CommandHandler : IRequestHandler<Command, CommandResult>
        {
            private readonly ApplicationDbContext _db;
            private readonly UserManager _userManager;

            public CommandHandler(ApplicationDbContext db, UserManager userManager)
            {
                _db = db;
                _userManager = userManager;
            }

            public async Task<CommandResult> Handle(Command command, CancellationToken token)
            {
                var useOldPassword = !AuthorizeHelper.IsSuperAdmin();
                if (useOldPassword)
                {
                    var changePasswordResult = await _userManager.ChangePasswordAsync(command.Id, command.OldPassword, command.NewPassword);
                    if (!changePasswordResult.Succeeded)
                    {
                        throw new Exception($"Unable to change password. Errors: {changePasswordResult.Errors.Join(",")}");
                    }
                }
                else
                {
                    await _userManager.RemovePasswordAsync(command.Id);
                    await _userManager.AddPasswordAsync(command.Id, command.NewPassword);
                }

                return new CommandResult
                {
                    Name = (await _db.Users.SingleAsync(u => u.Id == command.Id)).Name
                };
            }
        }
    }
}
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JPRSC.HRIS.Features.Accounts
{
    public class EditOwn
    {
        public class Query : IRequest<Command>
        {
        }

        public class Command : IRequest
        {
            public int? CompanyId { get; set; }
            public string Name { get; set; }
            public string UserName { get; set; }
            public IList<SelectListItem> CompaniesList { get; set; } = new List<SelectListItem>();
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
            public string RepeatNewPassword { get; set; }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<User, Command>();
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
                var currentUserId = HttpContext.Current.User.Identity.GetUserId();

                var command = await _db
                    .Users
                    .AsNoTracking()
                    .Where(u => u.Id == currentUserId && !u.DeletedOn.HasValue)
                    .ProjectTo<Command>(_mapper)
                    .SingleAsync();

                command.CompaniesList = await GetCompaniesList(query, currentUserId);

                return command;
            }

            private async Task<IList<SelectListItem>> GetCompaniesList(Query query, string userId)
            {
                var user = await _db.Users.AsNoTracking().Include(u => u.CustomRoles).SingleAsync(u => u.Id == userId && !u.DeletedOn.HasValue);
                var Companies = await _db.Companies.AsNoTracking().Where(cr => !cr.DeletedOn.HasValue).ToListAsync();

                return Companies
                    .Select(cp => new SelectListItem
                    {
                        Text = cp.Name,
                        Value = cp.Id.ToString(),
                        Selected = user.AllowedCompanies.Any(uac => uac.Id == cp.Id)
                    })
                    .ToList();
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly ApplicationDbContext _db;

            public CommandValidator(ApplicationDbContext db)
            {
                _db = db;

                RuleFor(c => c.UserName)
                    .NotEmpty();

                RuleFor(c => c.UserName)
                    .Must(BeUnique)
                    .WithMessage("That username is already taken.");

                When(c => !String.IsNullOrWhiteSpace(c.OldPassword) && !String.IsNullOrWhiteSpace(c.NewPassword), () =>
                {
                    RuleFor(c => c.RepeatNewPassword)
                        .NotEmpty();

                    RuleFor(c => c.RepeatNewPassword)
                        .Must(BeTheSameAsPassword)
                        .WithMessage("Repeat New Password must be the same as New Password.");
                });                    
            }

            private bool BeUnique(Command command, string userName)
            {
                var currentUserId = HttpContext.Current.User.Identity.GetUserId();

                return !_db.Users.Any(u => u.Id != currentUserId && u.UserName == userName);
            }

            private bool BeTheSameAsPassword(Command command, string repeatPassword)
            {
                return repeatPassword == command.NewPassword;
            }
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly ApplicationDbContext _db;
            private readonly JPRSC.HRIS.Infrastructure.Identity.UserManager _userManager;

            public CommandHandler(ApplicationDbContext db, JPRSC.HRIS.Infrastructure.Identity.UserManager userManager)
            {
                _db = db;
                _userManager = userManager;
            }

            public async Task<Unit> Handle(Command command, CancellationToken token)
            {
                var currentUserId = HttpContext.Current.User.Identity.GetUserId();

                var user = await _db.Users.Include(u => u.CustomRoles).SingleAsync(u => u.Id == currentUserId);
                user.CompanyId = command.CompanyId;
                user.Name = command.Name;
                user.ModifiedOn = DateTime.UtcNow;
                user.UserName = command.UserName;

                await _db.SaveChangesAsync();

                if (!String.IsNullOrWhiteSpace(command.OldPassword) && !String.IsNullOrWhiteSpace(command.NewPassword))
                {
                    var changePasswordResult = await _userManager.ChangePasswordAsync(currentUserId, command.OldPassword, command.NewPassword);
                    if (!changePasswordResult.Succeeded)
                    {
                        throw new Exception($"Unable to change password. Errors: {changePasswordResult.Errors.Join(",")}");
                    }
                }

                return Unit.Value;
            }
        }
    }
}
﻿using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;

namespace JPRSC.HRIS.WebApp.Features.SystemSettings
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int SystemSettingsId { get; set; }
        }

        public class Command : IRequest
        {
            public int Id { get; set; }
            public decimal? MinimumDeductionOfContribution { get; set; }
            public decimal? MinimumNetPay { get; set; }

            // Used only to render the view
            public string EmailAddress { get; set; }
            public string Password { get; set; }
            public string Port { get; set; }
            public string Host { get; set; }
            public string TestEmailAddress { get; set; }
            public bool? EnableSendingEmails { get; set; }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Models.SystemSettings, Command>();
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
                if (query.SystemSettingsId == default(int)) query.SystemSettingsId = _db.SystemSettings.Select(pr => pr.Id).FirstOrDefault();

                var command = await _db.SystemSettings.AsNoTracking().Where(r => r.Id == query.SystemSettingsId).ProjectTo<Command>(_mapper).SingleOrDefaultAsync();

                if (command == null)
                {
                    command = new Command
                    {
                        MinimumDeductionOfContribution = 1000,
                        MinimumNetPay = 2000
                    };
                }
                else
                {
                    if (!command.MinimumDeductionOfContribution.HasValue) command.MinimumDeductionOfContribution = 1000;
                    if (!command.MinimumNetPay.HasValue) command.MinimumNetPay = 2000;
                }

                return command;
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.MinimumNetPay)
                    .NotEmpty();

                RuleFor(c => c.MinimumDeductionOfContribution)
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
                var systemSettings = await _db.SystemSettings.SingleOrDefaultAsync(r => r.Id == command.Id);
                if (systemSettings != null)
                {
                    systemSettings.MinimumDeductionOfContribution = command.MinimumDeductionOfContribution;
                    systemSettings.MinimumNetPay = command.MinimumNetPay;
                }
                else
                {
                    _db.SystemSettings.Add(new Models.SystemSettings
                    {
                        MinimumDeductionOfContribution = command.MinimumDeductionOfContribution,
                        MinimumNetPay = command.MinimumNetPay
                    });
                }

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
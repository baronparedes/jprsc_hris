﻿using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.JobTitles
{
    public class Add
    {
        public class Command : IRequest
        {
            public string Name { get; set; }
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
                var jobTitle = new JobTitle
                {
                    AddedOn = DateTime.UtcNow,
                    Name = command.Name
                };

                _db.JobTitles.Add(jobTitle);
                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
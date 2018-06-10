﻿using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.TaxRecords
{
    public class Add
    {
        public class Command : IRequest
        {
            public string Code { get; set; }
            public double? Exemption { get; set; }
            public string Name { get; set; }
            public IList<TaxRange> TaxRanges { get; set; } = new List<TaxRange>();

            public class TaxRange
            {
                public double? Percentage { get; set; }
                public decimal? Plus { get; set; }
                public decimal? Range { get; set; }
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
                var now = DateTime.UtcNow;

                var taxRecord = new TaxRecord
                {
                    AddedOn = now,
                    Code = command.Code,
                    Exemption = command.Exemption,
                    Name = command.Name,
                    TaxRanges = command.TaxRanges.Select(tr => new TaxRange
                    {
                        AddedOn = now,
                        Percentage = tr.Percentage,
                        Plus = tr.Plus,
                        Range = tr.Range
                    }).ToList()
                };

                _db.TaxRecords.Add(taxRecord);
                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
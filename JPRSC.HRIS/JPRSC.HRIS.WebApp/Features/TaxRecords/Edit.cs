using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.TaxRecords
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int TaxRecordId { get; set; }
        }

        public class Command : IRequest
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public double? Exemption { get; set; }
            public string Name { get; set; }
            public IList<TaxRange> TaxRanges { get; set; } = new List<TaxRange>();

            public class TaxRange
            {
                public int Id { get; set; }
                public double? Percentage { get; set; }
                public decimal? Plus { get; set; }
                public decimal? Range { get; set; }
            }
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<Command> Handle(Query query, CancellationToken token)
            {
                return await _db.TaxRecords.Where(r => r.Id == query.TaxRecordId && !r.DeletedOn.HasValue).ProjectToSingleAsync<Command>();
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
                var taxRecord = await _db.TaxRecords.SingleAsync(r => r.Id == command.Id);

                taxRecord.Code = command.Code;
                taxRecord.Exemption = command.Exemption;
                taxRecord.ModifiedOn = now;
                taxRecord.Name = command.Name;

                await ProcessTaxRanges(command, now);

                await _db.SaveChangesAsync();

                return Unit.Value;
            }

            private async Task ProcessTaxRanges(Command command, DateTime addedOrModifiedOn)
            {
                var existingTaxRanges = await _db.TaxRanges.Where(tr => tr.TaxRecordId == command.Id).ToListAsync();
                var existingTaxRangeIds = existingTaxRanges.Select(tr => tr.Id).ToHashSet();
                var inputTaxRangeIds = command.TaxRanges.Select(tr => tr.Id).ToHashSet();

                var newTaxRangeIds = inputTaxRangeIds.Except(existingTaxRangeIds);
                var newTaxRanges = command.TaxRanges.Where(tr => newTaxRangeIds.Contains(tr.Id));
                foreach (var newTaxRange in newTaxRanges)
                {
                    _db.TaxRanges.Add(new TaxRange
                    {
                        AddedOn = addedOrModifiedOn,
                        Percentage = newTaxRange.Percentage,
                        Plus = newTaxRange.Plus,
                        Range = newTaxRange.Range,
                        TaxRecordId = command.Id
                    });
                }

                var updatedTaxRangeIds = inputTaxRangeIds.Intersect(existingTaxRangeIds);
                var taxRangesForUpdating = existingTaxRanges.Where(tr => updatedTaxRangeIds.Contains(tr.Id));
                foreach (var taxRangeForUpdating in taxRangesForUpdating)
                {
                    var updatedTaxRange = command.TaxRanges.Single(tr => tr.Id == taxRangeForUpdating.Id);
                    taxRangeForUpdating.ModifiedOn = addedOrModifiedOn;
                    taxRangeForUpdating.Percentage = updatedTaxRange.Percentage;
                    taxRangeForUpdating.Plus = updatedTaxRange.Plus;
                    taxRangeForUpdating.Range = updatedTaxRange.Range;
                }

                var deletedTaxRangeIds = existingTaxRangeIds.Except(inputTaxRangeIds);
                var deletedTaxRanges = existingTaxRanges.Where(tr => deletedTaxRangeIds.Contains(tr.Id));
                foreach (var deletedTaxRange in deletedTaxRanges)
                {
                    deletedTaxRange.DeletedOn = addedOrModifiedOn;
                }
            }
        }
    }
}
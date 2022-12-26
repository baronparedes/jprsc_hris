using AutoMapper;
using AutoMapper.QueryableExtensions;
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

namespace JPRSC.HRIS.Features.TaxStatuses
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int TaxStatusId { get; set; }
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
                public decimal? From { get; set; }
                public int Id { get; set; }
                public double? Percentage { get; set; }
                public decimal? Plus { get; set; }
                public decimal? To { get; set; }
            }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<TaxStatus, Command>();
                CreateMap<TaxRange, Command.TaxRange>();
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
                return await _db.TaxStatuses.AsNoTracking().Where(r => r.Id == query.TaxStatusId && !r.DeletedOn.HasValue).ProjectTo<Command>(_mapper).SingleAsync();
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
                var taxStatus = await _db.TaxStatuses.SingleAsync(r => r.Id == command.Id);

                taxStatus.Code = command.Code;
                taxStatus.Exemption = command.Exemption;
                taxStatus.ModifiedOn = now;
                taxStatus.Name = command.Name;

                await ProcessTaxRanges(command, now);

                await _db.SaveChangesAsync();

                return Unit.Value;
            }

            private async Task ProcessTaxRanges(Command command, DateTime addedOrModifiedOn)
            {
                var existingTaxRanges = await _db.TaxRanges.Where(tr => tr.TaxStatusId == command.Id).ToListAsync();
                var existingTaxRangeIds = new HashSet<int>(existingTaxRanges.Select(tr => tr.Id));
                var inputTaxRangeIds = new HashSet<int>(command.TaxRanges.Select(tr => tr.Id));

                var newTaxRangeIds = inputTaxRangeIds.Except(existingTaxRangeIds);
                var newTaxRanges = command.TaxRanges.Where(tr => newTaxRangeIds.Contains(tr.Id));
                foreach (var newTaxRange in newTaxRanges)
                {
                    _db.TaxRanges.Add(new TaxRange
                    {
                        AddedOn = addedOrModifiedOn,
                        From = newTaxRange.From,
                        Percentage = newTaxRange.Percentage,
                        Plus = newTaxRange.Plus,
                        TaxStatusId = command.Id,
                        To = newTaxRange.To
                    });
                }

                var updatedTaxRangeIds = inputTaxRangeIds.Intersect(existingTaxRangeIds);
                var taxRangesForUpdating = existingTaxRanges.Where(tr => updatedTaxRangeIds.Contains(tr.Id));
                foreach (var taxRangeForUpdating in taxRangesForUpdating)
                {
                    var updatedTaxRange = command.TaxRanges.Single(tr => tr.Id == taxRangeForUpdating.Id);
                    taxRangeForUpdating.From = updatedTaxRange.From;
                    taxRangeForUpdating.ModifiedOn = addedOrModifiedOn;
                    taxRangeForUpdating.Percentage = updatedTaxRange.Percentage;
                    taxRangeForUpdating.Plus = updatedTaxRange.Plus;
                    taxRangeForUpdating.To = updatedTaxRange.To;
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
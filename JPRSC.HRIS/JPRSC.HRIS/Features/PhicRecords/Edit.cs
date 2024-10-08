﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.PhicRecords
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int PhicRecordId { get; set; }
        }

        public class Command : IRequest
        {
            public double? EmployeePercentageShare { get; set; }
            public int Id { get; set; }
            public decimal? MaximumDeduction { get; set; }
            public decimal? MinimumDeduction { get; set; }
            public double? Percentage { get; set; }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<PhicRecord, Command>();
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
                if (query.PhicRecordId == default(int)) query.PhicRecordId = _db.PhicRecords.Select(pr => pr.Id).First();

                var command = await _db.PhicRecords.AsNoTracking().Where(r => r.Id == query.PhicRecordId && !r.DeletedOn.HasValue).ProjectTo<Command>(_mapper).SingleAsync();

                return command;
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.Percentage)
                    .NotEmpty();

                RuleFor(c => c.MaximumDeduction)
                    .NotEmpty();

                RuleFor(c => c.MinimumDeduction)
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
                var phicRecord = await _db.PhicRecords.SingleAsync(r => r.Id == command.Id);

                phicRecord.EmployeePercentageShare = command.EmployeePercentageShare;
                phicRecord.MaximumDeduction = command.MaximumDeduction;
                phicRecord.MinimumDeduction = command.MinimumDeduction;
                phicRecord.ModifiedOn = DateTime.UtcNow;
                phicRecord.Percentage = command.Percentage;

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
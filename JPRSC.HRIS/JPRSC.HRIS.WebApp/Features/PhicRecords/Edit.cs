using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.PhicRecords
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
            public double? Percentage { get; set; }
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
                if (query.PhicRecordId == default(int)) query.PhicRecordId = _db.PhicRecords.Select(pr => pr.Id).First();

                var command = await _db.PhicRecords.Where(r => r.Id == query.PhicRecordId && !r.DeletedOn.HasValue).ProjectToSingleAsync<Command>();

                return command;
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {

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
                phicRecord.ModifiedOn = DateTime.UtcNow;
                phicRecord.Percentage = command.Percentage;

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
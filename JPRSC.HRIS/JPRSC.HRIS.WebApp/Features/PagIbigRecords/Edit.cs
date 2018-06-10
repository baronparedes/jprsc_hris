using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.PagIbigRecords
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int PagIbigRecordId { get; set; }
        }

        public class Command : IRequest
        {
            public bool? ApplyToSalary { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public decimal? EmployeeAmount { get; set; }
            public double? EmployeePercentage { get; set; }
            public decimal? EmployerAmount { get; set; }
            public double? EmployerPercentage { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
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
                return await _db.PagIbigRecords.Where(r => r.Id == query.PagIbigRecordId && !r.DeletedOn.HasValue).ProjectToSingleAsync<Command>();
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.Code)
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
                if (!command.ApplyToSalary.HasValue) command.ApplyToSalary = false;

                var pagIbigRecord = await _db.PagIbigRecords.SingleAsync(r => r.Id == command.Id);

                pagIbigRecord.ApplyToSalary = command.ApplyToSalary;
                pagIbigRecord.Code = command.Code;
                pagIbigRecord.Description = command.Description;
                pagIbigRecord.EmployeeAmount = command.EmployeeAmount;
                pagIbigRecord.EmployeePercentage = command.EmployeePercentage;
                pagIbigRecord.EmployerAmount = command.EmployerAmount;
                pagIbigRecord.EmployerPercentage = command.EmployerPercentage;
                pagIbigRecord.ModifiedOn = DateTime.UtcNow;
                pagIbigRecord.Name = command.Name;

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
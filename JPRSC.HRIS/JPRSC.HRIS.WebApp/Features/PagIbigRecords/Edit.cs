using AutoMapper;
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
            public decimal? DeductionAmount { get; set; }
            public double? EmployeePercentage { get; set; }
            public double? EmployerPercentage { get; set; }
            public decimal? MinimumDeduction { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<PagIbigRecord, Command>();
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
                return await _db.PagIbigRecords.AsNoTracking().Where(r => r.Id == query.PagIbigRecordId && !r.DeletedOn.HasValue).ProjectTo<Command>(_mapper).SingleAsync();
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.Code)
                    .NotEmpty();

                RuleFor(c => c.DeductionAmount)
                    .NotEmpty()
                    .GreaterThan(0);

                RuleFor(c => c.EmployeePercentage)
                    .NotEmpty()
                    .GreaterThan(0);

                RuleFor(c => c.EmployerPercentage)
                    .NotEmpty()
                    .GreaterThan(0);

                RuleFor(c => c.MinimumDeduction)
                    .NotEmpty()
                    .GreaterThan(0);
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
                pagIbigRecord.DeductionAmount = command.DeductionAmount;
                pagIbigRecord.EmployeePercentage = command.EmployeePercentage;
                pagIbigRecord.MinimumDeduction = command.MinimumDeduction;
                pagIbigRecord.EmployerPercentage = command.EmployerPercentage;
                pagIbigRecord.ModifiedOn = DateTime.UtcNow;
                pagIbigRecord.Name = command.Name;

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
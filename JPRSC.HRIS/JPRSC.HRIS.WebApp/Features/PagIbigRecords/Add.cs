using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.PagIbigRecords
{
    public class Add
    {
        public class Command : IRequest
        {
            public bool? ApplyToSalary { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public decimal? DeductionAmount { get; set; }
            public double? EmployeePercentage { get; set; }
            public double? EmployerPercentage { get; set; }
            public decimal? MinimumDeduction { get; set; }
            public string Name { get; set; }
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

                var pagIbigRecord = new PagIbigRecord
                {
                    AddedOn = DateTime.UtcNow,
                    ApplyToSalary = command.ApplyToSalary,
                    Code = command.Code,
                    Description = command.Description,
                    DeductionAmount = command.DeductionAmount,
                    EmployeePercentage = command.EmployeePercentage,
                    MinimumDeduction = command.MinimumDeduction,
                    EmployerPercentage = command.EmployerPercentage,
                    Name = command.Name
                };

                _db.PagIbigRecords.Add(pagIbigRecord);
                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
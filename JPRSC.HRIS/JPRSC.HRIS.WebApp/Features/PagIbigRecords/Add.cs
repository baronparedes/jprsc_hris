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
            public decimal? EmployeeAmount { get; set; }
            public double? EmployeePercentage { get; set; }
            public decimal? EmployerAmount { get; set; }
            public double? EmployerPercentage { get; set; }
            public string Name { get; set; }
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

                var pagIbigRecord = new PagIbigRecord
                {
                    AddedOn = DateTime.UtcNow,
                    ApplyToSalary = command.ApplyToSalary,
                    Code = command.Code,
                    Description = command.Description,
                    EmployeeAmount = command.EmployeeAmount,
                    EmployeePercentage = command.EmployeePercentage,
                    EmployerAmount = command.EmployerAmount,
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
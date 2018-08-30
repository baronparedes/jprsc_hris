using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Payroll
{
    public class AddForProcessingBatch
    {
        public class Command : IRequest
        {
            public int ClientId { get; set; }
            public string EmployeeIds { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.ClientId)
                    .NotEmpty()
                    .GreaterThan(0);

                RuleFor(c => c.EmployeeIds)
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
                var dateFormatted = $"{DateTime.Now:MM/dd/yyyy}";
                var existingForProcessingBatch = await _db
                    .ForProcessingBatches
                    .SingleOrDefaultAsync(fpb => fpb.ClientId == command.ClientId && fpb.DateFormatted == dateFormatted);

                var now = DateTime.UtcNow;

                if (existingForProcessingBatch != null)
                {
                    existingForProcessingBatch.EmployeeIds = command.EmployeeIds;
                    existingForProcessingBatch.ModifiedOn = now;
                    existingForProcessingBatch.ProcessedOn = now;
                }
                else
                {
                    var client = await _db.Clients.SingleOrDefaultAsync(c => !c.DeletedOn.HasValue && c.Id == command.ClientId);

                    var forProcessingBatch = new ForProcessingBatch
                    {
                        AddedOn = now,
                        ClientId = command.ClientId,
                        DateFormatted = dateFormatted,
                        EmployeeIds = command.EmployeeIds,
                        Name = $"{dateFormatted} {client.Name}",
                        ProcessedOn = now
                    };
                    _db.ForProcessingBatches.Add(forProcessingBatch);
                }

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
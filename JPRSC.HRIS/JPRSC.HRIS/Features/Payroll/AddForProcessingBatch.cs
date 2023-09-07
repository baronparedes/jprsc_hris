using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.Payroll
{
    public class AddForProcessingBatch
    {
        public class Command : IRequest
        {
            public int ClientId { get; set; }
            public string EmployeeIds { get; set; }
            public DateTime? PayrollPeriodFrom { get; set; }
            public Month? PayrollPeriodMonth { get; set; }
            public DateTime? PayrollPeriodTo { get; set; }
            public bool Overwrite { get; set; }
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

                RuleFor(c => c.PayrollPeriodFrom)
                    .NotEmpty();

                RuleFor(c => c.PayrollPeriodTo)
                    .NotEmpty();

                RuleFor(c => c.PayrollPeriodMonth)
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
                var existingForProcessingBatchCount = await _db
                    .ForProcessingBatches
                    .CountAsync(fpb => fpb.ClientId == command.ClientId && fpb.DateFormatted == dateFormatted);

                var client = await _db.Clients.AsNoTracking().SingleOrDefaultAsync(c => !c.DeletedOn.HasValue && c.Id == command.ClientId);
                var newBatchName = GetBatchName(client, command.PayrollPeriodMonth.Value, command.PayrollPeriodFrom.Value, command.PayrollPeriodTo.Value);
                var batchName = GetBatchName(dateFormatted, client, existingForProcessingBatchCount);

                var now = DateTime.UtcNow;
                
                var forProcessingBatch = new ForProcessingBatch
                {
                    AddedOn = now,
                    ClientId = command.ClientId,
                    DateFormatted = dateFormatted,
                    EmployeeIds = command.EmployeeIds,
                    Name = batchName,
                    ProcessedOn = now
                };
                _db.ForProcessingBatches.Add(forProcessingBatch);

                await _db.SaveChangesAsync();

                return Unit.Value;
            }

            private static string GetBatchName(string dateFormatted, Client client, int existingForProcessingBatchCount)
            {
                string name;

                if (existingForProcessingBatchCount == 0)
                {
                    name = $"{dateFormatted} {client.Name}";
                }
                else
                {
                    name = $"{dateFormatted} {client.Name} - {existingForProcessingBatchCount}";
                }

                return name;
            }

            private static string GetBatchName(Client client, Month payrollPeriodMonth, DateTime payrollPeriodFrom, DateTime payrollPeriodTo)
            {
                return $"{client.Name}-{payrollPeriodMonth}-{payrollPeriodFrom:MM/dd}-{payrollPeriodTo:MM/dd-yyyy}";
            }
        }
    }
}
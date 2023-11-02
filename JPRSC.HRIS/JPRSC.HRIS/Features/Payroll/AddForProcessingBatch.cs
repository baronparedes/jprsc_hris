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
        public class Command : IRequest<CommandResult>
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

        public class CommandResult
        {
            public bool HasExisting { get; set; }
            public bool Overwritten { get; set; }
        }

        public class CommandHandler : IRequestHandler<Command, CommandResult>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<CommandResult> Handle(Command command, CancellationToken token)
            {
                var dateFormatted = $"{DateTime.Now:MM/dd/yyyy}";

                var client = await _db.Clients.AsNoTracking().SingleOrDefaultAsync(c => !c.DeletedOn.HasValue && c.Id == command.ClientId);
                var batchName = GetBatchName(client, command.PayrollPeriodMonth.Value, command.PayrollPeriodFrom.Value, command.PayrollPeriodTo.Value);

                var existingBatchWithSameName = await _db.ForProcessingBatches.SingleOrDefaultAsync(fpb => fpb.ClientId == command.ClientId && fpb.Name == batchName);
                var hasExisting = existingBatchWithSameName != null;
                var now = DateTime.UtcNow;

                if (!hasExisting)
                {
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

                    return new CommandResult
                    {
                        HasExisting = false,
                        Overwritten = false
                    };
                }

                if (!command.Overwrite)
                {
                    return new CommandResult
                    {
                        HasExisting = true,
                        Overwritten = false
                    };
                }

                existingBatchWithSameName.DateFormatted = dateFormatted;
                existingBatchWithSameName.EmployeeIds = command.EmployeeIds;
                existingBatchWithSameName.ModifiedOn = now;
                existingBatchWithSameName.ProcessedOn = now;

                await _db.SaveChangesAsync();

                return new CommandResult
                {
                    HasExisting = true,
                    Overwritten = true
                };
            }

            private static string GetBatchName(Client client, Month payrollPeriodMonth, DateTime payrollPeriodFrom, DateTime payrollPeriodTo)
            {
                return $"{client.Name}-{payrollPeriodMonth}-{payrollPeriodFrom:MM/dd}-{payrollPeriodTo:MM/dd-yyyy}";
            }
        }
    }
}
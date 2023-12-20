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

namespace JPRSC.HRIS.Features.Payroll
{
    public class AddForProcessingBatch
    {
        public class Command : IRequest
        {
            public int ClientId { get; set; }
            public string EmployeeIds { get; set; }
            public IList<int> EmployeeIdsList => String.IsNullOrWhiteSpace(EmployeeIds) ? new List<int>() : EmployeeIds.Split(',').Select(id => Convert.ToInt32(id)).ToList();
            public DateTime? PayrollPeriodFrom { get; set; }
            public Month? PayrollPeriodMonth { get; set; }
            public DateTime? PayrollPeriodTo { get; set; }
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
                    .CountAsync(fpb => fpb.ClientId == command.ClientId && fpb.DateFormatted == dateFormatted && fpb.PayrollPeriodMonth == command.PayrollPeriodMonth && fpb.PayrollPeriodFrom == command.PayrollPeriodFrom && fpb.PayrollPeriodTo == command.PayrollPeriodTo);
                
                var client = await _db.Clients.AsNoTracking().SingleOrDefaultAsync(c => !c.DeletedOn.HasValue && c.Id == command.ClientId);
                var batchName = GetBatchName(client, command.PayrollPeriodMonth.Value, command.PayrollPeriodFrom.Value, command.PayrollPeriodTo.Value, dateFormatted, existingForProcessingBatchCount);
                
                var now = DateTime.UtcNow;

                var forProcessingBatch = new ForProcessingBatch
                {
                    AddedOn = now,
                    ClientId = command.ClientId,
                    DateFormatted = dateFormatted,
                    EmployeeIds = command.EmployeeIds,
                    Name = batchName,
                    ProcessedOn = now,
                    PayrollPeriodMonth = command.PayrollPeriodMonth,
                    PayrollPeriodFrom = command.PayrollPeriodFrom,
                    PayrollPeriodTo = command.PayrollPeriodTo
                };
                _db.ForProcessingBatches.Add(forProcessingBatch);

                await _db.SaveChangesAsync();

                return Unit.Value;
            }

            private static string GetBatchName(Client client, Month payrollPeriodMonth, DateTime payrollPeriodFrom, DateTime payrollPeriodTo, string dateFormatted, int existingForProcessingBatchCount)
            {
                var baseName = $"{dateFormatted} - {client.Name}-{payrollPeriodMonth}-{payrollPeriodFrom:MM/dd}-{payrollPeriodTo:MM/dd-yyyy}";

                if (existingForProcessingBatchCount > 0)
                {
                    baseName += $" - {existingForProcessingBatchCount}";
                }

                return baseName;
            }
        }
    }
}
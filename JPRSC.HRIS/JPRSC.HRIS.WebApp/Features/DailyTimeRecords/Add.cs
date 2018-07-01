using Dapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.DailyTimeRecords
{
    public class Add
    {
        public class Command : IRequest
        {
            public double? DaysWorked { get; set; }
            public int? EmployeeId { get; set; }
            public double? HoursLate { get; set; }
            public double? HoursUndertime { get; set; }
            public double? HoursWorked { get; set; }
            public IEnumerable<Overtime> Overtimes { get; set; } = new List<Overtime>();

            public class Overtime
            {
                public DateTime? From { get; set; }
                public double? NumberOfHours { get; set; }
                public decimal? NumberOfHoursValue { get; set; }
                public string PayPercentageName { get; set; }
                public double? PayPercentagePercentage { get; set; }
                public string Reference { get; set; }
                public DateTime? To { get; set; }
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.EmployeeId)
                    .NotEmpty();

                When(c => c.DaysWorked.HasValue, () =>
                {
                    RuleFor(c => c.DaysWorked)
                        .GreaterThanOrEqualTo(0);
                });

                When(c => c.HoursWorked.HasValue, () =>
                {
                    RuleFor(c => c.HoursWorked)
                        .GreaterThanOrEqualTo(0);
                });

                When(c => c.HoursLate.HasValue, () =>
                {
                    RuleFor(c => c.HoursLate)
                        .GreaterThanOrEqualTo(0);
                });

                When(c => c.HoursUndertime.HasValue, () =>
                {
                    RuleFor(c => c.HoursUndertime)
                        .GreaterThanOrEqualTo(0);
                });
            }
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly ApplicationDbContext _db;

            public object ExecuteAsync { get; private set; }

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command command, CancellationToken token)
            {
                // TODO: Remove
                await RemoveExistingDailyTimeRecordsOfEmployee(command);

                var now = DateTime.UtcNow;
                var employee = await _db.Employees.SingleOrDefaultAsync(e => e.Id == command.EmployeeId);

                var dailyTimeRecord = new DailyTimeRecord
                {
                    AddedOn = now,
                    COLADailyValue = GetValue(command.DaysWorked, employee.COLADaily),
                    DailyRate = employee.DailyRate,
                    DaysWorked = command.DaysWorked,
                    DaysWorkedValue = GetValue(command.DaysWorked, employee.DailyRate),
                    EmployeeId = command.EmployeeId,
                    HourlyRate = employee.HourlyRate,
                    HoursLate = command.HoursLate,
                    HoursLateValue = GetValue(command.HoursLate, employee.HourlyRate),
                    HoursUndertime = command.HoursUndertime,
                    HoursUndertimeValue = GetValue(command.HoursUndertime, employee.HourlyRate),
                    HoursWorked = command.HoursWorked,
                    HoursWorkedValue = GetValue(command.HoursWorked, employee.HourlyRate)
                };
                _db.DailyTimeRecords.Add(dailyTimeRecord);

                var overtimes = command
                    .Overtimes
                    .Select(o => new Overtime
                    {
                        AddedOn = now,
                        EmployeeId = command.EmployeeId,
                        From = o.From,
                        NumberOfHours = o.NumberOfHours,
                        NumberOfHoursValue = o.NumberOfHoursValue,
                        PayPercentageName = o.PayPercentageName,
                        PayPercentagePercentage = o.PayPercentagePercentage,
                        Reference = o.Reference,
                        To = o.To
                    });
                _db.Overtimes.AddRange(overtimes);

                await _db.SaveChangesAsync();

                return Unit.Value;
            }

            private async Task RemoveExistingDailyTimeRecordsOfEmployee(Command command)
            {
                using (var connection = new SqlConnection(ConnectionStrings.ApplicationDbContext))
                {
                    var deleteCommand = "DELETE FROM DailyTimeRecords WHERE EmployeeId = @EmployeeId";

                    await connection.ExecuteAsync(deleteCommand, new { command.EmployeeId });
                }
            }

            private decimal? GetValue(double? quantity, decimal? rate)
            {
                return quantity.HasValue && rate.HasValue ? (decimal)quantity.Value * rate.Value : (decimal?)null;
            }
        }
    }
}
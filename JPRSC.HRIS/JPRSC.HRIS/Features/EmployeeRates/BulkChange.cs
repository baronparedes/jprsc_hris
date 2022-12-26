using AutoMapper;
using Dapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.EmployeeRates
{
    public class BulkChange
    {
        public class Command : IRequest
        {
            public int ClientId { get; set; }
            public decimal? COLADaily { get; set; }
            public decimal? COLAHourly { get; set; }
            public decimal? COLAMonthly { get; set; }
            public decimal? DailyRate { get; set; }
            public decimal? HourlyRate { get; set; }
            public decimal? MonthlyRate { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.ClientId)
                    .GreaterThan(0)
                    .WithMessage("Please choose a client.");
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
                using (var connection = new SqlConnection(ConnectionStrings.ApplicationDbContext))
                {
                    var updateCommand = "UPDATE [Employees] " +
                        "SET HourlyRate = @HourlyRate, DailyRate = @DailyRate, MonthlyRate = @MonthlyRate, COLAHourly = @COLAHourly, COLADaily = @COLADaily, COLAMonthly = @COLAMonthly " +
                        "WHERE ClientId = @ClientId AND DeletedOn IS NULL AND IsActive = 1";

                    await connection.ExecuteAsync(updateCommand, new { HourlyRate = command.HourlyRate, DailyRate = command.DailyRate, MonthlyRate = command.MonthlyRate, COLAHourly = command.COLAHourly, COLADaily = command.COLADaily, COLAMonthly = command.COLAMonthly, ClientId = command.ClientId });
                }

                return Unit.Value;
            }
        }
    }
}
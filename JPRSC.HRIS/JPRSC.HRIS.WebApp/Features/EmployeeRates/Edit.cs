using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.EmployeeRates
{
    public class Edit
    {
        public class Command : IRequest
        {
            public decimal? COLADaily { get; set; }
            public decimal? COLAHourly { get; set; }
            public decimal? DailyRate { get; set; }
            public string EmployeeCode { get; set; }
            public string FirstName { get; set; }
            public decimal? HourlyRate { get; set; }
            public int Id { get; set; }
            public string LastName { get; set; }
            public decimal? MonthlyRate { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
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
                var employee = await _db.Employees.SingleAsync(r => r.Id == command.Id);

                employee.COLADaily = command.COLADaily;
                employee.COLAHourly = command.COLAHourly;
                employee.DailyRate = command.DailyRate;
                employee.HourlyRate = command.HourlyRate;
                employee.ModifiedOn = DateTime.UtcNow;
                employee.MonthlyRate = command.MonthlyRate;

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
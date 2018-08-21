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
            public IEnumerable<EarningDeductionRecord> EarningDeductionRecords { get; set; } = new List<EarningDeductionRecord>();
            public int? EmployeeId { get; set; }
            public double? MinutesLate { get; set; }
            public double? MinutesUndertime { get; set; }
            public double? HoursWorked { get; set; }
            public IEnumerable<Overtime> Overtimes { get; set; } = new List<Overtime>();
            public DateTime? PayrollPeriodFrom { get; set; }
            public DateTime? PayrollPeriodTo { get; set; }
            public int? PayrollProcessBatchPayrollPeriodBasisId { get; set; }

            public class Overtime
            {
                public double? NumberOfHours { get; set; }
                public decimal? NumberOfHoursValue { get; set; }
                public int? PayPercentageId { get; set; }
                public string PayPercentageName { get; set; }
                public double? PayPercentagePercentage { get; set; }
                public string Reference { get; set; }
            }

            public class EarningDeductionRecord
            {
                public decimal? Amount { get; set; }
                public int? EarningDeductionId { get; set; }
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

                When(c => c.MinutesLate.HasValue, () =>
                {
                    RuleFor(c => c.MinutesLate)
                        .GreaterThanOrEqualTo(0);
                });

                When(c => c.MinutesUndertime.HasValue, () =>
                {
                    RuleFor(c => c.MinutesUndertime)
                        .GreaterThanOrEqualTo(0);
                });

                When(c => !c.PayrollProcessBatchPayrollPeriodBasisId.HasValue, () =>
                {
                    RuleFor(c => c.PayrollPeriodFrom)
                        .NotEmpty();

                    RuleFor(c => c.PayrollPeriodTo)
                        .NotEmpty();
                });

                When(c => c.PayrollPeriodFrom.HasValue && c.PayrollPeriodTo.HasValue, () =>
                {
                    RuleFor(c => c.PayrollPeriodFrom)
                        .Must(BeBeforePayrollPeriodTo)
                        .WithMessage("Payroll Period From must precede Payroll Period To.");
                });
            }

            private bool BeBeforePayrollPeriodTo(Command command, DateTime? payrollPeriodFrom)
            {
                return payrollPeriodFrom.Value.Date < command.PayrollPeriodTo.Value.Date;
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
                if (command.PayrollProcessBatchPayrollPeriodBasisId.HasValue)
                {
                    var payrollProcessBatchPayrollPeriodBasis = await _db.PayrollProcessBatches.FindAsync(command.PayrollProcessBatchPayrollPeriodBasisId.Value);
                    command.PayrollPeriodFrom = payrollProcessBatchPayrollPeriodBasis.PayrollPeriodFrom;
                    command.PayrollPeriodTo = payrollProcessBatchPayrollPeriodBasis.PayrollPeriodTo;
                }

                var now = DateTime.UtcNow;
                var employee = await _db.Employees.SingleOrDefaultAsync(e => e.Id == command.EmployeeId);

                decimal? colaHourlyOTValue = 0;

                var existingOvertimes = await _db
                    .Overtimes
                    .Where(o => !o.DeletedOn.HasValue && o.EmployeeId == command.EmployeeId && o.PayrollPeriodFrom == command.PayrollPeriodFrom && o.PayrollPeriodTo == command.PayrollPeriodTo)
                    .ToListAsync();

                foreach (var overtimeUpload in command.Overtimes)
                {
                    var existingOvertime = existingOvertimes.SingleOrDefault(o => o.PayPercentageId == o.PayPercentageId);

                    if (existingOvertime != null)
                    {
                        existingOvertime.ModifiedOn = now;
                        existingOvertime.NumberOfHours = overtimeUpload.NumberOfHours;
                        existingOvertime.NumberOfHoursValue = overtimeUpload.NumberOfHoursValue;
                        existingOvertime.PayPercentageName = overtimeUpload.PayPercentageName;
                        existingOvertime.PayPercentagePercentage = overtimeUpload.PayPercentagePercentage;
                    }
                    else
                    {
                        var overtime = new Overtime
                        {
                            AddedOn = now,
                            EmployeeId = command.EmployeeId,
                            From = command.PayrollPeriodFrom,
                            NumberOfHours = overtimeUpload.NumberOfHours,
                            NumberOfHoursValue = overtimeUpload.NumberOfHoursValue,
                            PayPercentageName = overtimeUpload.PayPercentageName,
                            PayPercentagePercentage = overtimeUpload.PayPercentagePercentage,
                            PayrollPeriodFrom = command.PayrollPeriodFrom,
                            PayrollPeriodTo = command.PayrollPeriodTo,
                            Reference = overtimeUpload.Reference,
                            To = command.PayrollPeriodTo
                        };
                        _db.Overtimes.Add(overtime);
                    }

                    colaHourlyOTValue += GetValue(overtimeUpload.NumberOfHours, employee.COLAHourly);
                }

                var existingDailyTimeRecord = _db.DailyTimeRecords.SingleOrDefault(dtr => !dtr.DeletedOn.HasValue && dtr.EmployeeId == employee.Id && dtr.PayrollPeriodFrom == command.PayrollPeriodFrom && dtr.PayrollPeriodTo == command.PayrollPeriodTo);
                if (existingDailyTimeRecord != null)
                {
                    existingDailyTimeRecord.COLADailyValue = GetValue(command.DaysWorked, employee.COLADaily);
                    existingDailyTimeRecord.COLAHourlyValue = GetValue(command.HoursWorked, employee.COLAHourly);
                    existingDailyTimeRecord.COLAHourlyOTValue = colaHourlyOTValue;
                    existingDailyTimeRecord.DailyRate = employee.DailyRate;
                    existingDailyTimeRecord.DaysWorked = command.DaysWorked;
                    existingDailyTimeRecord.DaysWorkedValue = GetValue(command.DaysWorked, employee.DailyRate);
                    existingDailyTimeRecord.EmployeeId = command.EmployeeId;
                    existingDailyTimeRecord.HourlyRate = employee.HourlyRate;
                    existingDailyTimeRecord.HoursLate = command.MinutesLate / 60;
                    existingDailyTimeRecord.HoursLateValue = GetValue(command.MinutesLate / 60, employee.HourlyRate);
                    existingDailyTimeRecord.HoursUndertime = command.MinutesUndertime / 60;
                    existingDailyTimeRecord.HoursUndertimeValue = GetValue(command.MinutesUndertime / 60, employee.HourlyRate);
                    existingDailyTimeRecord.HoursWorked = command.HoursWorked;
                    existingDailyTimeRecord.HoursWorkedValue = GetValue(command.HoursWorked, employee.HourlyRate);
                }
                else
                {
                    var dailyTimeRecord = new DailyTimeRecord
                    {
                        AddedOn = now,
                        COLADailyValue = GetValue(command.DaysWorked, employee.COLADaily),
                        COLAHourlyValue = GetValue(command.HoursWorked, employee.COLAHourly),
                        COLAHourlyOTValue = colaHourlyOTValue,
                        DailyRate = employee.DailyRate,
                        DaysWorked = command.DaysWorked,
                        DaysWorkedValue = GetValue(command.DaysWorked, employee.DailyRate),
                        EmployeeId = command.EmployeeId,
                        HourlyRate = employee.HourlyRate,
                        HoursLate = command.MinutesLate / 60,
                        HoursLateValue = GetValue(command.MinutesLate / 60, employee.HourlyRate),
                        HoursUndertime = command.MinutesUndertime / 60,
                        HoursUndertimeValue = GetValue(command.MinutesUndertime / 60, employee.HourlyRate),
                        HoursWorked = command.HoursWorked,
                        HoursWorkedValue = GetValue(command.HoursWorked, employee.HourlyRate),
                        PayrollPeriodFrom = command.PayrollPeriodFrom,
                        PayrollPeriodTo = command.PayrollPeriodTo
                    };
                    _db.DailyTimeRecords.Add(dailyTimeRecord);
                }

                var existingEarningDeductionRecords = await _db
                    .EarningDeductionRecords
                    .Where(edr => !edr.DeletedOn.HasValue && edr.EmployeeId == command.EmployeeId && edr.PayrollPeriodFrom == command.PayrollPeriodFrom && edr.PayrollPeriodTo == command.PayrollPeriodTo)
                    .ToListAsync();

                foreach (var earningDeductionRecordUpload in command.EarningDeductionRecords)
                {
                    var existingEarningDeductionRecord = existingEarningDeductionRecords
                        .SingleOrDefault(edr => edr.EarningDeductionId == earningDeductionRecordUpload.EarningDeductionId);

                    if (existingEarningDeductionRecord != null)
                    {
                        existingEarningDeductionRecord.Amount = earningDeductionRecordUpload.Amount;
                        existingEarningDeductionRecord.ModifiedOn = now;
                    }
                    else
                    {
                        var earningDeductionRecord = new EarningDeductionRecord
                        {
                            AddedOn = now,
                            Amount = earningDeductionRecordUpload.Amount,
                            EarningDeductionId = earningDeductionRecordUpload.EarningDeductionId,
                            EmployeeId = command.EmployeeId,
                            PayrollPeriodFrom = command.PayrollPeriodFrom,
                            PayrollPeriodTo = command.PayrollPeriodTo,
                        };
                        _db.EarningDeductionRecords.Add(earningDeductionRecord);
                    }
                }

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
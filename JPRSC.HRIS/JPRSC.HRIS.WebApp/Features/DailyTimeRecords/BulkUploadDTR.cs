using Dapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Dependency;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace JPRSC.HRIS.WebApp.Features.DailyTimeRecords
{
    public class BulkUploadDTR
    {
        public class Command : IRequest<CommandResult>
        {
            public int? ClientId { get; set; }
            public HttpPostedFileBase File { get; set; }
            public DateTime? PayrollPeriodFrom { get; set; }
            public DateTime? PayrollPeriodTo { get; set; }
            public int? PayrollProcessBatchPayrollPeriodBasisId { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly ApplicationDbContext _db = DependencyConfig.Instance.Container.GetInstance<ApplicationDbContext>();

            public CommandValidator()
            {
                RuleFor(c => c.ClientId)
                    .NotEmpty();

                RuleFor(c => c.File)
                    .NotNull();

                When(c => c.ClientId.HasValue && c.PayrollPeriodFrom.HasValue && c.PayrollPeriodTo.HasValue, () =>
                {
                    RuleFor(c => c.ClientId)
                        .Must(NotHaveEndProcessedYet)
                        .WithMessage("End process for this payroll period has finished.");
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

            private bool NotHaveEndProcessedYet(Command command, int? clientId)
            {
                var endProcessRecord = _db.PayrollProcessBatches.SingleOrDefault(ppb => !ppb.DeletedOn.HasValue && !ppb.DateOverwritten.HasValue && ppb.EndProcessedOn.HasValue && ppb.ClientId == clientId && ppb.PayrollPeriodFrom == command.PayrollPeriodFrom && ppb.PayrollPeriodTo == command.PayrollPeriodTo);
                return endProcessRecord == null;
            }

            private bool BeBeforePayrollPeriodTo(Command command, DateTime? payrollPeriodFrom)
            {
                return payrollPeriodFrom.Value.Date < command.PayrollPeriodTo.Value.Date;
            }
        }

        public class CommandResult
        {
            public IEnumerable<UnprocessedItem> UnprocessedItems { get; set; } = new List<UnprocessedItem>();
            public IEnumerable<UnprocessedItem> SkippedItems { get; set; } = new List<UnprocessedItem>();
            public int ProcessedItemsCount { get; set; }

            public class UnprocessedItem
            {
                public string FirstName { get; set; }
                public string LastName { get; set; }
                public string Reason { get; set; }
            }
        }

        public class CommandHandler : IRequestHandler<Command, CommandResult>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<CommandResult> Handle(Command command, CancellationToken cancellationToken)
            {
                if (command.PayrollProcessBatchPayrollPeriodBasisId.HasValue)
                {
                    var payrollProcessBatchPayrollPeriodBasis = await _db.PayrollProcessBatches.FindAsync(command.PayrollProcessBatchPayrollPeriodBasisId.Value);
                    command.PayrollPeriodFrom = payrollProcessBatchPayrollPeriodBasis.PayrollPeriodFrom;
                    command.PayrollPeriodTo = payrollProcessBatchPayrollPeriodBasis.PayrollPeriodTo;
                }

                var now = DateTime.UtcNow;
                var unprocessedItems = new List<CommandResult.UnprocessedItem>();
                var allPayRates = await _db.PayPercentages.AsNoTracking().ToListAsync();
                var client = await _db.Clients.SingleOrDefaultAsync(c => !c.DeletedOn.HasValue && c.Id == command.ClientId);
                var allEmployeesOfClient = await _db.Employees.AsNoTracking().Where(e => !e.DeletedOn.HasValue && e.ClientId == command.ClientId).ToListAsync();
                var allEmployeesOfClientIds = allEmployeesOfClient.Select(e => e.Id).ToList();

                var allEmployeeDailyTimeRecordsForPayrollPeriod = await _db.DailyTimeRecords.Where(dtr => allEmployeesOfClientIds.Contains(dtr.EmployeeId.Value) && !dtr.DeletedOn.HasValue && dtr.PayrollPeriodFrom == command.PayrollPeriodFrom && dtr.PayrollPeriodTo == command.PayrollPeriodTo).ToListAsync();
                var allEmployeeOvertimesForPayrollPeriod = await _db.Overtimes.Where(ot => allEmployeesOfClientIds.Contains(ot.EmployeeId.Value) && !ot.DeletedOn.HasValue && ot.PayrollPeriodFrom == command.PayrollPeriodFrom && ot.PayrollPeriodTo == command.PayrollPeriodTo).ToListAsync();

                var csvData = GetCSVData(command);
                var columnToPayPercentageMap = await GetColumnToPayPercentageMap(csvData.Item1);
                var processedItemsCount = 0;

                var skippedItems = new List<CommandResult.UnprocessedItem>();
                skippedItems.AddRange(allEmployeesOfClient.Where(e => String.IsNullOrWhiteSpace(e.EmployeeCode)).Select(e => new CommandResult.UnprocessedItem { FirstName = e.FirstName, LastName = e.LastName, Reason = "No employee code" }));

                var emptyEmployeeCode = csvData.Item2.Count(r => String.IsNullOrWhiteSpace(r[0]));

                // Upload behavior: all-or-nothing
                foreach (var line in csvData.Item2)
                {
                    var employeeCode = String.IsNullOrWhiteSpace(line[0]) ? null : line[0].Trim();
                    if (String.IsNullOrWhiteSpace(employeeCode)) continue;

                    var lastName = String.IsNullOrWhiteSpace(line[1]) ? null : line[1].Trim();
                    var firstName = String.IsNullOrWhiteSpace(line[2]) ? null : line[2].Trim();

                    double? daysOrMonthsWorked = null, hoursWorked = null, minutesLate = null, daysOrMinutesUndertime = null;

                    try
                    {
                        daysOrMonthsWorked = line[3].ToNullableDouble();
                        hoursWorked = line[4].ToNullableDouble();
                        minutesLate = line[5].ToNullableDouble();
                        daysOrMinutesUndertime = line[6].ToNullableDouble();
                    }
                    catch
                    {
                        unprocessedItems.Add(new CommandResult.UnprocessedItem
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            Reason = $"Unable to interpret line with employee code \"{employeeCode}\". Please make sure there are no extra commas or other characters."
                        });

                        continue;
                    }

                    Employee employee = null;

                    try
                    {
                        employee = allEmployeesOfClient.SingleOrDefault(e => !String.IsNullOrWhiteSpace(e.EmployeeCode) && String.Equals(e.EmployeeCode.Trim(), employeeCode, StringComparison.CurrentCultureIgnoreCase));
                        if (employee == null)
                        {
                            // Try finding the employee using employee code with trimmed leading zeroes
                            employee = allEmployeesOfClient.SingleOrDefault(e => !String.IsNullOrWhiteSpace(e.EmployeeCode) && String.Equals(e.EmployeeCode.Trim().TrimStart('0'), employeeCode.TrimStart('0'), StringComparison.CurrentCultureIgnoreCase));
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == "Sequence contains more than one matching element")
                        {
                            unprocessedItems.Add(new CommandResult.UnprocessedItem
                            {
                                FirstName = firstName,
                                LastName = lastName,
                                Reason = $"There is more than one employee found with employee code {employeeCode}."
                            });

                            continue;
                        }
                    }
                    
                    if (employee == null)
                    {
                        unprocessedItems.Add(new CommandResult.UnprocessedItem
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            Reason = $"Employee \"{employeeCode}\" not found."
                        });

                        continue;
                    }

                    if (unprocessedItems.Any()) continue;

                    processedItemsCount += 1;

                    decimal? colaHourlyOTValue = 0;

                    var overtimesToAdd = new List<Overtime>();

                    var existingOvertimeIdsToDelete = new List<int>();

                    foreach (KeyValuePair<int, PayPercentage> entry in columnToPayPercentageMap.Where(kvp => kvp.Value != null))
                    {
                        double? hours = null;

                        try
                        {
                            hours = line[entry.Key].ToNullableDouble();
                        }
                        catch
                        {
                            unprocessedItems.Add(new CommandResult.UnprocessedItem
                            {
                                FirstName = firstName,
                                LastName = lastName,
                                Reason = $"Unable to interpret line with employee code \"{employeeCode}\". Please make sure there are no extra commas or other characters."
                            });

                            break;
                        }

                        if (!hours.HasValue) continue;

                        var payPercentage = entry.Value;

                        var existingOvertimes = allEmployeeOvertimesForPayrollPeriod.OrderByDescending(o => o.AddedOn).Where(ot => ot.EmployeeId == employee.Id && ot.PayPercentageId == payPercentage.Id);
                        if (existingOvertimes.Count() > 1)
                        {
                            var existingOvertimeToDelete = existingOvertimes.Skip(1);
                            existingOvertimeIdsToDelete.AddRange(existingOvertimeToDelete.Select(ot => ot.Id));
                        }

                        var existingOvertime = existingOvertimes.FirstOrDefault(ot => ot.EmployeeId == employee.Id && ot.PayPercentageId == payPercentage.Id);
                        if (existingOvertime != null)
                        {
                            existingOvertime.ModifiedOn = now;
                            existingOvertime.NumberOfHours = hours;
                            existingOvertime.NumberOfHoursValue = (decimal?)hours * employee.HourlyRate * (decimal?)(payPercentage.Percentage / 100);
                            existingOvertime.PayPercentageName = payPercentage.Name;
                            existingOvertime.PayPercentagePercentage = payPercentage.Percentage;
                        }
                        else
                        {
                            var overtime = new Overtime
                            {
                                AddedOn = now,
                                EmployeeId = employee.Id,
                                NumberOfHours = hours,
                                NumberOfHoursValue = (decimal?)hours * employee.HourlyRate * (decimal?)(payPercentage.Percentage / 100),
                                PayPercentageId = payPercentage.Id,
                                PayPercentageName = payPercentage.Name,
                                PayPercentagePercentage = payPercentage.Percentage,
                                PayrollPeriodFrom = command.PayrollPeriodFrom,
                                PayrollPeriodTo = command.PayrollPeriodTo
                            };
                            overtimesToAdd.Add(overtime);
                        }

                        if (payPercentage.IncludeCOLA == true)
                        {
                            colaHourlyOTValue += (decimal?)hours * employee.COLAHourly;
                        }
                    }

                    if (overtimesToAdd.Any())
                    {
                        _db.Overtimes.AddRange(overtimesToAdd);
                    }

                    if (existingOvertimeIdsToDelete.Any())
                    {
                        using (var connection = new SqlConnection(ConnectionStrings.ApplicationDbContext))
                        {
                            var deleteCommand = "DELETE FROM Overtimes WHERE Id in @Ids";
                            var args = new { Ids = existingOvertimeIdsToDelete };

                            await connection.ExecuteAsync(deleteCommand, args);
                        }
                    }

                    var existingDailyTimeRecord = allEmployeeDailyTimeRecordsForPayrollPeriod.SingleOrDefault(dtr => dtr.EmployeeId == employee.Id);
                    if (existingDailyTimeRecord != null)
                    {
                        existingDailyTimeRecord.COLADailyValue = (decimal?)daysOrMonthsWorked * employee.COLADaily;
                        existingDailyTimeRecord.COLAHourlyValue = (decimal?)hoursWorked * employee.COLAHourly;
                        existingDailyTimeRecord.COLAHourlyOTValue = colaHourlyOTValue;
                        existingDailyTimeRecord.DailyRate = employee.DailyRate;
                        existingDailyTimeRecord.DaysWorked = daysOrMonthsWorked;
                        existingDailyTimeRecord.DaysWorkedValue =  (decimal?)daysOrMonthsWorked * (client.PayrollCode == PayrollCode.Monthly ? employee.MonthlyRate : employee.DailyRate);
                        existingDailyTimeRecord.HourlyRate = employee.HourlyRate;
                        existingDailyTimeRecord.HoursLate = minutesLate / 60;
                        existingDailyTimeRecord.HoursLateValue = (decimal?)(minutesLate / 60) * employee.HourlyRate;
                        existingDailyTimeRecord.HoursUndertime = client.PayrollCode == PayrollCode.Monthly ? daysOrMinutesUndertime : daysOrMinutesUndertime / 60;
                        existingDailyTimeRecord.HoursUndertimeValue = client.PayrollCode == PayrollCode.Monthly ? (decimal?)daysOrMinutesUndertime * employee.DailyRate : (decimal?)(daysOrMinutesUndertime / 60) * employee.HourlyRate;
                        existingDailyTimeRecord.HoursWorked = hoursWorked;
                        existingDailyTimeRecord.HoursWorkedValue = (decimal?)hoursWorked * employee.HourlyRate;
                        existingDailyTimeRecord.ModifiedOn = now;
                        existingDailyTimeRecord.MonthlyRate = employee.MonthlyRate;
                    }
                    else
                    {
                        var dailyTimeRecord = new DailyTimeRecord
                        {
                            AddedOn = now,
                            COLADailyValue = (decimal?)daysOrMonthsWorked * employee.COLADaily,
                            COLAHourlyValue = (decimal?)hoursWorked * employee.COLAHourly,
                            COLAHourlyOTValue = colaHourlyOTValue,
                            DailyRate = employee.DailyRate,
                            DaysWorked = daysOrMonthsWorked,
                            DaysWorkedValue = (decimal?)daysOrMonthsWorked * (client.PayrollCode == PayrollCode.Monthly ? employee.MonthlyRate : employee.DailyRate),
                            EmployeeId = employee.Id,
                            HourlyRate = employee.HourlyRate,
                            HoursLate = minutesLate / 60,
                            HoursLateValue = (decimal?)(minutesLate / 60) * employee.HourlyRate,
                            HoursUndertime = client.PayrollCode == PayrollCode.Monthly ? daysOrMinutesUndertime : daysOrMinutesUndertime / 60,
                            HoursUndertimeValue = client.PayrollCode == PayrollCode.Monthly ? (decimal?)daysOrMinutesUndertime * employee.DailyRate : (decimal?)(daysOrMinutesUndertime / 60) * employee.HourlyRate,
                            HoursWorked = hoursWorked,
                            HoursWorkedValue = (decimal?)hoursWorked * employee.HourlyRate,
                            MonthlyRate = employee.MonthlyRate,
                            PayrollPeriodFrom = command.PayrollPeriodFrom,
                            PayrollPeriodTo = command.PayrollPeriodTo
                        };
                        _db.DailyTimeRecords.Add(dailyTimeRecord);
                    }
                }

                if (!unprocessedItems.Any())
                {
                    await _db.SaveChangesAsync();
                }

                return new CommandResult
                {
                    UnprocessedItems = unprocessedItems,
                    ProcessedItemsCount = unprocessedItems.Any() ? 0 : processedItemsCount,
                    SkippedItems = skippedItems
                };
            }

            private async Task<Overtime> GetOvertime(Command command, DateTime addedOrModifiedOn, Employee employee, PayPercentage payRate)
            {
                Overtime theOvertime = null;

                var existingOvertime = await _db
                    .Overtimes
                    .SingleOrDefaultAsync(o => !o.DeletedOn.HasValue && o.EmployeeId == employee.Id && o.PayrollPeriodFrom == command.PayrollPeriodFrom && o.PayrollPeriodTo == command.PayrollPeriodTo && o.PayPercentageId == payRate.Id);

                if (existingOvertime != null)
                {
                    theOvertime = existingOvertime;
                    theOvertime.ModifiedOn = addedOrModifiedOn;
                    theOvertime.PayPercentageName = payRate.Name;
                    theOvertime.PayPercentagePercentage = payRate.Percentage;
                }
                else
                {
                    theOvertime = new Overtime
                    {
                        AddedOn = addedOrModifiedOn,
                        EmployeeId = employee.Id,
                        PayPercentageId = payRate.Id,
                        PayPercentageName = payRate.Name,
                        PayPercentagePercentage = payRate.Percentage,
                        PayrollPeriodFrom = command.PayrollPeriodFrom,
                        PayrollPeriodTo = command.PayrollPeriodTo
                    };
                }

                return theOvertime;
            }

            private Tuple<IList<string>, IList<IList<string>>> GetCSVData(Command command)
            {
                IList<string> header = new List<string>();
                IList<IList<string>> body = new List<IList<string>>();
                var headerPopulated = false;

                using (var csvreader = new StreamReader(command.File.InputStream))
                {
                    while (!csvreader.EndOfStream)
                    {
                        var line = csvreader.ReadLine();
                        var lineAsColumns = line.Split(',');

                        if (!headerPopulated)
                        {
                            header = lineAsColumns;
                            headerPopulated = true;
                        }
                        else
                        {
                            if (!IsBlankLine(lineAsColumns))
                            {
                                body.Add(lineAsColumns);
                            }
                        }
                    }
                }

                IList<IList<string>> reversedBody = new List<IList<string>>();
                var employeeCodes = new List<string>(body.Count);
                for (var i = body.Count - 1; i >= 0; i--)
                {
                    var employeeCode = body[i][0];
                    if (!employeeCodes.Contains(employeeCode))
                    {
                        reversedBody.Add(body[i]);
                        employeeCodes.Add(employeeCode);
                    }
                }

                return Tuple.Create(header, reversedBody);
            }

            private bool IsBlankLine(IEnumerable<string> items)
            {
                return items.All(i => String.IsNullOrWhiteSpace(i));
            }

            private async Task<Dictionary<int, PayPercentage>> GetColumnToPayPercentageMap(IList<string> header)
            {
                var allPayPercentages = await _db.PayPercentages.ToListAsync();
                var ppStartingColumnIndexInCSV = 7;
                var columnToPayPercentageIdMap = new Dictionary<int, PayPercentage>();

                for (var i = ppStartingColumnIndexInCSV; i < header.Count; i++)
                {
                    var headerValue = String.IsNullOrWhiteSpace(header[i]) ? null : header[i].Trim();
                    var matchingPayPercentage = allPayPercentages.SingleOrDefault(pp => String.Equals(pp.Code, headerValue, StringComparison.InvariantCultureIgnoreCase));
                    columnToPayPercentageIdMap.Add(i, matchingPayPercentage);
                }

                return columnToPayPercentageIdMap;
            }
        }
    }
}
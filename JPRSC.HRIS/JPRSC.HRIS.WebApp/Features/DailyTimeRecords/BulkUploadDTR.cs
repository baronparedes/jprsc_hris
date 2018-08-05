﻿using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Dependency;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            public IList<IList<string>> Lines { get; set; } = new List<IList<string>>();
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

                When(c => c.File != null, () =>
                {
                    RuleFor(c => c.File)
                        .Must(HaveValidNumberOfColumns)
                        .WithMessage("Unrecognized number of columns. Please use these columns: Employee Code, Last Name, First Name, Days Worked, Minutes Worked, Minutes Late, Minutes Undertime, ROT, ND, NDOT, SH, NDSH, SHOT, NDSHOT, LH, NDLH, LHOT, NDLHOT, DOD, DODOT, SHDOD, SHDODOT, LHDOD, LHDODOT, ADJ_PAY, EXC_DOD, HOLIDAY, ND_LHX8, ND_LHRDX8, ND_LHRDF8, ND_LHF8, NDX8, NDF8, ND_RDX8, ND_RDF8, ND_SHX8, ND_SHRDX8, ND_SHRDF8, ND_SHF8, ND_B2020, OT_LHX8, OT_LHRDX8, OT_LHRDF8, OT_LHF8, OT_RDX8, OT_RDF8, OT_SHF8M, OT_SHX8, OT_SHRDX8, OT_SHRDF8, OT_SHF8, UWLH_B2020");
                });

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

            private bool HaveValidNumberOfColumns(Command command, HttpPostedFileBase file)
            {
                // Set the lines here so we don't have to deal with the stream later on
                var hasValidNumberOfColumns = false;
                var numberOfColumns = 52;

                using (var csvreader = new StreamReader(file.InputStream))
                {
                    while (!csvreader.EndOfStream)
                    {
                        var line = csvreader.ReadLine();
                        var lineAsColumns = line.Split(',');
                        command.Lines.Add(lineAsColumns);

                        if (lineAsColumns.Count() == numberOfColumns)
                        {
                            hasValidNumberOfColumns = true;
                        }
                    }
                }

                return hasValidNumberOfColumns;
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
                var allPayRates = await _db.PayPercentages.ToListAsync();
                var allEmployeesOfClient = await _db.Employees.Where(e => !e.DeletedOn.HasValue && e.ClientId == command.ClientId).ToListAsync();
                var csvData = GetCSVData(command);
                var columnToPayPercentageMap = await GetColumnToPayPercentageMap(csvData.Item1);
                var processedItemsCount = 0;

                var skippedItems = new List<CommandResult.UnprocessedItem>();
                skippedItems.AddRange(allEmployeesOfClient.Where(e => String.IsNullOrWhiteSpace(e.EmployeeCode)).Select(e => new CommandResult.UnprocessedItem { FirstName = e.FirstName, LastName = e.LastName, Reason = "No employee code" }));

                // Upload behavior: all-or-nothing
                foreach (var line in csvData.Item2)
                {
                    var employeeCode = String.IsNullOrWhiteSpace(line[0]) ? null : line[0].Trim();
                    var lastName = String.IsNullOrWhiteSpace(line[1]) ? null : line[1].Trim();
                    var firstName = String.IsNullOrWhiteSpace(line[2]) ? null : line[2].Trim();
                    var daysWorked = line[3].ToNullableDouble();
                    var minutesWorked = line[4].ToNullableDouble();
                    var minutesLate = line[5].ToNullableDouble();
                    var minutesUndertime = line[6].ToNullableDouble();

                    var employee = allEmployeesOfClient.SingleOrDefault(e => !e.DeletedOn.HasValue && !String.IsNullOrWhiteSpace(e.EmployeeCode) && String.Equals(e.EmployeeCode.Trim().TrimStart('0'), employeeCode.TrimStart('0'), StringComparison.CurrentCultureIgnoreCase));
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

                    var existingDailyTimeRecord = _db.DailyTimeRecords.SingleOrDefault(dtr => !dtr.DeletedOn.HasValue && dtr.EmployeeId == employee.Id && dtr.PayrollPeriodFrom == command.PayrollPeriodFrom && dtr.PayrollPeriodTo == command.PayrollPeriodTo);
                    if (existingDailyTimeRecord != null)
                    {
                        existingDailyTimeRecord.COLADailyValue = (decimal?)daysWorked * employee.COLADaily;
                        existingDailyTimeRecord.DailyRate = employee.DailyRate;
                        existingDailyTimeRecord.DaysWorked = daysWorked;
                        existingDailyTimeRecord.DaysWorkedValue = (decimal?)daysWorked * employee.DailyRate;
                        existingDailyTimeRecord.HourlyRate = employee.HourlyRate;
                        existingDailyTimeRecord.HoursLate = minutesLate / 60;
                        existingDailyTimeRecord.HoursLateValue = (decimal?)(minutesLate / 60) * employee.HourlyRate;
                        existingDailyTimeRecord.HoursUndertime = minutesUndertime / 60;
                        existingDailyTimeRecord.HoursUndertimeValue = (decimal?)(minutesUndertime / 60) * employee.HourlyRate;
                        existingDailyTimeRecord.HoursWorked = minutesWorked / 60;
                        existingDailyTimeRecord.HoursWorkedValue = (decimal?)(minutesWorked / 60) * employee.HourlyRate;
                        existingDailyTimeRecord.ModifiedOn = now;
                    }
                    else
                    {
                        var dailyTimeRecord = new DailyTimeRecord
                        {
                            AddedOn = now,
                            COLADailyValue = (decimal?)daysWorked * employee.COLADaily,
                            DailyRate = employee.DailyRate,
                            DaysWorked = daysWorked,
                            DaysWorkedValue = (decimal?)daysWorked * employee.DailyRate,
                            EmployeeId = employee.Id,
                            HourlyRate = employee.HourlyRate,
                            HoursLate = minutesLate / 60,
                            HoursLateValue = (decimal?)(minutesLate / 60) * employee.HourlyRate,
                            HoursUndertime = minutesUndertime / 60,
                            HoursUndertimeValue = (decimal?)(minutesUndertime / 60) * employee.HourlyRate,
                            HoursWorked = minutesWorked / 60,
                            HoursWorkedValue = (decimal?)(minutesWorked / 60) * employee.HourlyRate,
                            PayrollPeriodFrom = command.PayrollPeriodFrom,
                            PayrollPeriodTo = command.PayrollPeriodTo
                        };
                        _db.DailyTimeRecords.Add(dailyTimeRecord);
                    }

                    foreach (KeyValuePair<int, PayPercentage> entry in columnToPayPercentageMap.Where(kvp => kvp.Value != null))
                    {
                        var hours = line[entry.Key].ToNullableDouble() / 60;
                        var payPercentage = entry.Value;

                        var existingOvertime = await _db.Overtimes.SingleOrDefaultAsync(ot => !ot.DeletedOn.HasValue && ot.EmployeeId == employee.Id && ot.PayrollPeriodFrom == command.PayrollPeriodFrom && ot.PayrollPeriodTo == command.PayrollPeriodTo && ot.PayPercentageId == payPercentage.Id);
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
                            _db.Overtimes.Add(overtime);
                        }
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

                return Tuple.Create(header, body);
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
                    var matchingPayPercentage = allPayPercentages.SingleOrDefault(ed => String.Equals(ed.Name, headerValue, StringComparison.InvariantCultureIgnoreCase));
                    columnToPayPercentageIdMap.Add(i, matchingPayPercentage);
                }

                return columnToPayPercentageIdMap;
            }
        }
    }
}
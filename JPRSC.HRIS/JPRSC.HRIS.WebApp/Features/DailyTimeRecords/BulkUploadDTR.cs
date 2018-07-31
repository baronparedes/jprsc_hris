using FluentValidation;
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
                        .WithMessage("Unrecognized number of columns. Please use these columns: Employee Code, Last Name, First Name, Days Worked, Hours Worked, Hours Late, Hours Undertime, ROT, ND, NDOT, SH, NDSH, SHOT, NDSHOT, LH, NDLH, LHOT, NDLHOT, DOD, DODOT, SHDOD, SHDODOT, LHDOD, LHDODOT, ADJ_PAY, EXC_DOD, HOLIDAY, ND_LHX8, ND_LHRDX8, ND_LHRDF8, ND_LHF8, NDX8, NDF8, ND_RDX8, ND_RDF8, ND_SHX8, ND_SHRDX8, ND_SHRDF8, ND_SHF8, ND_B2020, OT_LHX8, OT_LHRDX8, OT_LHRDF8, OT_LHF8, OT_RDX8, OT_RDF8, OT_SHF8M, OT_SHX8, OT_SHRDX8, OT_SHRDF8, OT_SHF8, UWLH_B2020");
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

                var unprocessedItems = new List<CommandResult.UnprocessedItem>();
                var uploadItems = GetUploadItemsFromUploadedFile(command);
                var allPayRates = await _db.PayPercentages.ToListAsync();
                var allEmployeesOfClient = await _db.Employees.Where(e => !e.DeletedOn.HasValue && e.ClientId == command.ClientId).ToListAsync();
                var now = DateTime.UtcNow;

                // Upload behavior: all-or-nothing
                foreach (var uploadItem in uploadItems)
                {
                    var employee = allEmployeesOfClient.SingleOrDefault(e => !e.DeletedOn.HasValue && String.Equals(e.EmployeeCode.Trim().TrimStart('0'), uploadItem.EmployeeCode.TrimStart('0'), StringComparison.CurrentCultureIgnoreCase));
                    if (employee == null)
                    {
                        unprocessedItems.Add(new CommandResult.UnprocessedItem
                        {
                            FirstName = uploadItem.FirstName,
                            LastName = uploadItem.LastName,
                            Reason = $"Employee \"{uploadItem.EmployeeCode}\" not found."
                        });

                        continue;
                    }

                    if (unprocessedItems.Any()) continue;
                    
                    var existingDailyTimeRecord = _db.DailyTimeRecords.SingleOrDefault(dtr => !dtr.DeletedOn.HasValue && dtr.EmployeeId == employee.Id && dtr.PayrollPeriodFrom == command.PayrollPeriodFrom && dtr.PayrollPeriodTo == command.PayrollPeriodTo);
                    if (existingDailyTimeRecord != null)
                    {
                        existingDailyTimeRecord.COLADailyValue = (decimal?)uploadItem.DaysWorked * employee.COLADaily;
                        existingDailyTimeRecord.DailyRate = employee.DailyRate;
                        existingDailyTimeRecord.DaysWorked = uploadItem.DaysWorked;
                        existingDailyTimeRecord.DaysWorkedValue = (decimal?)uploadItem.DaysWorked * employee.DailyRate;
                        existingDailyTimeRecord.HourlyRate = employee.HourlyRate;
                        existingDailyTimeRecord.HoursLate = uploadItem.HoursLate;
                        existingDailyTimeRecord.HoursLateValue = (decimal?)uploadItem.HoursLate * employee.HourlyRate;
                        existingDailyTimeRecord.HoursUndertime = uploadItem.HoursUndertime;
                        existingDailyTimeRecord.HoursUndertimeValue = (decimal?)uploadItem.HoursUndertime * employee.HourlyRate;
                        existingDailyTimeRecord.HoursWorked = uploadItem.HoursWorked;
                        existingDailyTimeRecord.HoursWorkedValue = (decimal?)uploadItem.HoursWorked * employee.HourlyRate;
                        existingDailyTimeRecord.ModifiedOn = now;
                    }
                    else
                    {
                        var dailyTimeRecord = new DailyTimeRecord
                        {
                            AddedOn = now,
                            COLADailyValue = (decimal?)uploadItem.DaysWorked * employee.COLADaily,
                            DailyRate = employee.DailyRate,
                            DaysWorked = uploadItem.DaysWorked,
                            DaysWorkedValue = (decimal?)uploadItem.DaysWorked * employee.DailyRate,
                            EmployeeId = employee.Id,
                            HourlyRate = employee.HourlyRate,
                            HoursLate = uploadItem.HoursLate,
                            HoursLateValue = (decimal?)uploadItem.HoursLate * employee.HourlyRate,
                            HoursUndertime = uploadItem.HoursUndertime,
                            HoursUndertimeValue = (decimal?)uploadItem.HoursUndertime * employee.HourlyRate,
                            HoursWorked = uploadItem.HoursWorked,
                            HoursWorkedValue = (decimal?)uploadItem.HoursWorked * employee.HourlyRate,
                            PayrollPeriodFrom = command.PayrollPeriodFrom,
                            PayrollPeriodTo = command.PayrollPeriodTo
                        };
                        _db.DailyTimeRecords.Add(dailyTimeRecord);
                    }

                    if (uploadItem.HoursROT.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ROT");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursROT;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursROT * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursND.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursND;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursND * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursNDOT.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "NDOT");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursNDOT;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursNDOT * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursSH.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "SH");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursSH;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursSH * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursNDSH.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "NDSH");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursNDSH;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursNDSH * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursSHOT.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "SHOT");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursSHOT;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursSHOT * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursNDSHOT.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "NDSHOT");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursNDSHOT;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursNDSHOT * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursLH.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "LH");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursLH;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursLH * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursNDLH.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "NDLH");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursNDLH;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursNDLH * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursLHOT.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "LHOT");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursLHOT;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursLHOT * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursNDLHOT.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "NDLHOT");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursNDLHOT;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursNDLHOT * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursDOD.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "DOD");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursDOD;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursDOD * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursDODOT.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "DODOT");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursDODOT;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursDODOT * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursSHDOD.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "SHDOD");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursSHDOD;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursSHDOD * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursSHDODOT.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "SHDODOT");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursSHDODOT;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursSHDODOT * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursLHDOD.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "LHDOD");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursLHDOD;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursLHDOD * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursLHDODOT.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "LHDODOT");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursLHDODOT;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursLHDODOT * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursADJ_PAY.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ADJ_PAY");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursADJ_PAY;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursADJ_PAY * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursEXC_DOD.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "EXC_DOD");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursEXC_DOD;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursEXC_DOD * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursHOLIDAY.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "HOLIDAY");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursHOLIDAY;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursHOLIDAY * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursND_LHX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_LHX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursND_LHX8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursND_LHX8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursND_LHRDX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_LHRDX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursND_LHRDX8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursND_LHRDX8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursND_LHRDF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_LHRDF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursND_LHRDF8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursND_LHRDF8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursND_LHF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_LHF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursND_LHF8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursND_LHF8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursNDX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "NDX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursNDX8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursNDX8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursNDF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "NDF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursNDF8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursNDF8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursND_RDX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_RDX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursND_RDX8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursND_RDX8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursND_RDF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_RDF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursND_RDF8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursND_RDF8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursND_SHX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_SHX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursND_SHX8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursND_SHX8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursND_SHRDX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_SHRDX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursND_SHRDX8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursND_SHRDX8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursND_SHRDF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_SHRDF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursND_SHRDF8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursND_SHRDF8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursND_SHF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_SHF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursND_SHF8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursND_SHF8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursND_B2020.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_B2020");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursND_B2020;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursND_B2020 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursOT_LHX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_LHX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursOT_LHX8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursOT_LHX8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursOT_LHRDX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_LHRDX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursOT_LHRDX8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursOT_LHRDX8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursOT_LHRDF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_LHRDF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursOT_LHRDF8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursOT_LHRDF8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursOT_LHF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_LHF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursOT_LHF8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursOT_LHF8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursOT_RDX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_RDX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursOT_RDX8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursOT_RDX8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursOT_RDF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_RDF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursOT_RDF8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursOT_RDF8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursOT_SHF8M.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_SHF8M");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursOT_SHF8M;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursOT_SHF8M * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursOT_SHX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_SHX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursOT_SHX8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursOT_SHX8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursOT_SHRDX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_SHRDX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursOT_SHRDX8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursOT_SHRDX8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursOT_SHRDF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_SHRDF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursOT_SHRDF8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursOT_SHRDF8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursOT_SHF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_SHF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursOT_SHF8;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursOT_SHF8 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.HoursUWLH_B2020.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "UWLH_B2020");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.HoursUWLH_B2020;
                        overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursUWLH_B2020 * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }
                }

                if (!unprocessedItems.Any())
                {
                    await _db.SaveChangesAsync();
                }

                return new CommandResult
                {
                    UnprocessedItems = unprocessedItems,
                    ProcessedItemsCount = unprocessedItems.Any() ? 0 : uploadItems.Count()
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

            private IEnumerable<UploadItem> GetUploadItemsFromUploadedFile(Command request)
            {
                var uploadItems = new List<UploadItem>();

                for (var i = 0; i < request.Lines.Count; i++)
                {
                    if (i == 0 || IsBlankLine(request.Lines[i])) continue;

                    uploadItems.Add(new UploadItem(request.Lines[i]));
                }

                return uploadItems;
            }

            private bool IsBlankLine(IEnumerable<string> items)
            {
                return items.All(i => String.IsNullOrWhiteSpace(i));
            }
        }

        public class UploadItem
        {
            public UploadItem(IList<string> items)
            {
                EmployeeCode = String.IsNullOrWhiteSpace(items[0]) ? null : items[0].Trim();
                LastName = String.IsNullOrWhiteSpace(items[1]) ? null : items[1].Trim();
                FirstName = String.IsNullOrWhiteSpace(items[2]) ? null : items[2].Trim();

                DaysWorked = items[3].ToNullableDouble();
                HoursWorked = items[4].ToNullableDouble();
                HoursLate = items[5].ToNullableDouble();
                HoursUndertime = items[6].ToNullableDouble();

                HoursROT = items[7].ToNullableDouble();

                HoursND = items[8].ToNullableDouble();
                HoursNDOT = items[9].ToNullableDouble();

                HoursSH = items[10].ToNullableDouble();
                HoursNDSH = items[11].ToNullableDouble();

                HoursSHOT = items[12].ToNullableDouble();
                HoursNDSHOT = items[13].ToNullableDouble();

                HoursLH = items[14].ToNullableDouble();
                HoursNDLH = items[15].ToNullableDouble();

                HoursLHOT = items[16].ToNullableDouble();
                HoursNDLHOT = items[17].ToNullableDouble();

                HoursDOD = items[18].ToNullableDouble();
                HoursDODOT = items[19].ToNullableDouble();

                HoursSHDOD = items[20].ToNullableDouble();
                HoursSHDODOT = items[21].ToNullableDouble();

                HoursLHDOD = items[22].ToNullableDouble();
                HoursLHDODOT = items[23].ToNullableDouble();

                HoursADJ_PAY = items[24].ToNullableDouble();
                HoursEXC_DOD = items[25].ToNullableDouble();
                HoursHOLIDAY = items[26].ToNullableDouble();
                HoursND_LHX8 = items[27].ToNullableDouble();
                HoursND_LHRDX8 = items[28].ToNullableDouble();
                HoursND_LHRDF8 = items[29].ToNullableDouble();
                HoursND_LHF8 = items[30].ToNullableDouble();
                HoursNDX8 = items[31].ToNullableDouble();
                HoursNDF8 = items[32].ToNullableDouble();
                HoursND_RDX8 = items[33].ToNullableDouble();
                HoursND_RDF8 = items[34].ToNullableDouble();
                HoursND_SHX8 = items[35].ToNullableDouble();
                HoursND_SHRDX8 = items[36].ToNullableDouble();
                HoursND_SHRDF8 = items[37].ToNullableDouble();
                HoursND_SHF8 = items[38].ToNullableDouble();
                HoursND_B2020 = items[39].ToNullableDouble();
                HoursOT_LHX8 = items[40].ToNullableDouble();
                HoursOT_LHRDX8 = items[41].ToNullableDouble();
                HoursOT_LHRDF8 = items[42].ToNullableDouble();
                HoursOT_LHF8 = items[43].ToNullableDouble();
                HoursOT_RDX8 = items[44].ToNullableDouble();
                HoursOT_RDF8 = items[45].ToNullableDouble();
                HoursOT_SHF8M = items[46].ToNullableDouble();
                HoursOT_SHX8 = items[47].ToNullableDouble();
                HoursOT_SHRDX8 = items[48].ToNullableDouble();
                HoursOT_SHRDF8 = items[49].ToNullableDouble();
                HoursOT_SHF8 = items[50].ToNullableDouble();
                HoursUWLH_B2020 = items[51].ToNullableDouble();
            }

            public string EmployeeCode { get; set; }
            public string LastName { get; }
            public string FirstName { get; }
            public double? DaysWorked { get; }
            public double? HoursWorked { get; }
            public double? HoursLate { get; }
            public double? HoursUndertime { get; }

            public double? HoursROT { get; }

            public double? HoursND { get; }
            public double? HoursNDOT { get; }

            public double? HoursSH { get; }
            public double? HoursNDSH { get; }

            public double? HoursSHOT { get; }
            public double? HoursNDSHOT { get; }

            public double? HoursLH { get; }
            public double? HoursNDLH { get; }

            public double? HoursLHOT { get; }
            public double? HoursNDLHOT { get; }

            public double? HoursDOD { get; }
            public double? HoursDODOT { get; }

            public double? HoursSHDOD { get; }
            public double? HoursSHDODOT { get; }

            public double? HoursLHDOD { get; }
            public double? HoursLHDODOT { get; }

            public double? HoursADJ_PAY { get; }
            public double? HoursEXC_DOD { get; }
            public double? HoursHOLIDAY { get; }
            public double? HoursND_LHX8 { get; }
            public double? HoursND_LHRDX8 { get; }
            public double? HoursND_LHRDF8 { get; }
            public double? HoursND_LHF8 { get; }
            public double? HoursNDX8 { get; }
            public double? HoursNDF8 { get; }
            public double? HoursND_RDX8 { get; }
            public double? HoursND_RDF8 { get; }
            public double? HoursND_SHX8 { get; }
            public double? HoursND_SHRDX8 { get; }
            public double? HoursND_SHRDF8 { get; }
            public double? HoursND_SHF8 { get; }
            public double? HoursND_B2020 { get; }
            public double? HoursOT_LHX8 { get; }
            public double? HoursOT_LHRDX8 { get; }
            public double? HoursOT_LHRDF8 { get; }
            public double? HoursOT_LHF8 { get; }
            public double? HoursOT_RDX8 { get; }
            public double? HoursOT_RDF8 { get; }
            public double? HoursOT_SHF8M { get; }
            public double? HoursOT_SHX8 { get; }
            public double? HoursOT_SHRDX8 { get; }
            public double? HoursOT_SHRDF8 { get; }
            public double? HoursOT_SHF8 { get; }
            public double? HoursUWLH_B2020 { get; }
        }
    }
}
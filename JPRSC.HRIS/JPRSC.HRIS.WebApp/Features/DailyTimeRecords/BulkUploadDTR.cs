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
                    var employee = allEmployeesOfClient.SingleOrDefault(e => !e.DeletedOn.HasValue && !String.IsNullOrWhiteSpace(e.EmployeeCode) && String.Equals(e.EmployeeCode.Trim().TrimStart('0'), uploadItem.EmployeeCode.TrimStart('0'), StringComparison.CurrentCultureIgnoreCase));
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
                        existingDailyTimeRecord.HoursLate = uploadItem.MinutesLate / 60;
                        existingDailyTimeRecord.HoursLateValue = (decimal?)(uploadItem.MinutesLate / 60) * employee.HourlyRate;
                        existingDailyTimeRecord.HoursUndertime = uploadItem.MinutesUndertime / 60;
                        existingDailyTimeRecord.HoursUndertimeValue = (decimal?)(uploadItem.MinutesUndertime / 60) * employee.HourlyRate;
                        existingDailyTimeRecord.HoursWorked = uploadItem.MinutesWorked / 60;
                        existingDailyTimeRecord.HoursWorkedValue = (decimal?)(uploadItem.MinutesWorked / 60) * employee.HourlyRate;
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
                            HoursLate = uploadItem.MinutesLate / 60,
                            HoursLateValue = (decimal?)(uploadItem.MinutesLate / 60) * employee.HourlyRate,
                            HoursUndertime = uploadItem.MinutesUndertime / 60,
                            HoursUndertimeValue = (decimal?)(uploadItem.MinutesUndertime / 60) * employee.HourlyRate,
                            HoursWorked = uploadItem.MinutesWorked / 60,
                            HoursWorkedValue = (decimal?)(uploadItem.MinutesWorked / 60) * employee.HourlyRate,
                            PayrollPeriodFrom = command.PayrollPeriodFrom,
                            PayrollPeriodTo = command.PayrollPeriodTo
                        };
                        _db.DailyTimeRecords.Add(dailyTimeRecord);
                    }

                    if (uploadItem.MinutesROT.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ROT");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesROT / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesROT / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesND.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesND / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesND / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesNDOT.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "NDOT");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesNDOT / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesNDOT / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesSH.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "SH");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesSH / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesSH / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesNDSH.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "NDSH");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesNDSH / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesNDSH / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesSHOT.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "SHOT");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesSHOT / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesSHOT / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesNDSHOT.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "NDSHOT");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesNDSHOT / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesNDSHOT / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesLH.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "LH");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesLH / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesLH / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesNDLH.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "NDLH");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesNDLH / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesNDLH / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesLHOT.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "LHOT");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesLHOT / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesLHOT / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesNDLHOT.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "NDLHOT");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesNDLHOT;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesNDLHOT / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesDOD.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "DOD");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesDOD / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesDOD / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesDODOT.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "DODOT");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesDODOT / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesDODOT / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesSHDOD.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "SHDOD");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesSHDOD / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesSHDOD / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesSHDODOT.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "SHDODOT");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesSHDODOT / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesSHDODOT / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesLHDOD.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "LHDOD");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesLHDOD / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesLHDOD / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesLHDODOT.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "LHDODOT");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesLHDODOT / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesLHDODOT / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesADJ_PAY.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ADJ_PAY");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesADJ_PAY / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesADJ_PAY / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesEXC_DOD.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "EXC_DOD");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesEXC_DOD;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesEXC_DOD / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesHOLIDAY.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "HOLIDAY");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesHOLIDAY / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesHOLIDAY / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesND_LHX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_LHX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesND_LHX8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesND_LHX8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesND_LHRDX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_LHRDX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesND_LHRDX8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesND_LHRDX8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesND_LHRDF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_LHRDF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesND_LHRDF8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesND_LHRDF8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesND_LHF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_LHF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesND_LHF8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesND_LHF8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesNDX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "NDX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesNDX8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesNDX8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesNDF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "NDF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesNDF8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesNDF8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesND_RDX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_RDX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesND_RDX8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesND_RDX8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesND_RDF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_RDF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesND_RDF8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesND_RDF8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesND_SHX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_SHX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesND_SHX8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesND_SHX8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesND_SHRDX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_SHRDX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesND_SHRDX8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesND_SHRDX8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesND_SHRDF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_SHRDF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesND_SHRDF8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesND_SHRDF8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesND_SHF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_SHF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesND_SHF8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesND_SHF8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesND_B2020.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "ND_B2020");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesND_B2020 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesND_B2020 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesOT_LHX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_LHX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesOT_LHX8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesOT_LHX8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesOT_LHRDX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_LHRDX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesOT_LHRDX8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesOT_LHRDX8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesOT_LHRDF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_LHRDF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesOT_LHRDF8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesOT_LHRDF8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesOT_LHF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_LHF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesOT_LHF8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesOT_LHF8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesOT_RDX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_RDX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesOT_RDX8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesOT_RDX8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesOT_RDF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_RDF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesOT_RDF8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesOT_RDF8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesOT_SHF8M.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_SHF8M");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesOT_SHF8M / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesOT_SHF8M / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesOT_SHX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_SHX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesOT_SHX8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesOT_SHX8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesOT_SHRDX8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_SHRDX8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesOT_SHRDX8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesOT_SHRDX8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesOT_SHRDF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_SHRDF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesOT_SHRDF8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesOT_SHRDF8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesOT_SHF8.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "OT_SHF8");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesOT_SHF8 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesOT_SHF8 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                        if (overtime.Id == 0) _db.Overtimes.Add(overtime);
                    }

                    if (uploadItem.MinutesUWLH_B2020.HasValue)
                    {
                        var payRate = allPayRates.Single(p => p.Name == "UWLH_B2020");
                        var overtime = await GetOvertime(command, now, employee, payRate);
                        overtime.NumberOfHours = uploadItem.MinutesUWLH_B2020 / 60;
                        overtime.NumberOfHoursValue = (decimal?)(uploadItem.MinutesUWLH_B2020 / 60) * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
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
                MinutesWorked = items[4].ToNullableDouble();
                MinutesLate = items[5].ToNullableDouble();
                MinutesUndertime = items[6].ToNullableDouble();

                MinutesROT = items[7].ToNullableDouble();

                MinutesND = items[8].ToNullableDouble();
                MinutesNDOT = items[9].ToNullableDouble();

                MinutesSH = items[10].ToNullableDouble();
                MinutesNDSH = items[11].ToNullableDouble();

                MinutesSHOT = items[12].ToNullableDouble();
                MinutesNDSHOT = items[13].ToNullableDouble();

                MinutesLH = items[14].ToNullableDouble();
                MinutesNDLH = items[15].ToNullableDouble();

                MinutesLHOT = items[16].ToNullableDouble();
                MinutesNDLHOT = items[17].ToNullableDouble();

                MinutesDOD = items[18].ToNullableDouble();
                MinutesDODOT = items[19].ToNullableDouble();

                MinutesSHDOD = items[20].ToNullableDouble();
                MinutesSHDODOT = items[21].ToNullableDouble();

                MinutesLHDOD = items[22].ToNullableDouble();
                MinutesLHDODOT = items[23].ToNullableDouble();

                MinutesADJ_PAY = items[24].ToNullableDouble();
                MinutesEXC_DOD = items[25].ToNullableDouble();
                MinutesHOLIDAY = items[26].ToNullableDouble();
                MinutesND_LHX8 = items[27].ToNullableDouble();
                MinutesND_LHRDX8 = items[28].ToNullableDouble();
                MinutesND_LHRDF8 = items[29].ToNullableDouble();
                MinutesND_LHF8 = items[30].ToNullableDouble();
                MinutesNDX8 = items[31].ToNullableDouble();
                MinutesNDF8 = items[32].ToNullableDouble();
                MinutesND_RDX8 = items[33].ToNullableDouble();
                MinutesND_RDF8 = items[34].ToNullableDouble();
                MinutesND_SHX8 = items[35].ToNullableDouble();
                MinutesND_SHRDX8 = items[36].ToNullableDouble();
                MinutesND_SHRDF8 = items[37].ToNullableDouble();
                MinutesND_SHF8 = items[38].ToNullableDouble();
                MinutesND_B2020 = items[39].ToNullableDouble();
                MinutesOT_LHX8 = items[40].ToNullableDouble();
                MinutesOT_LHRDX8 = items[41].ToNullableDouble();
                MinutesOT_LHRDF8 = items[42].ToNullableDouble();
                MinutesOT_LHF8 = items[43].ToNullableDouble();
                MinutesOT_RDX8 = items[44].ToNullableDouble();
                MinutesOT_RDF8 = items[45].ToNullableDouble();
                MinutesOT_SHF8M = items[46].ToNullableDouble();
                MinutesOT_SHX8 = items[47].ToNullableDouble();
                MinutesOT_SHRDX8 = items[48].ToNullableDouble();
                MinutesOT_SHRDF8 = items[49].ToNullableDouble();
                MinutesOT_SHF8 = items[50].ToNullableDouble();
                MinutesUWLH_B2020 = items[51].ToNullableDouble();
            }

            public string EmployeeCode { get; set; }
            public string LastName { get; }
            public string FirstName { get; }
            public double? DaysWorked { get; }
            public double? MinutesWorked { get; }
            public double? MinutesLate { get; }
            public double? MinutesUndertime { get; }

            public double? MinutesROT { get; }

            public double? MinutesND { get; }
            public double? MinutesNDOT { get; }

            public double? MinutesSH { get; }
            public double? MinutesNDSH { get; }

            public double? MinutesSHOT { get; }
            public double? MinutesNDSHOT { get; }

            public double? MinutesLH { get; }
            public double? MinutesNDLH { get; }

            public double? MinutesLHOT { get; }
            public double? MinutesNDLHOT { get; }

            public double? MinutesDOD { get; }
            public double? MinutesDODOT { get; }

            public double? MinutesSHDOD { get; }
            public double? MinutesSHDODOT { get; }

            public double? MinutesLHDOD { get; }
            public double? MinutesLHDODOT { get; }

            public double? MinutesADJ_PAY { get; }
            public double? MinutesEXC_DOD { get; }
            public double? MinutesHOLIDAY { get; }
            public double? MinutesND_LHX8 { get; }
            public double? MinutesND_LHRDX8 { get; }
            public double? MinutesND_LHRDF8 { get; }
            public double? MinutesND_LHF8 { get; }
            public double? MinutesNDX8 { get; }
            public double? MinutesNDF8 { get; }
            public double? MinutesND_RDX8 { get; }
            public double? MinutesND_RDF8 { get; }
            public double? MinutesND_SHX8 { get; }
            public double? MinutesND_SHRDX8 { get; }
            public double? MinutesND_SHRDF8 { get; }
            public double? MinutesND_SHF8 { get; }
            public double? MinutesND_B2020 { get; }
            public double? MinutesOT_LHX8 { get; }
            public double? MinutesOT_LHRDX8 { get; }
            public double? MinutesOT_LHRDF8 { get; }
            public double? MinutesOT_LHF8 { get; }
            public double? MinutesOT_RDX8 { get; }
            public double? MinutesOT_RDF8 { get; }
            public double? MinutesOT_SHF8M { get; }
            public double? MinutesOT_SHX8 { get; }
            public double? MinutesOT_SHRDX8 { get; }
            public double? MinutesOT_SHRDF8 { get; }
            public double? MinutesOT_SHF8 { get; }
            public double? MinutesUWLH_B2020 { get; }
        }
    }
}
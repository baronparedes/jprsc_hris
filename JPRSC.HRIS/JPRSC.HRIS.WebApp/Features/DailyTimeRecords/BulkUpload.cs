using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
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
    public class BulkUpload
    {
        public class Command : IRequest<CommandResult>
        {
            public int? ClientId { get; set; }
            public HttpPostedFileBase File { get; set; }
            public DateTime? PayrollPeriodFrom { get; set; }
            public DateTime? PayrollPeriodTo { get; set; }
            public IList<IList<string>> Lines { get; set; } = new List<IList<string>>();
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator(ApplicationDbContext db)
            {
                RuleFor(c => c.ClientId)
                    .NotEmpty();

                RuleFor(c => c.PayrollPeriodFrom)
                    .NotNull();

                RuleFor(c => c.PayrollPeriodTo)
                    .NotNull();

                RuleFor(c => c.File)
                    .NotNull();

                When(c => c.File != null, () =>
                {
                    RuleFor(c => c.File)
                        .Must(Have23Columns)
                        .WithMessage("Unrecognized number of columns. Please use these 23 columns: Last Name, First Name, Days Worked, Hours Worked, Hours Late, Hours Undertime, ROT, ND, NDOT, SH, NDSH, SHOT, NDSHOT, LH, NDLH, LHOT, NDLHOT, DOD, DODOT, SHDOD, SHDODOT, LHDOD, LHDODOT");
                });
            }

            private bool Have23Columns(Command command, HttpPostedFileBase file)
            {
                // Set the lines here so we don't have to deal with the stream later on
                var has23Columns = false;

                using (var csvreader = new StreamReader(file.InputStream))
                {
                    while (!csvreader.EndOfStream)
                    {
                        var line = csvreader.ReadLine();
                        var lineAsColumns = line.Split(',');
                        command.Lines.Add(lineAsColumns);

                        if (lineAsColumns.Count() == 23)
                        {
                            has23Columns = true;
                        }
                    }
                }

                return has23Columns;
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

            public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
            {
                var unprocessedItems = new List<CommandResult.UnprocessedItem>();
                var uploadItems = GetUploadItemsFromUploadedFile(request);
                var allPayRates = await _db.PayPercentages.ToListAsync();
                var allEmployeesOfClient = await _db.Employees.Where(e => !e.DeletedOn.HasValue && e.ClientId == request.ClientId).ToListAsync();
                var now = DateTime.UtcNow;

                // Upload behavior: all-or-nothing
                foreach (var uploadItem in uploadItems)
                {
                    var employee = allEmployeesOfClient.SingleOrDefault(e => String.Equals(e.FirstName.Trim(), uploadItem.FirstName, StringComparison.CurrentCultureIgnoreCase) && String.Equals(e.LastName.Trim(), uploadItem.LastName, StringComparison.CurrentCultureIgnoreCase));
                    if (employee == null)
                    {
                        unprocessedItems.Add(new CommandResult.UnprocessedItem
                        {
                            FirstName = uploadItem.FirstName,
                            LastName = uploadItem.LastName,
                            Reason = "Employee not found."
                        });

                        continue;
                    }

                    var existingDailyTimeRecord = _db.DailyTimeRecords.SingleOrDefault(dtr => !dtr.DeletedOn.HasValue && dtr.EmployeeId == employee.Id && dtr.PayrollPeriodFrom == request.PayrollPeriodFrom && dtr.PayrollPeriodTo == request.PayrollPeriodTo);
                    if (existingDailyTimeRecord != null)
                    {
                        unprocessedItems.Add(new CommandResult.UnprocessedItem
                        {
                            FirstName = uploadItem.FirstName,
                            LastName = uploadItem.LastName,
                            Reason = $"DTR or OT for this payroll period already processed."
                        });

                        continue;
                    }

                    if (!unprocessedItems.Any())
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
                            PayrollPeriodFrom = request.PayrollPeriodFrom,
                            PayrollPeriodTo = request.PayrollPeriodTo
                        };
                        _db.DailyTimeRecords.Add(dailyTimeRecord);

                        if (uploadItem.HoursROT.HasValue)
                        {
                            var payRate = allPayRates.Single(p => p.Name == "ROT");
                            var overtime = NewOvertime(request, now, employee, payRate);
                            overtime.NumberOfHours = uploadItem.HoursROT;
                            overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursROT * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                            _db.Overtimes.Add(overtime);
                        }

                        if (uploadItem.HoursND.HasValue)
                        {
                            var payRate = allPayRates.Single(p => p.Name == "ND");
                            var overtime = NewOvertime(request, now, employee, payRate);
                            overtime.NumberOfHours = uploadItem.HoursND;
                            overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursND * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                            _db.Overtimes.Add(overtime);
                        }

                        if (uploadItem.HoursNDOT.HasValue)
                        {
                            var payRate = allPayRates.Single(p => p.Name == "NDOT");
                            var overtime = NewOvertime(request, now, employee, payRate);
                            overtime.NumberOfHours = uploadItem.HoursNDOT;
                            overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursNDOT * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                            _db.Overtimes.Add(overtime);
                        }

                        if (uploadItem.HoursSH.HasValue)
                        {
                            var payRate = allPayRates.Single(p => p.Name == "SH");
                            var overtime = NewOvertime(request, now, employee, payRate);
                            overtime.NumberOfHours = uploadItem.HoursSH;
                            overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursSH * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                            _db.Overtimes.Add(overtime);
                        }

                        if (uploadItem.HoursNDSH.HasValue)
                        {
                            var payRate = allPayRates.Single(p => p.Name == "NDSH");
                            var overtime = NewOvertime(request, now, employee, payRate);
                            overtime.NumberOfHours = uploadItem.HoursNDSH;
                            overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursNDSH * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                            _db.Overtimes.Add(overtime);
                        }

                        if (uploadItem.HoursSHOT.HasValue)
                        {
                            var payRate = allPayRates.Single(p => p.Name == "SHOT");
                            var overtime = NewOvertime(request, now, employee, payRate);
                            overtime.NumberOfHours = uploadItem.HoursSHOT;
                            overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursSHOT * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                            _db.Overtimes.Add(overtime);
                        }

                        if (uploadItem.HoursNDSHOT.HasValue)
                        {
                            var payRate = allPayRates.Single(p => p.Name == "NDSHOT");
                            var overtime = NewOvertime(request, now, employee, payRate);
                            overtime.NumberOfHours = uploadItem.HoursNDSHOT;
                            overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursNDSHOT * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                            _db.Overtimes.Add(overtime);
                        }

                        if (uploadItem.HoursLH.HasValue)
                        {
                            var payRate = allPayRates.Single(p => p.Name == "LH");
                            var overtime = NewOvertime(request, now, employee, payRate);
                            overtime.NumberOfHours = uploadItem.HoursLH;
                            overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursLH * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                            _db.Overtimes.Add(overtime);
                        }

                        if (uploadItem.HoursNDLH.HasValue)
                        {
                            var payRate = allPayRates.Single(p => p.Name == "NDLH");
                            var overtime = NewOvertime(request, now, employee, payRate);
                            overtime.NumberOfHours = uploadItem.HoursNDLH;
                            overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursNDLH * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                            _db.Overtimes.Add(overtime);
                        }

                        if (uploadItem.HoursLHOT.HasValue)
                        {
                            var payRate = allPayRates.Single(p => p.Name == "LHOT");
                            var overtime = NewOvertime(request, now, employee, payRate);
                            overtime.NumberOfHours = uploadItem.HoursLHOT;
                            overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursLHOT * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                            _db.Overtimes.Add(overtime);
                        }

                        if (uploadItem.HoursNDLHOT.HasValue)
                        {
                            var payRate = allPayRates.Single(p => p.Name == "NDLHOT");
                            var overtime = NewOvertime(request, now, employee, payRate);
                            overtime.NumberOfHours = uploadItem.HoursNDLHOT;
                            overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursNDLHOT * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                            _db.Overtimes.Add(overtime);
                        }

                        if (uploadItem.HoursDOD.HasValue)
                        {
                            var payRate = allPayRates.Single(p => p.Name == "DOD");
                            var overtime = NewOvertime(request, now, employee, payRate);
                            overtime.NumberOfHours = uploadItem.HoursDOD;
                            overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursDOD * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                            _db.Overtimes.Add(overtime);
                        }

                        if (uploadItem.HoursDODOT.HasValue)
                        {
                            var payRate = allPayRates.Single(p => p.Name == "DODOT");
                            var overtime = NewOvertime(request, now, employee, payRate);
                            overtime.NumberOfHours = uploadItem.HoursDODOT;
                            overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursDODOT * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                            _db.Overtimes.Add(overtime);
                        }

                        if (uploadItem.HoursSHDOD.HasValue)
                        {
                            var payRate = allPayRates.Single(p => p.Name == "SHDOD");
                            var overtime = NewOvertime(request, now, employee, payRate);
                            overtime.NumberOfHours = uploadItem.HoursSHDOD;
                            overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursSHDOD * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                            _db.Overtimes.Add(overtime);
                        }

                        if (uploadItem.HoursSHDODOT.HasValue)
                        {
                            var payRate = allPayRates.Single(p => p.Name == "SHDODOT");
                            var overtime = NewOvertime(request, now, employee, payRate);
                            overtime.NumberOfHours = uploadItem.HoursSHDODOT;
                            overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursSHDODOT * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                            _db.Overtimes.Add(overtime);
                        }

                        if (uploadItem.HoursLHDOD.HasValue)
                        {
                            var payRate = allPayRates.Single(p => p.Name == "LHDOD");
                            var overtime = NewOvertime(request, now, employee, payRate);
                            overtime.NumberOfHours = uploadItem.HoursLHDOD;
                            overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursLHDOD * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
                            _db.Overtimes.Add(overtime);
                        }

                        if (uploadItem.HoursLHDODOT.HasValue)
                        {
                            var payRate = allPayRates.Single(p => p.Name == "LHDODOT");
                            var overtime = NewOvertime(request, now, employee, payRate);
                            overtime.NumberOfHours = uploadItem.HoursLHDODOT;
                            overtime.NumberOfHoursValue = (decimal?)uploadItem.HoursLHDODOT * employee.HourlyRate * (decimal?)(payRate.Percentage / 100);
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
                    ProcessedItemsCount = unprocessedItems.Any() ? 0 : uploadItems.Count()
                };
            }

            private Overtime NewOvertime(Command request, DateTime addedOn, Employee employee, PayPercentage payRate)
            {
                return new Overtime
                {
                    AddedOn = addedOn,
                    EmployeeId = employee.Id,
                    PayPercentageName = payRate.Name,
                    PayPercentagePercentage = payRate.Percentage,
                    PayrollPeriodFrom = request.PayrollPeriodFrom,
                    PayrollPeriodTo = request.PayrollPeriodTo
                };
            }

            private IEnumerable<UploadItem> GetUploadItemsFromUploadedFile(Command request)
            {
                var uploadItems = new List<UploadItem>();

                for (var i = 0; i < request.Lines.Count; i++)
                {
                    if (i == 0) continue;

                    uploadItems.Add(new UploadItem(request.Lines[i]));
                }

                return uploadItems;
            }
        }

        public class UploadItem
        {
            public UploadItem(IList<string> items)
            {
                LastName = String.IsNullOrWhiteSpace(items[0]) ? null : items[0].Trim();
                FirstName = String.IsNullOrWhiteSpace(items[1]) ? null : items[1].Trim();

                DaysWorked = items[2].ToNullableDouble();
                HoursWorked = items[3].ToNullableDouble();
                HoursLate = items[4].ToNullableDouble();
                HoursUndertime = items[5].ToNullableDouble();

                HoursROT = items[6].ToNullableDouble();

                HoursND = items[7].ToNullableDouble();
                HoursNDOT = items[8].ToNullableDouble();

                HoursSH = items[9].ToNullableDouble();
                HoursNDSH = items[10].ToNullableDouble();

                HoursSHOT = items[11].ToNullableDouble();
                HoursNDSHOT = items[12].ToNullableDouble();

                HoursLH = items[13].ToNullableDouble();
                HoursNDLH = items[14].ToNullableDouble();

                HoursLHOT = items[15].ToNullableDouble();
                HoursNDLHOT = items[16].ToNullableDouble();

                HoursDOD = items[17].ToNullableDouble();
                HoursDODOT = items[18].ToNullableDouble();

                HoursSHDOD = items[19].ToNullableDouble();
                HoursSHDODOT = items[20].ToNullableDouble();

                HoursLHDOD = items[21].ToNullableDouble();
                HoursLHDODOT = items[22].ToNullableDouble();
            }

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
        }
    }
}
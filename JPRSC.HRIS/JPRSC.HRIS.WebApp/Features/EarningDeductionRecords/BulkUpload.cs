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

namespace JPRSC.HRIS.WebApp.Features.EarningDeductionRecords
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
                        .Must(HaveValidNumberOfColumns)
                        .WithMessage("Unrecognized number of columns. Please use these columns: Employee Code, Last Name, First Name, ");
                });
            }

            private bool HaveValidNumberOfColumns(Command command, HttpPostedFileBase file)
            {
                // Set the lines here so we don't have to deal with the stream later on
                var hasValidNumberOfColumns = false;
                var numberOfColumns = 6;

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
                var allEarningDeductions = await _db.EarningDeductions.ToListAsync();
                var allEmployeesOfClient = await _db.Employees.Where(e => !e.DeletedOn.HasValue && e.ClientId == request.ClientId).ToListAsync();
                var now = DateTime.UtcNow;

                // Upload behavior: all-or-nothing
                foreach (var uploadItem in uploadItems)
                {
                    var employee = allEmployeesOfClient.SingleOrDefault(e => !e.DeletedOn.HasValue && String.Equals(e.EmployeeCode.Trim(), uploadItem.EmployeeCode, StringComparison.CurrentCultureIgnoreCase));
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

                    var existingEarningDeductionRecord = _db.EarningDeductionRecords.SingleOrDefault(dtr => !dtr.DeletedOn.HasValue && dtr.EmployeeId == employee.Id && dtr.PayrollPeriodFrom == request.PayrollPeriodFrom && dtr.PayrollPeriodTo == request.PayrollPeriodTo);
                    if (existingEarningDeductionRecord != null)
                    {
                        unprocessedItems.Add(new CommandResult.UnprocessedItem
                        {
                            FirstName = uploadItem.FirstName,
                            LastName = uploadItem.LastName,
                            Reason = $"Earning / deduction record already exists."
                        });

                        continue;
                    }

                    if (!unprocessedItems.Any())
                    {
                        var earningDeductionRecord = new EarningDeductionRecord
                        {
                            AddedOn = now,
                            //Amount
                            //EarningDeductionId
                            EmployeeId = employee.Id,
                            PayrollPeriodFrom = request.PayrollPeriodFrom,
                            PayrollPeriodTo = request.PayrollPeriodTo
                        };

                        _db.EarningDeductionRecords.Add(earningDeductionRecord);                        
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
                EmployeeCode = String.IsNullOrWhiteSpace(items[0]) ? null : items[0].Trim();
                LastName = String.IsNullOrWhiteSpace(items[1]) ? null : items[1].Trim();
                FirstName = String.IsNullOrWhiteSpace(items[2]) ? null : items[2].Trim();

                AdjustmentDeduction = items[3].ToNullableDecimal();
                CashAdv = items[4].ToNullableDecimal();
            }

            public string EmployeeCode { get; set; }
            public string LastName { get; }
            public string FirstName { get; }

            public decimal? AdjustmentDeduction { get; set; }
            public decimal? CashAdv { get; set; }
        }
    }
}
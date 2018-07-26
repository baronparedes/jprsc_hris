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
    public class BulkUploadEDR
    {
        public class Command : IRequest<CommandResult>
        {
            public int? ClientId { get; set; }
            public HttpPostedFileBase File { get; set; }
            public DateTime? PayrollPeriodFrom { get; set; }
            public DateTime? PayrollPeriodTo { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly ApplicationDbContext _db = DependencyConfig.Instance.Container.GetInstance<ApplicationDbContext>();

            public CommandValidator()
            {
                RuleFor(c => c.ClientId)
                    .NotEmpty();

                RuleFor(c => c.PayrollPeriodFrom)
                    .NotNull();

                RuleFor(c => c.PayrollPeriodTo)
                    .NotNull();

                RuleFor(c => c.File)
                    .NotNull();

                When(c => c.ClientId.HasValue && c.PayrollPeriodFrom.HasValue && c.PayrollPeriodTo.HasValue, () =>
                {
                    RuleFor(c => c.ClientId)
                        .Must(NotHaveEndProcessedYet)
                        .WithMessage("End process for this payroll period has finished.");
                });
            }

            private bool NotHaveEndProcessedYet(Command command, int? clientId)
            {
                var endProcessRecord = _db.PayrollProcessBatches.SingleOrDefault(ppb => !ppb.DeletedOn.HasValue && !ppb.DateOverwritten.HasValue && ppb.EndProcessedOn.HasValue && ppb.ClientId == clientId && ppb.PayrollPeriodFrom == command.PayrollPeriodFrom && ppb.PayrollPeriodTo == command.PayrollPeriodTo);
                return endProcessRecord == null;
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
                var now = DateTime.UtcNow;
                var unprocessedItems = new List<CommandResult.UnprocessedItem>();
                var allEmployeesOfClient = await _db.Employees.Where(e => !e.DeletedOn.HasValue && e.ClientId == request.ClientId).ToListAsync();
                var csvData = GetCSVData(request);
                var columnToEarningDeductionMap = await GetColumnToEarningDeductionMap(csvData.Item1);

                // Upload behavior: all-or-nothing
                foreach (var line in csvData.Item2)
                {
                    var employeeCode = String.IsNullOrWhiteSpace(line[0]) ? null : line[0].Trim();
                    var lastName = String.IsNullOrWhiteSpace(line[1]) ? null : line[1].Trim();
                    var firstName = String.IsNullOrWhiteSpace(line[2]) ? null : line[2].Trim();

                    var employee = allEmployeesOfClient.SingleOrDefault(e => !e.DeletedOn.HasValue && String.Equals(e.EmployeeCode.Trim().TrimStart('0'), employeeCode.TrimStart('0'), StringComparison.CurrentCultureIgnoreCase));
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

                    foreach (KeyValuePair<int, EarningDeduction> entry in columnToEarningDeductionMap.Where(kvp => kvp.Value != null))
                    {
                        var amount = line[entry.Key].ToNullableDecimal();

                        var existingEarningDeductionRecord = await _db.EarningDeductionRecords.SingleOrDefaultAsync(edr => !edr.DeletedOn.HasValue && edr.EmployeeId == employee.Id && edr.PayrollPeriodFrom == request.PayrollPeriodFrom && edr.PayrollPeriodTo == request.PayrollPeriodTo && edr.EarningDeductionId == entry.Value.Id);
                        if (existingEarningDeductionRecord != null)
                        {
                            existingEarningDeductionRecord.Amount = amount;
                            existingEarningDeductionRecord.ModifiedOn = now;
                        }
                        else
                        {
                            var earningDeductionRecord = new EarningDeductionRecord
                            {
                                AddedOn = now,
                                Amount = amount,
                                EarningDeductionId = entry.Value.Id,
                                EmployeeId = employee.Id,
                                PayrollPeriodFrom = request.PayrollPeriodFrom,
                                PayrollPeriodTo = request.PayrollPeriodTo
                            };
                            _db.EarningDeductionRecords.Add(earningDeductionRecord);
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
                    ProcessedItemsCount = unprocessedItems.Any() ? 0 : csvData.Item2.Count()
                };
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
                            body.Add(lineAsColumns);
                        }
                    }
                }

                return Tuple.Create(header, body);
            }

            private async Task<Dictionary<int, EarningDeduction>> GetColumnToEarningDeductionMap(IList<string> header)
            {
                var allEarningDeductions = await _db.EarningDeductions.Where(ed => !ed.DeletedOn.HasValue).ToListAsync();
                var edStartingColumnIndexInCSV = 3;
                var columnToEarningDeductionIdMap = new Dictionary<int, EarningDeduction>();

                for (var i = edStartingColumnIndexInCSV; i < header.Count; i++)
                {
                    var headerValue = String.IsNullOrWhiteSpace(header[i]) ? null : header[i].Trim();
                    var matchingEarningDeduction = allEarningDeductions.SingleOrDefault(ed => String.Equals(ed.Code, headerValue, StringComparison.InvariantCultureIgnoreCase));
                    columnToEarningDeductionIdMap.Add(i, matchingEarningDeduction);
                }

                return columnToEarningDeductionIdMap;
            }
        }
    }
}
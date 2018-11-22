using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Mvc;
using MediatR;
using Newtonsoft.Json;
using SelectPdf;

namespace JPRSC.HRIS.WebApp.Features.Payroll
{
    public class EndProcess
    {
        public class Query : IRequest<QueryResult>
        {
        }

        public class QueryResult
        {
            public IEnumerable<Client> Clients { get; set; } = new List<Client>();

            public class Client
            {
                public string Code { get; set; }
                public int? CurrentPayrollPeriod { get; set; }
                public CutOffPeriod? CutOffPeriod { get; set; }
                public int? DaysPerWeek { get; set; }
                public string Description { get; set; }
                public int? HoursPerDay { get; set; }
                public int Id { get; set; }
                public string Name { get; set; }
                public int? NumberOfHoursInADay { get; set; }
                public int? NumberOfPayrollPeriodsAMonth { get; set; }
                public int? NumberOfWorkingDaysForThisPayrollPeriod { get; set; }
                public PayrollCode? PayrollCode { get; set; }
                public DateTime? PayrollPeriodFrom { get; set; }
                public Month? PayrollPeriodMonth { get; set; }
                public string PayrollPeriodMonthString => PayrollPeriodMonth?.ToString();
                public DateTime? PayrollPeriodTo { get; set; }
                public TaxTable? TaxTable { get; set; }
                public bool? ZeroBasic { get; set; }

                [JsonIgnore]
                public IEnumerable<PayrollProcessBatch> PayrollProcessBatches { get; set; } = new List<PayrollProcessBatch>();

                public PayrollProcessBatch PayrollProcessBatchForEndProcess { get; set; }

                public int? NextPayrollPeriod => CurrentPayrollPeriod == NumberOfPayrollPeriodsAMonth ? 1 : CurrentPayrollPeriod += 1;
                public DateTime? NextPayrollPeriodFrom => PayrollPeriodFrom.Value.AddDays(NumberOfWorkingDaysForThisPayrollPeriod.Value);
                public DateTime? NextPayrollPeriodTo => PayrollPeriodTo.Value.AddDays(NumberOfWorkingDaysForThisPayrollPeriod.Value);
                public Month? NextPayrollPeriodMonth => !PayrollPeriodMonth.HasValue ? (Month?)null : (int)PayrollPeriodMonth == 120 ? (Month)10 : (Month)((int)PayrollPeriodMonth.Value + 10);
            }

            public class PayrollProcessBatch
            {
                public DateTime AddedOn { get; set; }
                public DateTime? DeletedOn { get; set; }
                public int Id { get; set; }
                public DateTime? ModifiedOn { get; set; }
                public int? ClientId { get; set; }
                public bool? DeductedSSS { get; set; }
                public bool? DeductedPHIC { get; set; }
                public bool? DeductedPagIbig { get; set; }
                public bool? DeductedTax { get; set; }
                public int? PayrollPeriod { get; set; }
                public DateTime? PayrollPeriodFrom { get; set; }
                public DateTime? PayrollPeriodTo { get; set; }
                public DateTime? DateOverwritten { get; set; }
                public Month? PayrollPeriodMonth { get; set; }
                public DateTime? EndProcessedOn { get; set; }
            }
        }

        public class QueryHandler : IRequestHandler<Query, QueryResult>
        {
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<QueryResult> Handle(Query query, CancellationToken token)
            {
                var clients = await _db.Clients
                    .Include(c => c.PayrollProcessBatches)
                    .Where(c => !c.DeletedOn.HasValue)
                    .ProjectToListAsync<QueryResult.Client>();

                foreach (var client in clients)
                {
                    client.PayrollProcessBatchForEndProcess = client.PayrollProcessBatches.FirstOrDefault(ppb => !ppb.DeletedOn.HasValue && !ppb.DateOverwritten.HasValue && !ppb.EndProcessedOn.HasValue);
                }

                return new QueryResult
                {
                    Clients = clients.Where(c => c.PayrollProcessBatchForEndProcess != null)
                };
            }
        }

        public class EndProcessQuery : IRequest<EndProcessQueryResult>
        {
        }

        public class QueryValidator : AbstractValidator<Query>
        {
        }

        public class EndProcessQueryResult
        {
            public IEnumerable<PayrollProcessBatch> PayrollProcessBatches { get; set; } = new List<PayrollProcessBatch>();

            public class PayrollProcessBatch
            {
                public DateTime AddedOn { get; set; }
                public DateTime? DeletedOn { get; set; }
                public int Id { get; set; }
                public DateTime? ModifiedOn { get; set; }
                public Client Client { get; set; }
                public int? ClientId { get; set; }
                public bool? DeductedSSS { get; set; }
                public bool? DeductedPHIC { get; set; }
                public bool? DeductedPagIbig { get; set; }
                public bool? DeductedTax { get; set; }
                public int? PayrollPeriod { get; set; }
                public DateTime? PayrollPeriodFrom { get; set; }
                public DateTime? PayrollPeriodTo { get; set; }
                public DateTime? DateOverwritten { get; set; }
                public Month? PayrollPeriodMonth { get; set; }
                public DateTime? EndProcessedOn { get; set; }
            }

            public class Client
            {
                public string Code { get; set; }
                public int? CurrentPayrollPeriod { get; set; }
                public CutOffPeriod? CutOffPeriod { get; set; }
                public int? DaysPerWeek { get; set; }
                public string Description { get; set; }
                public int? HoursPerDay { get; set; }
                public int Id { get; set; }
                public string Name { get; set; }
                public int? NumberOfHoursInADay { get; set; }
                public int? NumberOfPayrollPeriodsAMonth { get; set; }
                public int? NumberOfWorkingDaysForThisPayrollPeriod { get; set; }
                public PayrollCode? PayrollCode { get; set; }
                public DateTime? PayrollPeriodFrom { get; set; }
                public Month? PayrollPeriodMonth { get; set; }
                public DateTime? PayrollPeriodTo { get; set; }
                public TaxTable? TaxTable { get; set; }
                public bool? ZeroBasic { get; set; }
            }
        }

        public class EmdProcessQueryHandler : IRequestHandler<EndProcessQuery, EndProcessQueryResult>
        {
            private readonly ApplicationDbContext _db;

            public EmdProcessQueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<EndProcessQueryResult> Handle(EndProcessQuery query, CancellationToken cancellationToken)
            {
                var payrollProcessBatches = await _db
                    .PayrollProcessBatches
                    .Include(ppb => ppb.Client)
                    .Where(ppb => !ppb.DeletedOn.HasValue && !ppb.DateOverwritten.HasValue && ppb.EndProcessedOn.HasValue)
                    .ProjectToListAsync<EndProcessQueryResult.PayrollProcessBatch>();

                return new EndProcessQueryResult
                {
                    PayrollProcessBatches = payrollProcessBatches
                };
            }
        }

        public class Command : IRequest<CommandResult>
        {
            public AppController AppController { get; set; }
            public int? PayrollProcessBatchId { get; set; }
            public int? PayrollPeriod { get; set; }
            public DateTime? PayrollPeriodFrom { get; set; }
            public DateTime? PayrollPeriodTo { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.PayrollProcessBatchId)
                    .NotEmpty();

                RuleFor(c => c.PayrollPeriod)
                    .NotEmpty();

                RuleFor(c => c.PayrollPeriodFrom)
                    .NotEmpty();

                RuleFor(c => c.PayrollPeriodTo)
                    .NotEmpty();
            }
        }

        public class CommandResult
        {
        }

        public class CommandHandler : IRequestHandler<Command, CommandResult>
        {
            private readonly ApplicationDbContext _db;
            private readonly IMediator _mediator;

            public CommandHandler(ApplicationDbContext db, IMediator mediator)
            {
                _db = db;
                _mediator = mediator;
            }

            public async Task<CommandResult> Handle(Command command, CancellationToken cancellationToken)
            {
                var now = DateTime.UtcNow;

                var payrollProcessBatch = await _db
                    .PayrollProcessBatches
                    .SingleOrDefaultAsync(ppb => ppb.Id == command.PayrollProcessBatchId && !ppb.DeletedOn.HasValue && !ppb.DateOverwritten.HasValue && !ppb.EndProcessedOn.HasValue);

                payrollProcessBatch.EndProcessedOn = now;
                payrollProcessBatch.ModifiedOn = now;

                var client = await _db
                    .Clients
                    .SingleOrDefaultAsync(c => c.Id == payrollProcessBatch.ClientId && !c.DeletedOn.HasValue);

                client.CurrentPayrollPeriod = command.PayrollPeriod;
                client.PayrollPeriodFrom = command.PayrollPeriodFrom;
                client.PayrollPeriodTo = command.PayrollPeriodTo;
                client.ModifiedOn = now;

                await _db.SaveChangesAsync();

                //await CreatePayslipFiles(command);

                return new CommandResult();
            }

            private async Task CreatePayslipFiles(Command command)
            {
                var payslipReportQuery = new PayslipReport.Query
                {
                    PayrollProcessBatchId = command.PayrollProcessBatchId
                };

                var payslipReportQueryResult = await _mediator.Send(payslipReportQuery);

                var tupledCollection = Tuple.Create(payslipReportQueryResult, payslipReportQueryResult.PayslipRecords);

                foreach (var payslipRecord in tupledCollection.Item2)
                {
                    var tupledIndividual = Tuple.Create(payslipReportQueryResult, payslipRecord);

                    var partialRendered = command.AppController.RenderPartialToString("PayslipReportTableIndividual", tupledIndividual);

                    var converter = new HtmlToPdf();
                    converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                    var doc = converter.ConvertHtmlString(partialRendered);

                    var saveDirectoryBase = HttpContext.Current.Server.MapPath("~/wwwroot/payslips/");
                    var saveDirectoryForBatch = Path.Combine(saveDirectoryBase, $"{command.PayrollProcessBatchId}/");

                    if (!Directory.Exists(saveDirectoryForBatch))
                    {
                        Directory.CreateDirectory(saveDirectoryForBatch);
                    }

                    var saveFileName = Path.Combine(saveDirectoryForBatch, $"{payslipRecord.PayrollRecord.EmployeeId}.pdf");

                    doc.Save(saveFileName);
                    doc.Close();
                }
            }
        }
    }
}
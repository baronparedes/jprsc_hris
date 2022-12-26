using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
                public DateTime? NextPayrollPeriodFrom => PayrollPeriodFrom.HasValue && NumberOfWorkingDaysForThisPayrollPeriod.HasValue ? PayrollPeriodFrom.Value.AddDays(NumberOfWorkingDaysForThisPayrollPeriod.Value) : (DateTime?)null;
                public DateTime? NextPayrollPeriodTo => PayrollPeriodTo.HasValue && NumberOfWorkingDaysForThisPayrollPeriod.HasValue ? PayrollPeriodTo.Value.AddDays(NumberOfWorkingDaysForThisPayrollPeriod.Value) : (DateTime?)null;
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

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<PayrollProcessBatch, QueryResult.PayrollProcessBatch>();
                CreateMap<Client, QueryResult.Client>();
                CreateMap<PayrollProcessBatch, EndProcessQueryResult.PayrollProcessBatch>();
                CreateMap<Client, EndProcessQueryResult.Client>();
            }
        }

        public class QueryHandler : IRequestHandler<Query, QueryResult>
        {
            private readonly ApplicationDbContext _db;
            private readonly IMapper _mapper;

            public QueryHandler(ApplicationDbContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<QueryResult> Handle(Query query, CancellationToken token)
            {
                var clients = await _db.Clients
                    .AsNoTracking()
                    .Include(c => c.PayrollProcessBatches)
                    .Where(c => !c.DeletedOn.HasValue)
                    .OrderBy(c => c.Code)
                    .ProjectTo<QueryResult.Client>(_mapper)
                    .ToListAsync();

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
            private readonly IMapper _mapper;

            public EmdProcessQueryHandler(ApplicationDbContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<EndProcessQueryResult> Handle(EndProcessQuery query, CancellationToken cancellationToken)
            {
                var payrollProcessBatches = await _db
                    .PayrollProcessBatches
                    .AsNoTracking()
                    .Include(ppb => ppb.Client)
                    .Where(ppb => !ppb.DeletedOn.HasValue && !ppb.DateOverwritten.HasValue && ppb.EndProcessedOn.HasValue)
                    .ProjectTo<EndProcessQueryResult.PayrollProcessBatch>(_mapper)
                    .ToListAsync();

                return new EndProcessQueryResult
                {
                    PayrollProcessBatches = payrollProcessBatches
                };
            }
        }

        public class Command : IRequest<CommandResult>
        {
            public AppController AppController { get; set; }
            public int? ClientId { get; set; }
            public int? NextPayrollPeriod { get; set; }
            public DateTime? NextPayrollPeriodFrom { get; set; }
            public DateTime? NextPayrollPeriodTo { get; set; }
            public int? PayrollPeriod { get; set; }
            public DateTime? PayrollPeriodFrom { get; set; }
            public DateTime? PayrollPeriodTo { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.ClientId)
                    .NotEmpty();

                RuleFor(c => c.NextPayrollPeriod)
                    .NotEmpty();

                RuleFor(c => c.NextPayrollPeriodFrom)
                    .NotEmpty();

                RuleFor(c => c.NextPayrollPeriodTo)
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

                var payrollPeriodFromDate = command.PayrollPeriodFrom.Value.Date;
                var payrollPeriodToDate = command.PayrollPeriodTo.Value.Date;

                var payrollProcessBatch = await _db
                    .PayrollProcessBatches
                    .Include(ppb => ppb.Client)
                    .SingleOrDefaultAsync(ppb => ppb.ClientId == command.ClientId && ppb.PayrollPeriod == command.PayrollPeriod && DbFunctions.TruncateTime(ppb.PayrollPeriodFrom) == payrollPeriodFromDate && DbFunctions.TruncateTime(ppb.PayrollPeriodTo) == payrollPeriodToDate && ppb.PayrollPeriod == command.PayrollPeriod && !ppb.DeletedOn.HasValue && !ppb.DateOverwritten.HasValue && !ppb.EndProcessedOn.HasValue);

                if (payrollProcessBatch == null) throw new Exception("Unable to find payroll process batch.");

                payrollProcessBatch.EndProcessedOn = now;
                payrollProcessBatch.ModifiedOn = now;

                var client = await _db
                    .Clients
                    .SingleOrDefaultAsync(c => c.Id == payrollProcessBatch.ClientId && !c.DeletedOn.HasValue);

                client.CurrentPayrollPeriod = command.NextPayrollPeriod;
                client.PayrollPeriodFrom = command.NextPayrollPeriodFrom;
                client.PayrollPeriodTo = command.NextPayrollPeriodTo;
                client.ModifiedOn = now;

                var clientEmployeeIds = await _db
                    .Employees
                    .AsNoTracking()
                    .Where(e => !e.DeletedOn.HasValue && e.ClientId == payrollProcessBatch.ClientId && e.DailyRate.HasValue)
                    .Select(e => e.Id)
                    .ToListAsync();

                var zeroBalanceActiveLoans = await _db.Loans
                    .Where(l => !l.DeletedOn.HasValue && clientEmployeeIds.Contains(l.EmployeeId.Value) && !l.ZeroedOutOn.HasValue && l.RemainingBalance <= 0 && DbFunctions.TruncateTime(l.StartDeductionDate) <= DbFunctions.TruncateTime(payrollProcessBatch.PayrollPeriodTo))
                    .ToListAsync();

                zeroBalanceActiveLoans = zeroBalanceActiveLoans.Where(l => l.LoanPayrollPeriods.Contains(payrollProcessBatch.PayrollPeriod.Value)).ToList();

                var payrollRecords = await _db
                    .PayrollRecords
                    .Include(pr => pr.Employee)
                    .AsNoTracking()
                    .Where(pr => pr.PayrollProcessBatchId == payrollProcessBatch.Id)
                    .ToListAsync();

                foreach (var payrollRecord in payrollRecords)
                {
                    if (!payrollRecord.LoansDeducted) continue;

                    var employeeZeroBalanceActiveLoans = zeroBalanceActiveLoans.Where(l => l.EmployeeId == payrollRecord.EmployeeId);

                    foreach (var loan in employeeZeroBalanceActiveLoans)
                    {
                        loan.ZeroedOutOn = now;
                    }
                }

                await _db.SaveChangesAsync();
                
                //await CreatePayslipFiles(command, payrollProcessBatch);

                return new CommandResult();
            }

            private async Task CreatePayslipFiles(Command command, PayrollProcessBatch payrollProcessBatch)
            {
                var payslipReportQuery = new PayslipReport.Query
                {
                    PayrollProcessBatchId = payrollProcessBatch.Id
                };

                var payslipReportQueryResult = await _mediator.Send(payslipReportQuery);

                var tupledCollection = Tuple.Create(payslipReportQueryResult, payslipReportQueryResult.PayslipRecords);

                var saveDirectoryBase = HttpContext.Current.Server.MapPath("~/wwwroot/payslips/");
                var saveDirectoryForBatch = Path.Combine(saveDirectoryBase, $"{payrollProcessBatch.Client.Code} - {payrollProcessBatch.PayrollPeriodFromFormatted} to {payrollProcessBatch.PayrollPeriodToFormatted}/");

                if (!Directory.Exists(saveDirectoryForBatch))
                {
                    Directory.CreateDirectory(saveDirectoryForBatch);
                }

                foreach (var payslipRecord in tupledCollection.Item2)
                {
                    var tupledIndividual = Tuple.Create(payslipReportQueryResult, payslipRecord);

                    var partialRendered = command.AppController.RenderPartialToString("PayslipReportTableIndividual", tupledIndividual);

                    var converter = new HtmlToPdf();
                    converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                    var doc = converter.ConvertHtmlString(partialRendered);

                    var saveFileName = Path.Combine(saveDirectoryForBatch, $"{payslipRecord.PayrollRecord.Employee.EmployeeCode} - {payslipRecord.PayrollRecord.Employee.LastName}, {payslipRecord.PayrollRecord.Employee.FirstName} {(String.IsNullOrWhiteSpace(payslipRecord.PayrollRecord.Employee.MiddleName) ? "" : payslipRecord.PayrollRecord.Employee.MiddleName.First().ToString())}.pdf");

                    doc.Save(saveFileName);
                    doc.Close();
                }
            }
        }
    }
}
﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Humanizer;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.Payroll
{
    public class HoldReport
    {
        public class Query : IRequest<QueryResult>
        {
            public int? PayrollProcessBatchId { get; set; }
            public string DisplayMode { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(q => q.PayrollProcessBatchId)
                    .NotEmpty();
            }
        }

        public class QueryResult
        {
            public int? PayrollProcessBatchId { get; set; }
            public string DisplayMode { get; set; }
            public IEnumerable<PayrollRecord> PayrollRecords { get; set; } = new List<PayrollRecord>();
            public PayrollProcessBatch PayrollProcessBatchResult { get; set; }

            public class PayrollRecord
            {
                public DateTime AddedOn { get; set; }
                public DateTime? DeletedOn { get; set; }
                public int Id { get; set; }
                public DateTime? ModifiedOn { get; set; }

                public Employee Employee { get; set; }
                public int? EmployeeId { get; set; }
                public int? PayrollProcessBatchId { get; set; }

                public decimal? DaysWorkedValue { get; set; }
                public decimal? HoursWorkedValue { get; set; }
                public decimal? OvertimeValue { get; set; }
                public decimal? HoursLateValue { get; set; }
                public decimal? HoursUndertimeValue { get; set; }
                public decimal? COLADailyValue { get; set; }
                public decimal? COLAHourlyValue { get; set; }
                public decimal? COLAHourlyOTValue { get; set; }
                public decimal? EarningsValue { get; set; }
                public decimal? DeductionsValue { get; set; }

                public decimal? SSSValueEmployee { get; set; }
                public decimal? SSSValueEmployer { get; set; }
                public decimal? PHICValueEmployee { get; set; }
                public decimal? PHICValueEmployer { get; set; }
                public decimal? PagIbigValueEmployee { get; set; }
                public decimal? PagIbigValueEmployer { get; set; }
                public decimal? TaxValueEmployee { get; set; }

                public decimal? LoanPaymentValue { get; set; }

                public decimal BasicPayValue => DaysWorkedValue.GetValueOrDefault() + HoursWorkedValue.GetValueOrDefault();
                public decimal TotalEarningsValue => BasicPayValue + OvertimeValue.GetValueOrDefault() - HoursUndertimeValue.GetValueOrDefault() - HoursLateValue.GetValueOrDefault() + COLADailyValue.GetValueOrDefault() + COLAHourlyValue.GetValueOrDefault() + COLAHourlyOTValue.GetValueOrDefault() + EarningsValue.GetValueOrDefault();
                public decimal TotalGovDeductionsValue => SSSValueEmployee.GetValueOrDefault() + PagIbigValueEmployee.GetValueOrDefault() + PHICValueEmployee.GetValueOrDefault() + TaxValueEmployee.GetValueOrDefault();
                public decimal TotalDeductionsValue => TotalGovDeductionsValue + DeductionsValue.GetValueOrDefault() + LoanPaymentValue.GetValueOrDefault();
                public decimal NetPayValue { get; set; }
            }

            public class Employee
            {
                public DateTime AddedOn { get; set; }
                public string ATMAccountNumber { get; set; }
                public string CelNo { get; set; }
                public string CityAddress { get; set; }
                public int? ClientId { get; set; }
                public decimal? COLADaily { get; set; }
                public decimal? COLAHourly { get; set; }
                public decimal? COLAMonthly { get; set; }
                public int? CompanyId { get; set; }
                public decimal? DailyRate { get; set; }
                public DateTime? DateHired { get; set; }
                public DateTime? DateOfBirth { get; set; }
                public DateTime? DateResigned { get; set; }
                public DateTime? DeletedOn { get; set; }
                public Department Department { get; set; }
                public int? DepartmentId { get; set; }
                public string Email { get; set; }
                public string EmployeeCode { get; set; }
                public string EmployeeStatus { get; set; }
                public string FirstName { get; set; }
                public decimal? HourlyRate { get; set; }
                public int Id { get; set; }
                public string LastName { get; set; }
                public bool? LoanExempt { get; set; }
                public string MiddleName { get; set; }
                public DateTime? ModifiedOn { get; set; }
                public decimal? MonthlyRate { get; set; }
                public string Nickname { get; set; }
                public string PagIbig { get; set; }
                public bool? PagIbigExempt { get; set; }
                public string PhilHealth { get; set; }
                public bool? PhilHealthExempt { get; set; }
                public string Position { get; set; }
                public int? ReligionId { get; set; }
                public string ResignStatus { get; set; }
                public string SalaryStatus { get; set; }
                public string SSS { get; set; }
                public bool? TaxExempt { get; set; }
                public int? TaxStatusId { get; set; }
                public string TelNo { get; set; }
                public bool? ThirteenthMonthExempt { get; set; }
                public string TIN { get; set; }
                public string ZipCode { get; set; }
            }

            public class Department
            {
                public int Id { get; set; }
                public string Code { get; set; }
                public string Name { get; set; }
            }

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

                public string PayrollPeriodFromFormatted => $"{PayrollPeriodFrom.Value:MMM d, yyy}";
                public string PayrollPeriodToFormatted => $"{PayrollPeriodTo.Value:MMM d, yyy}";
                public string PayrollPeriodFormatted => PayrollPeriod.Value.Ordinalize();
            }

            public class Client
            {
                public string Code { get; set; }
                public string Description { get; set; }
                public int Id { get; set; }
                public DateTime? ModifiedOn { get; set; }
                public string Name { get; set; }
            }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<PayrollProcessBatch, QueryResult.PayrollProcessBatch>();
                CreateMap<PayrollRecord, QueryResult.PayrollRecord>();
                CreateMap<Employee, QueryResult.Employee>();
                CreateMap<Client, QueryResult.Client>();
                CreateMap<Department, QueryResult.Department>();
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

            public async Task<QueryResult> Handle(Query query, CancellationToken cancellationToken)
            {
                var payrollProcessBatch = await _db.PayrollProcessBatches
                    .AsNoTracking()
                    .Include(ppb => ppb.Client)
                    .SingleAsync(ppb => ppb.Id == query.PayrollProcessBatchId);

                var payrollRecords = await _db.PayrollRecords
                    .AsNoTracking()
                    .Include(pr => pr.Employee)
                    .Include(pr => pr.Employee.Department)
                    .Where(pr => pr.PayrollProcessBatchId == query.PayrollProcessBatchId && pr.Employee.SalaryStatus == SalaryStatus.OnHold)
                    .OrderBy(pr => pr.Employee.LastName)
                    .ThenBy(pr => pr.Employee.FirstName)
                    .ProjectTo<QueryResult.PayrollRecord>(_mapper)
                    .ToListAsync();

                return new QueryResult
                {
                    PayrollProcessBatchId = query.PayrollProcessBatchId,
                    DisplayMode = query.DisplayMode,
                    PayrollProcessBatchResult = _mapper.Map<QueryResult.PayrollProcessBatch>(payrollProcessBatch),
                    PayrollRecords = payrollRecords
                };
            }
        }
    }
}
using AutoMapper;
using AutoMapper.QueryableExtensions;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.Employees
{
    public class Details
    {
        public class Query : IRequest<QueryResult>
        {
            public int EmployeeId { get; set; }
        }

        public class QueryResult
        {
            public string EmployeeCode { get; set; }
            public string CompanyIdNumber { get; set; }
            public int Id { get; set; }

            // Employee Info
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string Nickname { get; set; }
            public string Email { get; set; }
            public string CityAddress { get; set; }
            public string PermanentAddress { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string PlaceOfBirth { get; set; }
            public string ZipCode { get; set; }
            public string TelNo { get; set; }
            public string CelNo { get; set; }
            public Gender? Gender { get; set; }
            public string ReligionCode { get; set; }
            public Citizenship? Citizenship { get; set; }
            public CivilStatus? CivilStatus { get; set; }
            public string SSS { get; set; }
            public string TIN { get; set; }
            public string PagIbig { get; set; }
            public string PhilHealth { get; set; }

            // Company Info
            public string ClientName { get; set; }
            public DateTime? DateHired { get; set; }
            public DateTime? DateResigned { get; set; }
            public string JobTitleName { get; set; }
            public string DepartmentName { get; set; }
            public Region? Region { get; set; }
            public string EmployeeStatus { get; set; }
            public string ResignStatus { get; set; }
            public bool? IsActive { get; set; } = true;

            // Pay Info
            public string ATMAccountNumber { get; set; }
            public AccountType? AccountType { get; set; }
            public string TaxStatusCode { get; set; }
            public string PagIbigRecordCode { get; set; }
            public decimal? HourlyRate { get; set; }
            public decimal? DailyRate { get; set; }
            public decimal? MonthlyRate { get; set; }
            public decimal? COLAHourly { get; set; }
            public decimal? COLADaily { get; set; }
            public decimal? COLAMonthly { get; set; }
            public bool? SSSExempt { get; set; }
            public bool? TaxExempt { get; set; }
            public bool? PagIbigExempt { get; set; }
            public bool? ThirteenthMonthExempt { get; set; }
            public bool? PhilHealthExempt { get; set; }
            public bool? LoanExempt { get; set; }
            public string SalaryStatus { get; set; }

            // Submitted Files
            public bool? SubmittedBiodata { get; set; }
            public bool? SubmittedIdPictures { get; set; }
            public bool? SubmittedNBIClearance { get; set; }
            public bool? SubmittedPoliceClearance { get; set; }
            public bool? SubmittedBarangayClearance { get; set; }
            public bool? SubmittedSSSIdOrED1Form { get; set; }
            public bool? SubmittedPhilHealthIdOrMDRForm { get; set; }
            public bool? SubmittedPagIbigIdOrMIDNo { get; set; }
            public bool? SubmittedTINIdOr1902Form { get; set; }
            public bool? SubmittedBirthCertificate { get; set; }
            public bool? SubmittedMarriageCertification { get; set; }
            public bool? SubmittedBirthCertificateOfChildren { get; set; }
            public bool? SubmittedDiplomaOrTCR { get; set; }
            public bool? SubmittedPreEmploymentMedicalResult { get; set; }
            public bool? SubmittedSSSLoanVerification { get; set; }

            public IList<RehireTransferEvent> RehireTransferEvents { get; set; } = new List<RehireTransferEvent>();

            public class RehireTransferEvent
            {
                public DateTime RehireTransferDateLocal { get; set; }
                public int? EmployeeId { get; set; }
                public Client Client { get; set; }
                public int? ClientId { get; set; }
            }

            public class Client
            {
                public string Code { get; set; }
                public int Id { get; set; }
                public string Name { get; set; }
            }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Employee, QueryResult>();
                CreateMap<RehireTransferEvent, QueryResult.RehireTransferEvent>();
                CreateMap<Client, QueryResult.Client>();
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
                var rehireTransferEvents = await _db
                    .RehireTransferEvents
                    .Where(rte => rte.EmployeeId == query.EmployeeId)
                    .ToListAsync();

                if (rehireTransferEvents.Count == 0)
                {
                    var employee = await _db
                        .Employees
                        .Select(e => new
                        {
                            AddedOn = e.AddedOn,
                            ClientId = e.ClientId,
                            Id = e.Id
                        })
                        .SingleAsync(e => e.Id == query.EmployeeId);

                    var originalRehireTransferEvent = new RehireTransferEvent
                    {
                        AddedOn = employee.AddedOn,
                        ClientId = employee.ClientId,
                        EmployeeId = employee.Id,
                        RehireTransferDateLocal = employee.AddedOn.AddHours(8), // all employees so far are based in the PH
                        Type = RehireTransferEventType.New
                    };
                    _db.RehireTransferEvents.Add(originalRehireTransferEvent);
                    await _db.SaveChangesAsync();
                }

                var queryResult = await _db
                    .Employees
                    .AsNoTracking()
                    .Include(e => e.RehireTransferEvents)
                    .Include(e => e.RehireTransferEvents.Select(rte => rte.Client))
                    .Where(r => r.Id == query.EmployeeId && !r.DeletedOn.HasValue)
                    .ProjectTo<QueryResult>(_mapper)
                    .SingleAsync();

                if (queryResult.RehireTransferEvents.Any())
                {
                    queryResult.RehireTransferEvents = queryResult.RehireTransferEvents.OrderBy(rte => rte.RehireTransferDateLocal).ToList();
                }

                return queryResult;
            }
        }
    }
}
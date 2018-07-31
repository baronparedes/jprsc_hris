using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Security;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.Employees
{
    public class Details
    {
        public class Query : IRequest<QueryResult>
        {
            public int EmployeeId { get; set; }
        }

        public class QueryResult
        {
            public IList<SelectListItem> ReligionsList { get; set; } = new List<SelectListItem>();
            public IList<SelectListItem> ClientsList { get; set; } = new List<SelectListItem>();
            public IList<SelectListItem> DepartmentsList { get; set; } = new List<SelectListItem>();
            public IList<SelectListItem> TaxStatusesList { get; set; } = new List<SelectListItem>();

            public string EmployeeCode { get; set; }
            public int Id { get; set; }

            // Employee Info
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string Nickname { get; set; }
            public string Email { get; set; }
            public string CityAddress { get; set; }
            public DateTime? DateOfBirth { get; set; }
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
            public string Position { get; set; }
            public string DepartmentName { get; set; }
            public string Region { get; set; }
            public string EmployeeStatus { get; set; }
            public string ResignStatus { get; set; }
            public bool? IsActive { get; set; } = true;

            // Pay Info
            public string ATMAccountNumber { get; set; }
            public AccountType? AccountType { get; set; }
            public string TaxStatusCode { get; set; }
            public decimal? HourlyRate { get; set; }
            public decimal? DailyRate { get; set; }
            public decimal? COLAHourly { get; set; }
            public decimal? COLADaily { get; set; }
            public bool? TaxExempt { get; set; }
            public bool? PagIbigExempt { get; set; }
            public bool? ThirteenthMonthExempt { get; set; }
            public bool? PhilHealthExempt { get; set; }
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
                var result = await _db
                    .Employees
                    .Where(r => r.Id == query.EmployeeId && !r.DeletedOn.HasValue)
                    .ProjectToSingleAsync<QueryResult>();

                return result;
            }
        }
    }
}
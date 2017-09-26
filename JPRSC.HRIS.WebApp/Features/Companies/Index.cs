using AutoMapper;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Companies
{
    public class Index
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
            public IEnumerable<CompanyProfile> CompanyProfiles { get; set; } = new List<CompanyProfile>();

            public class CompanyProfile
            {
                public DateTime AddedOn { get; set; }
                public string Address { get; set; }
                public string Code { get; set; }
                public string Email { get; set; }
                public int Id { get; set; }
                public DateTime? ModifiedOn { get; set; }
                public string Name { get; set; }
                public string Phone { get; set; }
                public string Position { get; set; }
                public string Signatory { get; set; }
            }
        }

        public class Handler : IAsyncRequestHandler<Query, Result>
        {
            private readonly ApplicationDbContext _db;

            public Handler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<Result> Handle(Query query)
            {
                var surveys = await _db
                    .CompanyProfiles
                    .ProjectToListAsync<Result.CompanyProfile>();

                return new Result
                {
                    CompanyProfiles = surveys
                };
            }
        }
    }
}
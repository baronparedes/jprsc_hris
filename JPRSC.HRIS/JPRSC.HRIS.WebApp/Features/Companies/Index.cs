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
        public class Query : IRequest<QueryResult>
        {
        }

        public class QueryResult
        {
            public IEnumerable<CompanyProfile> CompanyProfiles { get; set; } = new List<CompanyProfile>();

            public class CompanyProfile
            {
                public string Address { get; set; }
                public string Email { get; set; }
                public int Id { get; set; }
                public string Name { get; set; }
                public string Phone { get; set; }
            }
        }

        public class QueryHandler : IAsyncRequestHandler<Query, QueryResult>
        {
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<QueryResult> Handle(Query query)
            {
                var companyProfiles = await _db
                    .CompanyProfiles
                    .ProjectToListAsync<QueryResult.CompanyProfile>();

                return new QueryResult
                {
                    CompanyProfiles = companyProfiles
                };
            }
        }
    }
}
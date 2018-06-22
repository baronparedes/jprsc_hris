using AutoMapper;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.SSSRecords
{
    public class Search
    {
        public class Query : IRequest<QueryResult>
        {
            public string SearchTerm { get; set; }

            public string SearchLikeTerm
            {
                get
                {
                    if (String.IsNullOrWhiteSpace(SearchTerm)) return null;

                    return $"%{SearchTerm}%";
                }
            }
        }

        public class QueryResult
        {
            public IEnumerable<SSSRecord> SSSRecords { get; set; } = new List<SSSRecord>();

            public class SSSRecord
            {
                public decimal? ECC { get; set; }
                public decimal? Employee { get; set; }
                public decimal? Employer { get; set; }
                public int Id { get; set; }
                public int? Number { get; set; }
                public decimal? PhilHealthEmployee { get; set; }
                public decimal? PhilHealthEmployer { get; set; }
                public decimal? Range1 { get; set; }
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
                var dbQuery = _db
                    .SSSRecords
                    .Where(sr => !sr.DeletedOn.HasValue);

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {

                }

                var sssRecords = await dbQuery
                    .OrderBy(sr => sr.Id)
                    .Take(AppSettings.Int("DefaultGridPageSize"))
                    .ProjectToListAsync<QueryResult.SSSRecord>();

                return new QueryResult
                {
                    SSSRecords = sssRecords
                };
            }
        }
    }
}
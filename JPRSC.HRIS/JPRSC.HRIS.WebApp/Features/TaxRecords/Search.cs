using AutoMapper;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.TaxRecords
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
            public IEnumerable<TaxRecord> TaxRecords { get; set; } = new List<TaxRecord>();

            public class TaxRecord
            {
                public string Code { get; set; }
                public double? Exemption { get; set; }
                public string Name { get; set; }
                public int Id { get; set; }
                public IList<TaxRange> TaxRanges { get; set; } = new List<TaxRange>();
            }

            public class TaxRange
            {
                [JsonIgnore]
                public DateTime? DeletedOn { get; set; }
                public decimal? From { get; set; }
                public int Id { get; set; }
                public double? Percentage { get; set; }
                public decimal? Plus { get; set; }
                public decimal? To { get; set; }
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
                    .TaxRecords
                    .Include(tr => tr.TaxRanges)
                    .Where(tr => !tr.DeletedOn.HasValue);

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {
                    dbQuery = dbQuery
                        .Where(tr => DbFunctions.Like(tr.Name, query.SearchLikeTerm) ||
                            DbFunctions.Like(tr.Code, query.SearchLikeTerm));
                }

                var taxRecords = await dbQuery
                    .OrderBy(r => r.Id)
                    .Take(AppSettings.Int("DefaultGridPageSize"))
                    .ProjectToListAsync<QueryResult.TaxRecord>();

                foreach (var taxRecord in taxRecords)
                {
                    taxRecord.TaxRanges = taxRecord.TaxRanges.Where(tr => !tr.DeletedOn.HasValue).ToList();
                }

                return new QueryResult
                {
                    TaxRecords = taxRecords
                };
            }
        }
    }
}
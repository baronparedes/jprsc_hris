using AutoMapper;
using AutoMapper.QueryableExtensions;
using JPRSC.HRIS.Infrastructure.Configuration;
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
    public class ForProcessingBatchSearch
    {
        public class Query : IRequest<QueryResult>
        {
            public string SearchTerm { get; set; }
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }

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
            public IList<ForProcessingBatch> ForProcessingBatches { get; set; } = new List<ForProcessingBatch>();
            public int LastPageNumber { get; set; }
            public int TotalResultsCount { get; set; }

            public class ForProcessingBatch
            {
                public string ClientCode { get; set; }
                public string ClientName { get; set; }
                public string DateFormatted { get; set; }
                public string EmployeeIds { get; set; }
                public IList<int> EmployeeIdsList => String.IsNullOrWhiteSpace(EmployeeIds) ? new List<int>() : EmployeeIds.Split(',').Select(id => Convert.ToInt32(id)).ToList();
                public int Id { get; set; }
                public string Name { get; set; }
            }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<ForProcessingBatch, QueryResult.ForProcessingBatch>();
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
                var pageNumber = query.PageNumber.HasValue && query.PageNumber > 0 ? query.PageNumber.Value : 1;
                var pageSize = query.PageSize.HasValue && query.PageSize > 0 ? Math.Min(query.PageSize.Value, 1000) : AppSettings.Int("DefaultGridPageSize");

                var dbQuery = _db
                    .ForProcessingBatches
                    .AsNoTracking()
                    .Where(fpb => !fpb.DeletedOn.HasValue);

                if (!String.IsNullOrWhiteSpace(query.SearchLikeTerm))
                {
                    dbQuery = dbQuery
                        .Where(r => DbFunctions.Like(r.Name, query.SearchLikeTerm));
                }

                var totalResultsCount = await dbQuery
                    .CountAsync();

                var forProcessingBatches = await dbQuery
                    .OrderByDescending(fpb => fpb.ProcessedOn)
                    .PageBy(pageNumber, pageSize)
                    .ProjectTo<QueryResult.ForProcessingBatch>(_mapper)
                    .ToListAsync();

                var remainder = totalResultsCount % pageSize;
                var divisor = totalResultsCount / pageSize;
                var lastPageNumber = remainder > 0 ? divisor + 1 : divisor;

                return new QueryResult
                {
                    ForProcessingBatches = forProcessingBatches,
                    LastPageNumber = lastPageNumber,
                    TotalResultsCount = totalResultsCount
                };
            }
        }
    }
}
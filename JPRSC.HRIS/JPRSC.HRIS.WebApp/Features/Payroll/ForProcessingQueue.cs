using AutoMapper;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Payroll
{
    public class ForProcessingQueue
    {
        public class Query : IRequest<QueryResult>
        {
        }

        public class QueryResult
        {
            public IList<ForProcessingBatch> ForProcessingBatches { get; set; } = new List<ForProcessingBatch>();

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

        public class QueryHandler : IRequestHandler<Query, QueryResult>
        {
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<QueryResult> Handle(Query query, CancellationToken token)
            {
                var forProcessingBatches = await _db
                    .ForProcessingBatches
                    .OrderBy(fpb => fpb.ProcessedOn)
                    .ProjectToListAsync<QueryResult.ForProcessingBatch>();

                return new QueryResult
                {
                    ForProcessingBatches = forProcessingBatches
                };
            }
        }
    }
}
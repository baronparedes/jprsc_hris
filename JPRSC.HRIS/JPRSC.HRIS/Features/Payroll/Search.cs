using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
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
using System.Web;

namespace JPRSC.HRIS.Features.Payroll
{
    public class Search
    {
        public class Query : IRequest<QueryResult>
        {
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
            public int? ClientId { get; set; }
            public Month? PayrollPeriodMonth { get; set; }
            public int? PayrollPeriodYear { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
            }
        }

        public class QueryResult
        {
            public IEnumerable<PayrollProcessBatch> PayrollProcessBatches { get; set; } = new List<PayrollProcessBatch>();
            public int LastPageNumber { get; set; }
            public int TotalResultsCount { get; set; }

            public class PayrollProcessBatch
            {
                public DateTime AddedOn { get; set; }
                public DateTime? DeletedOn { get; set; }
                public int Id { get; set; }
                public DateTime? ModifiedOn { get; set; }
                public Client Client { get; set; }
                public int? ClientId { get; set; }
                public DateTime? EndProcessedOn { get; set; }
                public int? PayrollPeriod { get; set; }
                public DateTime? PayrollPeriodFrom { get; set; }
                public Month? PayrollPeriodMonth { get; set; }
                public DateTime? PayrollPeriodTo { get; set; }

                public bool IsEndProcessed => EndProcessedOn.HasValue;
                public string MonthText => PayrollPeriodMonth.HasValue ? PayrollPeriodMonth.Value.ToString() : null;
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

            public async Task<QueryResult> Handle(Query query, CancellationToken cancellationToken)
            {
                var pageNumber = query.PageNumber.HasValue && query.PageNumber > 0 ? query.PageNumber.Value : 1;
                var pageSize = query.PageSize.HasValue && query.PageSize > 0 ? Math.Min(query.PageSize.Value, 1000) : AppSettings.Int("DefaultGridPageSize");

                var dbQuery = _db
                    .PayrollProcessBatches
                    .AsNoTracking()
                    .Where(ppb => !ppb.DeletedOn.HasValue && !ppb.DateOverwritten.HasValue);

                if (query.ClientId.HasValue && query.ClientId.Value > 0)
                {
                    dbQuery = dbQuery
                        .Where(ppb => ppb.ClientId == query.ClientId);
                }

                if (query.PayrollPeriodMonth.HasValue && query.PayrollPeriodMonth.Value > 0)
                {
                    dbQuery = dbQuery
                        .Where(ppb => ppb.PayrollPeriodMonth == query.PayrollPeriodMonth);
                }

                if (query.PayrollPeriodYear.HasValue && query.PayrollPeriodYear.Value > 0)
                {
                    dbQuery = dbQuery
                        .Where(ppb => ppb.PayrollPeriodFrom.HasValue && ppb.PayrollPeriodFrom.Value.Year == query.PayrollPeriodYear.Value);
                }

                var totalResultsCount = await dbQuery
                    .CountAsync();

                var payrollProcessBatches = await dbQuery
                    .OrderByDescending(ppb => ppb.AddedOn)
                    .PageBy(pageNumber, pageSize)
                    .ProjectTo<QueryResult.PayrollProcessBatch>(_mapper)
                    .ToListAsync();

                var remainder = totalResultsCount % pageSize;
                var divisor = totalResultsCount / pageSize;
                var lastPageNumber = remainder > 0 ? divisor + 1 : divisor;

                return new QueryResult
                {
                    PayrollProcessBatches = payrollProcessBatches,
                    LastPageNumber = lastPageNumber,
                    TotalResultsCount = totalResultsCount
                };
            }
        }
    }
}
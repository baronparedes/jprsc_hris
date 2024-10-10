using AutoMapper;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.CompanyClientTags
{
    public class GetByClientId
    {
        public class Query : IRequest<QueryResult>
        {
            public int? ClientId { get; set; }
        }

        public class QueryResult
        {
            public string CompanyName { get; set; }
            public int? ClientId { get; set; }
            public int? CompanyId { get; set; }
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
                // TODO: Move to Configuration
                var defaultTag = new QueryResult
                {
                    ClientId = query.ClientId,
                    CompanyName = "Job Placement Resources Services Cooperative"
                };

                if (query.ClientId == null) return defaultTag;

                var dbQuery = _db
                    .CompanyClientTags
                    .AsNoTracking()
                    .Where(e => e.ClientId == query.ClientId);

                var result = await dbQuery
                    .Include(e => e.Company)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (result != null)
                {
                    return new QueryResult
                    {
                        ClientId = query.ClientId,
                        CompanyId = result.CompanyId,
                        CompanyName = result.Company.Name
                    };
                }

                return defaultTag;
            }
        }
    }
}

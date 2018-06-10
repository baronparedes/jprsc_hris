using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.TaxRecords
{
    public class Index
    {
        public class Query : IRequest<QueryResult>
        {
        }

        public class QueryResult
        {
        }

        public class QueryHandler : IRequestHandler<Query, QueryResult>
        {
            public async Task<QueryResult> Handle(Query query, CancellationToken token)
            {
                return new QueryResult();
            }
        }
    }
}
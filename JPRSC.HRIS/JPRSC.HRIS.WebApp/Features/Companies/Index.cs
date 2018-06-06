using MediatR;
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
        }

        public class QueryHandler : IRequestHandler<Query, QueryResult>
        {
            public async Task<QueryResult> Handle(Query query, System.Threading.CancellationToken token)
            {
                return new QueryResult();
            }
        }
    }
}
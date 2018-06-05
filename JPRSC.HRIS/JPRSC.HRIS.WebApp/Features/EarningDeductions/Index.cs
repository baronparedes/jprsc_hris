using MediatR;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.EarningDeductions
{
    public class Index
    {
        public class Query : IRequest<QueryResult>
        {
        }

        public class QueryResult
        {
        }

        public class QueryHandler : AsyncRequestHandler<Query, QueryResult>
        {
            protected override async Task<QueryResult> HandleCore(Query query)
            {
                return new QueryResult();
            }
        }
    }
}
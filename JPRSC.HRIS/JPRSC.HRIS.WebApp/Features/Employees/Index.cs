using MediatR;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Employees
{
    public class Index
    {
        public class Query : IRequest<QueryResult>
        {
        }

        public class QueryResult
        {
        }

        public class QueryHandler : IAsyncRequestHandler<Query, QueryResult>
        {
            public async Task<QueryResult> Handle(Query query)
            {
                return new QueryResult();
            }
        }
    }
}
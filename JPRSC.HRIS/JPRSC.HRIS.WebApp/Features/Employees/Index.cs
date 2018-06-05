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

        public class QueryHandler : AsyncRequestHandler<Query, QueryResult>
        {
            protected override async Task<QueryResult> HandleCore(Query query)
            {
                return new QueryResult();
            }
        }
    }
}
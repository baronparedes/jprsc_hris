using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.Loans
{
    public class Index
    {
        public class Query : IRequest<QueryResult>
        {
        }

        public class QueryResult
        {
            public IList<SelectListItem> ClientsList { get; set; } = new List<SelectListItem>();
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
                return new QueryResult
                {
                    ClientsList = await GetClientsList()
                };
            }

            private async Task<IList<SelectListItem>> GetClientsList()
            {
                var clients = await _db.Clients.Where(c => !c.DeletedOn.HasValue).ToListAsync();

                return clients
                    .Select(c => new SelectListItem
                    {
                        Text = c.Code,
                        Value = c.Id.ToString()
                    })
                    .ToList();
            }
        }
    }
}
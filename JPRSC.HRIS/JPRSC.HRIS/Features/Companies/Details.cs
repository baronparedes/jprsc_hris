using AutoMapper;
using AutoMapper.QueryableExtensions;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.Companies
{
    public class Details
    {
        public class Query : IRequest<QueryResult>
        {
            public int CompanyId { get; set; }
        }

        public class QueryResult
        {
            public string Address { get; set; }
            public string BOI { get; set; }
            public string Code { get; set; }
            public DateTime? DateIssued { get; set; }
            public string DTI { get; set; }
            public string Email { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
            public string PagIbig { get; set; }
            public string PERAA { get; set; }
            public string PhilHealth { get; set; }
            public string Phone { get; set; }
            public string PlaceIssued { get; set; }
            public string Registration { get; set; }
            public string SEC { get; set; }
            public string SSS { get; set; }
            public string VAT { get; set; }
            public string ZipCode { get; set; }
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
                return await _db
                    .Companies
                    .AsNoTracking()
                    .Where(cp => cp.Id == query.CompanyId && !cp.DeletedOn.HasValue)
                    .ProjectTo<QueryResult>(_mapper)
                    .SingleAsync();
            }
        }
    }
}
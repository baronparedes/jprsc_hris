using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Loans
{
    public class CheckExisting
    {
        public class Query : IRequest<QueryResult>
        {
            public int? EmployeeId { get; set; }
            public int? LoanTypeId { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(c => c.EmployeeId)
                    .NotEmpty();

                RuleFor(c => c.LoanTypeId)
                    .NotEmpty();
            }
        }

        public class QueryResult
        {
            public bool HasExisting { get; set; }
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
                var hasExisting = await _db
                    .Loans
                    .AnyAsync(l => !l.DeletedOn.HasValue && !l.ZeroedOutOn.HasValue && l.RemainingBalance > 0 && l.EmployeeId.Value == query.EmployeeId.Value && l.LoanTypeId.Value == query.LoanTypeId.Value);

                return new QueryResult
                {
                    HasExisting = hasExisting
                };
            }
        }
    }
}
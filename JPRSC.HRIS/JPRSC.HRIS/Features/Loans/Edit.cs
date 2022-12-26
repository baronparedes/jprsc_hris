using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.Loans
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int LoanId { get; set; }
        }

        public class Command : IRequest
        {
            public decimal? DeductionAmount { get; set; }
            public int Id { get; set; }
            public string LoanPayrollPeriod { get; set; }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Loan, Command>();
            }
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly ApplicationDbContext _db;
            private readonly IMapper _mapper;

            public QueryHandler(ApplicationDbContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Command> Handle(Query query, CancellationToken token)
            {
                return await _db.Loans.AsNoTracking().Where(r => r.Id == query.LoanId && !r.DeletedOn.HasValue).ProjectTo<Command>(_mapper).SingleAsync();
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.DeductionAmount)
                    .NotEmpty();

                RuleFor(c => c.LoanPayrollPeriod)
                    .NotEmpty();
            }
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command command, CancellationToken token)
            {
                var loan = await _db.Loans.SingleAsync(r => r.Id == command.Id);
                loan.DeductionAmount = command.DeductionAmount;
                loan.ModifiedOn = DateTime.UtcNow;
                loan.LoanPayrollPeriod = command.LoanPayrollPeriod;

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Loans
{
    public class Add
    {
        public class Command : IRequest
        {
            public decimal? DeductionAmount { get; set; }
            public int? EmployeeId { get; set; }
            public double? InterestRate { get; set; }
            public DateTime? LoanDate { get; set; }
            public int? LoanTypeId { get; set; }
            public int? MonthsPayable { get; set; }
            public int? PayrollPeriod { get; set; }
            public decimal? PrincipalAmount { get; set; }
            public string TransactionNumber { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.EmployeeId)
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
                var loan = new Loan
                {
                    AddedOn = DateTime.UtcNow,
                    DeductionAmount = command.DeductionAmount,
                    EmployeeId = command.EmployeeId,
                    InterestRate = command.InterestRate,
                    LoanDate = command.LoanDate,
                    LoanTypeId = command.LoanTypeId,
                    MonthsPayable = command.MonthsPayable,
                    PayrollPeriod = command.PayrollPeriod,
                    PrincipalAmount = command.PrincipalAmount,
                    TransactionNumber = command.TransactionNumber
                };

                _db.Loans.Add(loan);
                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
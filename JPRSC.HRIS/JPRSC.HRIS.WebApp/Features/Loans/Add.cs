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
            public decimal? InterestAmount { get; set; }
            public DateTime? LoanDate { get; set; }
            public int? LoanTypeId { get; set; }
            public string LoanPayrollPeriod { get; set; }
            public int? MonthsPayable { get; set; }
            public decimal? PrincipalAmount { get; set; }
            public decimal? RemainingBalance { get; set; }
            public DateTime? StartDeductionDate { get; set; }
            public string TransactionNumber { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.DeductionAmount)
                    .NotEmpty();

                RuleFor(c => c.EmployeeId)
                    .NotEmpty();

                RuleFor(c => c.InterestAmount)
                    .NotEmpty();

                RuleFor(c => c.LoanDate)
                    .NotEmpty();

                RuleFor(c => c.LoanTypeId)
                    .NotEmpty();

                RuleFor(c => c.LoanPayrollPeriod)
                    .NotEmpty();

                RuleFor(c => c.MonthsPayable)
                    .NotEmpty();

                RuleFor(c => c.PrincipalAmount)
                    .NotEmpty();

                RuleFor(c => c.RemainingBalance)
                    .NotEmpty();

                RuleFor(c => c.StartDeductionDate)
                    .NotEmpty();

                RuleFor(c => c.TransactionNumber)
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
                    InterestAmount = command.InterestAmount,
                    LoanDate = command.LoanDate,
                    LoanTypeId = command.LoanTypeId,
                    LoanPayrollPeriod = command.LoanPayrollPeriod,
                    //MonthsPayable = command.MonthsPayable,
                    PrincipalAmount = command.PrincipalAmount,
                    RemainingBalance = command.RemainingBalance,
                    StartDeductionDate = command.StartDeductionDate,
                    TransactionNumber = command.TransactionNumber
                };

                _db.Loans.Add(loan);
                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
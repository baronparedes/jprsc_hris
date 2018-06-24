using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Banks
{
    public class Add
    {
        public class Command : IRequest
        {
            public string AccountNumber { get; set; }
            public string Address1 { get; set; }
            public string BatchNumber { get; set; }
            public string BranchCode { get; set; }
            public string Code { get; set; }
            public string CompanyCode { get; set; }
            public string ContactPerson { get; set; }
            public string Description { get; set; }
            public string Name { get; set; }
            public string OtherBankInfo { get; set; }
            public string Position { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.Code)
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
                var bank = new Bank
                {
                    AccountNumber = command.AccountNumber,
                    AddedOn = DateTime.UtcNow,
                    Address1 = command.Address1,
                    BatchNumber = command.BatchNumber,
                    BranchCode = command.BranchCode,
                    Code = command.Code,
                    CompanyCode = command.CompanyCode,
                    ContactPerson = command.ContactPerson,
                    Description = command.Description,
                    Name = command.Name,
                    OtherBankInfo = command.OtherBankInfo,
                    Position = command.Position
                };

                _db.Banks.Add(bank);
                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Banks
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int BankId { get; set; }
        }

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
            public int Id { get; set; }
            public string Name { get; set; }
            public string OtherBankInfo { get; set; }
            public string Position { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<Command> Handle(Query query, CancellationToken token)
            {
                return await _db.Banks.Where(r => r.Id == query.BankId && !r.DeletedOn.HasValue).ProjectToSingleAsync<Command>();
            }
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
                var bank = await _db.Banks.SingleAsync(r => r.Id == command.Id);

                bank.AccountNumber = command.AccountNumber;
                bank.Address1 = command.Address1;
                bank.BatchNumber = command.BatchNumber;
                bank.BranchCode = command.BranchCode;
                bank.Code = command.Code;
                bank.CompanyCode = command.CompanyCode;
                bank.ContactPerson = command.ContactPerson;
                bank.Description = command.Description;
                bank.ModifiedOn = DateTime.UtcNow;
                bank.Name = command.Name;
                bank.OtherBankInfo = command.OtherBankInfo;
                bank.Position = command.Position;

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
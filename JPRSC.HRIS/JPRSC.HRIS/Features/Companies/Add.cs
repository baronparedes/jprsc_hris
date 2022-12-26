using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.Companies
{
    public class Add
    {
        public class Command : IRequest
        {
            public string Address { get; set; }
            public string Code { get; set; }
            public string BOI { get; set; }
            public DateTime? DateIssued { get; set; }
            public string DTI { get; set; }
            public string Email { get; set; }
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

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.Code)
                    .NotEmpty();

                When(c => !String.IsNullOrWhiteSpace(c.Email), () =>
                {
                    RuleFor(c => c.Email)
                        .EmailAddress();
                });

                RuleFor(c => c.PagIbig)
                    .NotEmpty();

                RuleFor(c => c.PhilHealth)
                    .NotEmpty();

                RuleFor(c => c.SSS)
                    .NotEmpty();

                RuleFor(c => c.VAT)
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
                var Company = new Company
                {
                    AddedOn = DateTime.UtcNow,
                    Address = command.Address,
                    BOI = command.BOI,
                    Code = command.Code,
                    DateIssued = command.DateIssued,
                    DTI = command.DTI,
                    Email = command.Email,
                    Name = command.Name,
                    PagIbig = command.PagIbig,
                    PERAA = command.PERAA,
                    PhilHealth = command.PhilHealth,
                    Phone = command.Phone,
                    PlaceIssued = command.PlaceIssued,
                    Registration = command.Registration,
                    SEC = command.SEC,
                    SSS = command.SSS,
                    VAT = command.VAT,
                    ZipCode = command.ZipCode
                };

                _db.Companies.Add(Company);
                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Companies
{
    public class Add
    {
        public class Command : IRequest
        {
            public string Address { get; set; }
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
                RuleFor(c => c.Address)
                    .NotEmpty();

                RuleFor(c => c.BOI)
                    .MustBeANumber();

                RuleFor(c => c.DateIssued)
                    .NotEmpty();

                RuleFor(c => c.DTI)
                    .NotEmpty()
                    .DependentRules(d =>
                    {
                        d.RuleFor(c => c.DTI)
                            .MustBeANumber();
                    });

                RuleFor(c => c.Email)
                    .EmailAddress();

                RuleFor(c => c.Name)
                    .NotEmpty();

                RuleFor(c => c.PagIbig)
                    .NotEmpty()
                    .DependentRules(d =>
                    {
                        d.RuleFor(c => c.PagIbig)
                            .MustBeANumber();
                    });

                RuleFor(c => c.PhilHealth)
                    .NotEmpty()
                    .DependentRules(d =>
                    {
                        d.RuleFor(c => c.PhilHealth)
                            .MustBeANumber();
                    });

                RuleFor(c => c.PERAA)
                    .NotEmpty()
                    .DependentRules(d =>
                    {
                        d.RuleFor(c => c.PERAA)
                            .MustBeANumber();
                    });

                RuleFor(c => c.Phone)
                    .MustBeANumber();

                RuleFor(c => c.PlaceIssued)
                    .NotEmpty();

                RuleFor(c => c.Registration)
                    .MustBeANumber();

                RuleFor(c => c.SEC)
                    .NotEmpty()
                    .DependentRules(d =>
                    {
                        d.RuleFor(c => c.SEC)
                            .MustBeANumber();
                    });

                RuleFor(c => c.SSS)
                    .NotEmpty()
                    .DependentRules(d =>
                    {
                        d.RuleFor(c => c.SSS)
                            .MustBeANumber();
                    });

                RuleFor(c => c.VAT)
                    .NotEmpty()
                    .DependentRules(d =>
                    {
                        d.RuleFor(c => c.VAT)
                            .MustBeANumber();
                    });

                RuleFor(c => c.ZipCode)
                    .NotEmpty()
                    .DependentRules(d =>
                    {
                        d.RuleFor(c => c.ZipCode)
                            .MustBeANumber();
                    });
            }
        }

        public class CommandHandler : IAsyncRequestHandler<Command>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task Handle(Command command)
            {
                var Company = new Company
                {
                    AddedOn = DateTime.UtcNow,
                    Address = command.Address,
                    BOI = command.BOI,
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
            }
        }
    }
}
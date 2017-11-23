using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Companies
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int CompanyProfileId { get; set; }
        }

        public class Command : IRequest
        {
            public string Address { get; set; }
            public string BOI { get; set; }
            public DateTime? DateIssued { get; set; }
            public string DTI { get; set; }
            public string Email { get; set; }
            public int Id { get; set; }
            public bool? IsDefault { get; set; }
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

        public class QueryHandler : IAsyncRequestHandler<Query, Command>
        {
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<Command> Handle(Query query)
            {
                return await _db.CompanyProfiles.Where(cp => cp.Id == query.CompanyProfileId && !cp.DeletedOn.HasValue).ProjectToSingleAsync<Command>();
            }
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
                var companyProfile = _db.CompanyProfiles.Single(cp => cp.Id == command.Id);

                companyProfile.Address = command.Address;
                companyProfile.BOI = command.BOI;
                companyProfile.DateIssued = command.DateIssued;
                companyProfile.DTI = command.DTI;
                companyProfile.Email = command.Email;
                companyProfile.IsDefault = command.IsDefault;
                companyProfile.ModifiedOn = DateTime.UtcNow;
                companyProfile.Name = command.Name;
                companyProfile.PagIbig = command.PagIbig;
                companyProfile.PERAA = command.PERAA;
                companyProfile.PhilHealth = command.PhilHealth;
                companyProfile.Phone = command.Phone;
                companyProfile.PlaceIssued = command.PlaceIssued;
                companyProfile.Registration = command.Registration;
                companyProfile.SEC = command.SEC;
                companyProfile.SSS = command.SSS;
                companyProfile.VAT = command.VAT;
                companyProfile.ZipCode = command.ZipCode;

                if (command.IsDefault.HasValue && command.IsDefault.HasValue)
                {
                    _db.Database.ExecuteSqlCommand($"UPDATE [dbo].[CompanyProfiles] SET [IsDefault] = 0 WHERE [IsDefault] = 1");
                }

                await _db.SaveChangesAsync();
            }
        }
    }
}
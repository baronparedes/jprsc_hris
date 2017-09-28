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
            public string Code { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Position { get; set; }
            public string Signatory { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(c => c.Code)
                    .NotEmpty();
            }
        }

        public class Handler : IAsyncRequestHandler<Command>
        {
            private readonly ApplicationDbContext _db;

            public Handler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task Handle(Command command)
            {
                var companyProfile = new CompanyProfile
                {
                    AddedOn = DateTime.UtcNow,
                    Address = command.Address,
                    Code = command.Code,
                    Email = command.Email,
                    Name = command.Name,
                    Phone = command.Phone,
                    Position = command.Position,
                    Signatory = command.Signatory
                };

                _db.CompanyProfiles.Add(companyProfile);
                await _db.SaveChangesAsync();
            }
        }
    }
}
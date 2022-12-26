using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.Companies
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int CompanyId { get; set; }
        }

        public class Command : IRequest
        {
            public string Address { get; set; }
            public string BOI { get; set; }
            public string Code { get; set; }
            public DateTime? DateIssued { get; set; }
            public string DTI { get; set; }
            public string Email { get; set; }
            public int Id { get; set; }
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

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Company, Command>();
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
                return await _db.Companies.AsNoTracking().Where(cp => cp.Id == query.CompanyId && !cp.DeletedOn.HasValue).ProjectTo<Command>(_mapper).SingleAsync();
            }
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

                RuleFor(c => c.Id)
                    .NotEmpty();

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
                var company = await _db.Companies.SingleAsync(cp => cp.Id == command.Id);

                company.Address = command.Address;
                company.BOI = command.BOI;
                company.Code = command.Code;
                company.DateIssued = command.DateIssued;
                company.DTI = command.DTI;
                company.Email = command.Email;
                company.ModifiedOn = DateTime.UtcNow;
                company.Name = command.Name;
                company.PagIbig = command.PagIbig;
                company.PERAA = command.PERAA;
                company.PhilHealth = command.PhilHealth;
                company.Phone = command.Phone;
                company.PlaceIssued = command.PlaceIssued;
                company.Registration = command.Registration;
                company.SEC = command.SEC;
                company.SSS = command.SSS;
                company.VAT = command.VAT;
                company.ZipCode = command.ZipCode;

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
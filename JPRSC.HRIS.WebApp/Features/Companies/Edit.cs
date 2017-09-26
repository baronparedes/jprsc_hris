using AutoMapper;
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
            public DateTime AddedOn { get; set; }
            public string Address { get; set; }
            public string Code { get; set; }
            public string Email { get; set; }
            public int Id { get; set; }
            public DateTime? ModifiedOn { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Position { get; set; }
            public string Signatory { get; set; }
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
                return await _db.CompanyProfiles.Where(cp => cp.Id == query.CompanyProfileId).ProjectToSingleAsync<Command>();
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
                //companyProfile.Code = command.Code;
                companyProfile.Email = command.Email;
                companyProfile.ModifiedOn = DateTime.UtcNow;
                companyProfile.Name = command.Name;
                companyProfile.Phone = command.Phone;
                companyProfile.Position = command.Position;
                companyProfile.Signatory = command.Signatory;

                await _db.SaveChangesAsync();
            }
        }
    }
}
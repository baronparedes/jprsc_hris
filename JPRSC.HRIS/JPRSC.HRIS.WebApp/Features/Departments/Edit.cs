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

namespace JPRSC.HRIS.WebApp.Features.Departments
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int DepartmentId { get; set; }
        }

        public class Command : IRequest
        {
            public string Code { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Department, Command>();
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
                return await _db.Departments.AsNoTracking().Where(r => r.Id == query.DepartmentId && !r.DeletedOn.HasValue).ProjectTo<Command>(_mapper).SingleAsync();
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.Name)
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
                var department = await _db.Departments.SingleAsync(r => r.Id == command.Id);

                department.Code = command.Code;
                department.ModifiedOn = DateTime.UtcNow;
                department.Name = command.Name;

                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
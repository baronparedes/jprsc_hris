using AutoMapper;
using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;
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
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class QueryHandler : AsyncRequestHandler<Query, Command>
        {
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            protected override async Task<Command> HandleCore(Query query)
            {
                return await _db.Departments.Where(r => r.Id == query.DepartmentId && !r.DeletedOn.HasValue).ProjectToSingleAsync<Command>();
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

        public class CommandHandler : AsyncRequestHandler<Command>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            protected override async Task HandleCore(Command command)
            {
                var department = await _db.Departments.SingleAsync(r => r.Id == command.Id);
                
                department.ModifiedOn = DateTime.UtcNow;
                department.Name = command.Name;

                await _db.SaveChangesAsync();
            }
        }
    }
}
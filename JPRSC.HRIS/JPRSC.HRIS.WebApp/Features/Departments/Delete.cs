﻿using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Departments
{
    public class Delete
    {
        public class Command : IRequest<CommandResult>
        {
            public int? DepartmentId { get; set; }
        }

        public class CommandResult
        {
            public string Name { get; set; }
        }

        public class CommandHandler : IAsyncRequestHandler<Command, CommandResult>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<CommandResult> Handle(Command command)
            {
                var department = _db.Departments.Single(r => r.Id == command.DepartmentId);
                department.DeletedOn = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                return new CommandResult
                {
                    Name = department.Name
                };
            }
        }
    }
}
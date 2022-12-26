using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.Infrastructure.Excel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Features.Payroll
{
    public class ExportQueueToDTRExcel
    {
        public class Query : IRequest<QueryResult>
        {
            public int ForProcessingBatchId { get; set; }
        }

        public class QueryResult
        {
            public byte[] FileContent { get; set; }
            public string Filename { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, QueryResult>
        {
            private readonly ApplicationDbContext _db;
            private readonly IExcelBuilder _excelBuilder;
            private readonly IMediator _mediator;

            public QueryHandler(ApplicationDbContext db, IExcelBuilder excelBuilder, IMediator mediator)
            {
                _db = db;
                _excelBuilder = excelBuilder;
                _mediator = mediator;
            }

            public async Task<QueryResult> Handle(Query query, CancellationToken token)
            {
                var forProcessingBatch = await _db
                    .ForProcessingBatches
                    .SingleAsync(fpb => fpb.Id == query.ForProcessingBatchId);

                var employeeIds = forProcessingBatch.EmployeeIdsList;

                var employees = await _db
                    .Employees
                    .Where(e => employeeIds.Contains(e.Id))
                    .ToListAsync();

                var payRates = await _db
                    .PayPercentages
                    .ToListAsync();

                var dtrLines = GetDTRLines(employees, payRates);
                var dtrFileContent = _excelBuilder.BuildExcelFile(dtrLines);
                var dtrFilename = $"DTR {forProcessingBatch.Name}.xlsx";

                return new QueryResult
                {
                    FileContent = dtrFileContent,
                    Filename = dtrFilename
                };
            }

            private IList<IList<string>> GetDTRLines(IEnumerable<Employee> employees, IEnumerable<PayPercentage> payRates)
            {
                var lines = new List<IList<string>>();

                var header = new List<string>();
                header.Add("Employee Code");
                header.Add("Last Name");
                header.Add("First Name");
                header.Add("Days Worked");
                header.Add("Hours Worked");
                header.Add("Minutes Late");
                header.Add("Minutes Undertime");
                payRates.ForEach(pr =>
                {
                    header.Add(pr.Code);
                });

                lines.Add(header);

                foreach (var employee in employees)
                {
                    var line = new List<string>
                    {
                        employee.EmployeeCode,
                        employee.LastName,
                        employee.FirstName,
                        String.Empty,
                        String.Empty,
                        String.Empty,
                        String.Empty
                    };

                    payRates.ForEach(pr =>
                    {
                        line.Add(String.Empty);
                    });

                    lines.Add(line);
                }

                return lines;
            }
        }
    }
}
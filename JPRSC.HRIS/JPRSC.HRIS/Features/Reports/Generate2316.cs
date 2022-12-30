using FluentValidation;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Infrastructure.NET;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.Infrastructure.CSV;
using JPRSC.HRIS.Infrastructure.Excel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Drawing;
using static JPRSC.HRIS.Features.Reports.GenerateAlphalist.QueryResult;

namespace JPRSC.HRIS.Features.Reports
{
    public class Generate2316
    {
        public class Query : IRequest<QueryResult>
        {
            public string AlphalistType { get; set; }
            public int? ClientId { get; set; }
            public DateTime DateGenerated { get; set; }
            public string Destination { get; set; }
            public string DisplayMode { get; set; }
            public int PayrollPeriodFromYear { get; set; }
            public int PayrollPeriodToYear { get; set; }
            public Month? FromPayrollPeriodMonth { get; set; }
            public int? FromPayrollPeriod { get; set; }
            public Month? ToPayrollPeriodMonth { get; set; }
            public int? ToPayrollPeriod { get; set; }
            public int ThirteenthMonthPayrollPeriodFromYear { get; set; }
            public int ThirteenthMonthPayrollPeriodToYear { get; set; }
            public Month? ThirteenthMonthFromPayrollPeriodMonth { get; set; }
            public int? ThirteenthMonthFromPayrollPeriod { get; set; }
            public Month? ThirteenthMonthToPayrollPeriodMonth { get; set; }
            public int? ThirteenthMonthToPayrollPeriod { get; set; }
        }

        public class QueryResult
        {
            public string AlphalistType { get; set; }
            public int? ClientId { get; set; }
            public string ClientName { get; set; }
            public int PayrollPeriodFromYear { get; set; }
            public int PayrollPeriodToYear { get; set; }
            public Month? FromPayrollPeriodMonth { get; set; }
            public int? FromPayrollPeriod { get; set; }
            public Month? ToPayrollPeriodMonth { get; set; }
            public int? ToPayrollPeriod { get; set; }
            public int ThirteenthMonthPayrollPeriodFromYear { get; set; }
            public int ThirteenthMonthPayrollPeriodToYear { get; set; }
            public Month? ThirteenthMonthFromPayrollPeriodMonth { get; set; }
            public int? ThirteenthMonthFromPayrollPeriod { get; set; }
            public Month? ThirteenthMonthToPayrollPeriodMonth { get; set; }
            public int? ThirteenthMonthToPayrollPeriod { get; set; }
            public string DisplayMode { get; set; }
            public byte[] FileContent { get; set; }
            public string Filename { get; set; }
            public IList<AlphalistRecord> AlphalistRecords { get; set; } = new List<AlphalistRecord>();
            public Query Query { get; set; }
            public string FilesPath { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, QueryResult>
        {
            private readonly ApplicationDbContext _db;
            private readonly IMediator _mediator;

            public QueryHandler(ApplicationDbContext db, IMediator mediator)
            {
                _db = db;
                _mediator = mediator;
            }

            public async Task<QueryResult> Handle(Query query, CancellationToken token)
            {
                var generateAlphalistQuery = new GenerateAlphalist.Query
                {
                    AlphalistType = query.AlphalistType,
                    ClientId = query.ClientId,
                    DateGenerated = query.DateGenerated,
                    Destination = query.Destination,
                    DisplayMode = query.DisplayMode,
                    PayrollPeriodFromYear = query.PayrollPeriodFromYear,
                    PayrollPeriodToYear = query.PayrollPeriodToYear,
                    FromPayrollPeriodMonth = query.FromPayrollPeriodMonth,
                    FromPayrollPeriod = query.FromPayrollPeriod,
                    ToPayrollPeriodMonth = query.ToPayrollPeriodMonth,
                    ToPayrollPeriod = query.ToPayrollPeriod,
                    ThirteenthMonthPayrollPeriodFromYear = query.ThirteenthMonthPayrollPeriodFromYear,
                    ThirteenthMonthPayrollPeriodToYear = query.ThirteenthMonthPayrollPeriodToYear,
                    ThirteenthMonthFromPayrollPeriodMonth = query.ThirteenthMonthFromPayrollPeriodMonth,
                    ThirteenthMonthFromPayrollPeriod = query.ThirteenthMonthFromPayrollPeriod,
                    ThirteenthMonthToPayrollPeriodMonth = query.ThirteenthMonthToPayrollPeriodMonth,
                    ThirteenthMonthToPayrollPeriod = query.ThirteenthMonthToPayrollPeriod
                };
                var generateAlphalistResult = await _mediator.Send(generateAlphalistQuery);

                var generate2316Result = new Generate2316.QueryResult
                {
                    AlphalistType = generateAlphalistResult.AlphalistType,
                    ClientId = generateAlphalistResult.ClientId,
                    ClientName = generateAlphalistResult.ClientName,
                    PayrollPeriodFromYear = generateAlphalistResult.PayrollPeriodFromYear,
                    PayrollPeriodToYear = generateAlphalistResult.PayrollPeriodToYear,
                    FromPayrollPeriodMonth = generateAlphalistResult.FromPayrollPeriodMonth,
                    FromPayrollPeriod = generateAlphalistResult.FromPayrollPeriod,
                    ToPayrollPeriodMonth = generateAlphalistResult.ToPayrollPeriodMonth,
                    ToPayrollPeriod = generateAlphalistResult.ToPayrollPeriod,
                    ThirteenthMonthPayrollPeriodFromYear = generateAlphalistResult.ThirteenthMonthPayrollPeriodFromYear,
                    ThirteenthMonthPayrollPeriodToYear = generateAlphalistResult.ThirteenthMonthPayrollPeriodToYear,
                    ThirteenthMonthFromPayrollPeriodMonth = generateAlphalistResult.ThirteenthMonthFromPayrollPeriodMonth,
                    ThirteenthMonthFromPayrollPeriod = generateAlphalistResult.ThirteenthMonthFromPayrollPeriod,
                    ThirteenthMonthToPayrollPeriodMonth = generateAlphalistResult.ThirteenthMonthToPayrollPeriodMonth,
                    ThirteenthMonthToPayrollPeriod = generateAlphalistResult.ThirteenthMonthToPayrollPeriod,
                    DisplayMode = generateAlphalistResult.DisplayMode,
                    FileContent = generateAlphalistResult.FileContent,
                    Filename = generateAlphalistResult.Filename,
                    AlphalistRecords = generateAlphalistResult.AlphalistRecords,
                    Query = query
                };

                var template2316 = (Bitmap)Image.FromFile(HttpContext.Current.Server.MapPath(AppSettings.String("2316TemplatePath")));






                //string firstText = "Hello";
                //string secondText = "World";

                //PointF firstLocation = new PointF(10f, 10f);
                //PointF secondLocation = new PointF(10f, 50f);

                ////string imageFilePath = @"path\picture.bmp"
                ////Bitmap bitmap = (Bitmap)Image.FromFile(imageFilePath);//load the image file

                //using (Graphics graphics = Graphics.FromImage(template2316))
                //{
                //    using (Font arialFont = new Font("Arial", 10))
                //    {
                //        graphics.DrawString(firstText, arialFont, Brushes.Blue, firstLocation);
                //        graphics.DrawString(secondText, arialFont, Brushes.Red, secondLocation);
                //    }
                //}

                //template2316.Save(HttpContext.Current.Server.MapPath(AppSettings.String("2316FilesPath")));//save the image file 

                generate2316Result.FilesPath = HttpContext.Current.Server.MapPath(AppSettings.String("2316FilesPath"));

                return generate2316Result;
            }
        }
    }
}

using iTextSharp.text;
using iTextSharp.text.pdf;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Infrastructure.Mvc;
using JPRSC.HRIS.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
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

                var template2316FilePath = HttpContext.Current.Server.MapPath(AppSettings.String("2316TemplatePath"));
                var fileDirectory = HttpContext.Current.Server.MapPath(AppSettings.String("2316FilesPath"));
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }

                var filePath = fileDirectory + "test.pdf";

                Bitmap filledOutImage;
                using (var template2316 = (Bitmap)System.Drawing.Image.FromFile(template2316FilePath))
                {
                    using (var graphics = Graphics.FromImage(template2316))
                    {
                        using (var courierNew = new System.Drawing.Font("Courier New", 18, FontStyle.Bold))
                        {
                            graphics.DrawSpacedString("2022", courierNew, Brushes.Black, new PointF(360f, 270f), 48f);
                            graphics.DrawString("Hello World".ToUpperInvariant(), courierNew, Brushes.Black, new PointF(135f, 420f));
                        }
                    }

                    filledOutImage = new Bitmap(template2316);
                }

                Document document = new Document();
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    PdfWriter.GetInstance(document, stream);
                    document.Open();
                    using (var ms = new MemoryStream())
                    {
                        filledOutImage.Save(ms, ImageFormat.Jpeg);
                        ms.Position = 0;
                        var image = iTextSharp.text.Image.GetInstance(ms);

                        image.SetAbsolutePosition(0, 0);
                        image.ScaleAbsolute(iTextSharp.text.PageSize.A4.Width, iTextSharp.text.PageSize.A4.Height);

                        document.Add(image);
                    }
                    document.Close();
                }

                filledOutImage.Dispose();

                generate2316Result.FilesPath = filePath;
                return generate2316Result;
            }
        }
    }

    internal static class GraphicsExtensions
    {
        internal static void DrawSpacedString(this Graphics g, string s, System.Drawing.Font font, Brush brush, PointF point, float spaces)
        {
            for (var i = 0; i < s.Length; i++)
            {
                g.DrawString(s[i].ToString(), font, brush, new PointF(point.X + i * spaces, point.Y));
            }
        }
    }
}

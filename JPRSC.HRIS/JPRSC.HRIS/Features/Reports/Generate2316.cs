using iTextSharp.text;
using iTextSharp.text.pdf;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
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
            public List<string> Errors { get; set; } = new List<string>();
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

                var generate2316Result = new QueryResult
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

                // http://localhost:52654/Reports/Generate2316?alphalistType=7.1&clientId=8&payrollPeriodFromYear=2022&payrollPeriodToYear=2022&fromPayrollPeriod=1&fromPayrollPeriodMonth=10&toPayrollPeriod=1&toPayrollPeriodMonth=120&thirteenthMonthPayrollPeriodFromYear=2022&thirteenthMonthPayrollPeriodToYear=2022&thirteenthMonthFromPayrollPeriod=1&thirteenthMonthFromPayrollPeriodMonth=10&thirteenthMonthToPayrollPeriod=1&thirteenthMonthToPayrollPeriodMonth=120&destination=page

                var template2316FilePath = HttpContext.Current.Server.MapPath(AppSettings.String("2316TemplatePath"));
                var saveFolder = GetSaveFolder();

                foreach (var record in generate2316Result.AlphalistRecords)
                {
                    var success = Generate2316File(generate2316Result, record, template2316FilePath, saveFolder, out string error);
                    if (!success)
                    {
                        generate2316Result.Errors.Add(error);
                        break;
                    }
                }

                generate2316Result.FilesPath = saveFolder;
                return generate2316Result;
            }

            internal static bool Generate2316File(QueryResult queryResult, AlphalistRecord record, string template2316FilePath, string saveFolder, out string error)
            {
                var fullName = record.Employee.FullName;
                var filePath = Path.Combine(saveFolder, fullName + ".pdf");
                error = null;

                Bitmap filledOutImage = null;
                try
                {
                    using (var template2316 = (Bitmap)System.Drawing.Image.FromFile(template2316FilePath))
                    {
                        using (var graphics = Graphics.FromImage(template2316))
                        {
                            WriteInfo(graphics, queryResult, record);
                        }

                        filledOutImage = new Bitmap(template2316);
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    if (filledOutImage != null) filledOutImage.Dispose();
                    return false;
                }

                Document document = new Document();
                try
                {
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
                            image.ScaleAbsolute(PageSize.A4.Width, PageSize.A4.Height);

                            document.Add(image);
                        }
                        document.Close();
                    }

                    filledOutImage.Dispose();
                    document.Dispose();
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    return false;
                }
                finally
                {
                    if (filledOutImage != null) filledOutImage.Dispose();
                    if (document != null) document.Dispose();
                }

                return true;
            }

            private static string GetSaveFolder()
            {
                var fileDirectory = HttpContext.Current.Server.MapPath(AppSettings.String("2316FilesPath"));
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }
                var subFolder = DateTime.Now.ToString("yyyyMMddHHmmss");
                var fileDirectoryWithSubFolder = Path.Combine(fileDirectory, subFolder);
                if (!Directory.Exists(fileDirectoryWithSubFolder))
                {
                    Directory.CreateDirectory(fileDirectoryWithSubFolder);
                }
                else
                {
                    DeleteAllFiles(fileDirectoryWithSubFolder);
                }

                return fileDirectoryWithSubFolder;
            }

            private static void DeleteAllFiles(string path)
            {
                var di = new DirectoryInfo(path);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }

            private static void WriteInfo(Graphics graphics, QueryResult queryResult, AlphalistRecord record)
            {
                using (var courierNew = new System.Drawing.Font("Courier New", 18, FontStyle.Bold))
                {
                    // For the year
                    if (queryResult.PayrollPeriodToYear != default(int)) graphics.Write(courierNew, queryResult.PayrollPeriodToYear, new PointF(360f, 270f), 48f);

                    // TIN
                    var tin = new TIN(record.Employee.TIN);
                    if (!String.IsNullOrWhiteSpace(tin.FirstPart)) graphics.Write(courierNew, tin.FirstPart, new PointF(247f, 353f), 34f);
                    if (!String.IsNullOrWhiteSpace(tin.SecondPart)) graphics.Write(courierNew, tin.SecondPart, new PointF(386f, 353f), 34f);
                    if (!String.IsNullOrWhiteSpace(tin.ThirdPart)) graphics.Write(courierNew, tin.ThirdPart, new PointF(523f, 353f), 34f);

                    // Employee's name
                    graphics.Write(courierNew, record.Employee.FullName.ToUpperInvariant(), new PointF(135f, 420f));
                }
            }
        }
    }

    internal class TIN
    {
        public TIN(string tin)
        {
            if (!String.IsNullOrWhiteSpace(tin))
            {
                FirstPart = tin.Substring(0, tin.IndexOf("-"));
                SecondPart = tin.Substring(tin.IndexOf("-") + 1, 3);
                ThirdPart = tin.Substring(tin.LastIndexOf("-") + 1);
            }
        }

        internal string FirstPart { get; }
        internal string SecondPart { get; }
        internal string ThirdPart { get; }
    }

    internal static class GraphicsExtensions
    {
        internal static void Write(this Graphics g, System.Drawing.Font font, string s, PointF point, float spaces)
        {
            for (var i = 0; i < s.Length; i++)
            {
                g.DrawString(s[i].ToString(), font, Brushes.Black, new PointF(point.X + i * spaces, point.Y));
            }
        }

        internal static void Write(this Graphics g, System.Drawing.Font font, int i, PointF point, float spaces)
        {
            g.Write(font, i.ToString(), point, spaces);
        }

        internal static void Write(this Graphics g, System.Drawing.Font font, string s, PointF point)
        {
            g.DrawString(s, font, Brushes.Black, point);
        }
    }
}

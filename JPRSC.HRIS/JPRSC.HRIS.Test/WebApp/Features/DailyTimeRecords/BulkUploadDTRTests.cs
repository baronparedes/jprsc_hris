using JPRSC.HRIS.Features.DailyTimeRecords;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Mvc;
using System;
using System.IO;
using System.Threading;
using Xunit;

namespace JPRSC.HRIS.Test.WebApp.Features.DailyTimeRecords
{
    public class BulkUploadDTRTests
    {
        [Fact(Skip = "Relies on hardcoded path")]
        public void BulkUploadDTR()
        {
            // Arrange
            var fileName = "DTR 06_04_2020 SERMASISON CORPORATION  .csv";
            var clientId = 87;
            var payrollPeriodFrom = new DateTime(2020, 5, 16);
            var payrollPeriodTo = new DateTime(2020, 5, 31);
            var payrollPeriodMonth = Month.June;

            // Act
            var filePath = Path.GetFullPath($"C:\\Projects\\Work\\jprsc\\{fileName}");
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var fileImage = new MemoryFile(fileStream, "text/csv", fileName);

                var command = new BulkUploadDTR.Command
                {
                    File = fileImage,
                    ClientId = clientId,
                    PayrollPeriodFrom = payrollPeriodFrom,
                    PayrollPeriodTo = payrollPeriodTo,
                    PayrollPeriodMonth = payrollPeriodMonth
                };

                using (var db = new ApplicationDbContext())
                {
                    var handler = new BulkUploadDTR.CommandHandler(db);
                    handler.Handle(command, CancellationToken.None);
                }
            }
        }
    }
}
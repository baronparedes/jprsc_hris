using JPRSC.HRIS.Features.Reports;
using System;
using System.IO;
using Xunit;

namespace JPRSC.HRIS.Test.Features.Reports
{
    public class Generate2316Test
    {
        [Fact]
        public void Generate2316File_ShouldGenerateFile_01()
        {
            var queryResult = new Generate2316.QueryResult
            {
                PayrollPeriodToYear = 2022
            };
            var record = AlphalistRecordSample.AbucotMichelle;
            var template2316filePath = Path.Combine(Config.SolutionFolder, "JPRSC.HRIS.WebApp\\wwwroot\\images\\2316-template.jpg");
            var saveFolder = GetSaveFolder("2316_01");

            Generate2316.QueryHandler.Generate2316File(queryResult, record, template2316filePath, saveFolder);
        }

        private static string GetSaveFolder(string test)
        {
            var saveFolder = Path.Combine(Config.SolutionFolder, "JPRSC.HRIS.Test\\bin\\FileOutput", test, DateTime.Now.ToString("yyyyMMddHHmmss"));
            Directory.CreateDirectory(saveFolder);

            return saveFolder;
        }
    }
}

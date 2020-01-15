using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace JPRSC.HRIS.WebApp.Infrastructure.CSV
{
    public interface ICSVBuilder
    {
        byte[] BuildCSVFile(IEnumerable<IEnumerable<string>> lines);
    }

    public class CSVBuilder : ICSVBuilder
    {
        public byte[] BuildCSVFile(IEnumerable<IEnumerable<string>> lines)
        {
            var builder = new StringBuilder(lines.Count() * 100);

            foreach (var line in lines)
            {
                builder.AppendLine(line.Join(","));
            }

            var bytes = new UTF8Encoding().GetBytes(builder.ToString());

            return bytes;
        }
    }
}
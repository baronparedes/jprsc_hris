using System;
using System.Collections.Generic;
using System.Text;

namespace JPRSC.HRIS.Infrastructure.Excel
{
    public class ExcelObject
    {
        public IList<string> Header { get; set; }
        public IList<IList<string>> Rows { get; set; }

        public string ToOneBigString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(String.Join(",", Header));

            foreach (var row in Rows)
            {
                sb.AppendLine(String.Join(",", row));
            }

            return sb.ToString();
        }

        public IEnumerable<IEnumerable<string>> ToLines()
        {
            var lines = new List<IEnumerable<string>>();

            lines.Add(Header);
            lines.AddRange(Rows);

            return lines;
        }
    }
}
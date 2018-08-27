using System;
using System.Collections.Generic;
using System.Text;

namespace JPRSC.HRIS.WebApp.Infrastructure.Excel
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace JPRSC.HRIS.Infrastructure.Excel
{
    public static class IEnumerableExtensions
    {
        public static ExcelObject ToExcelObject<TRow>(this IEnumerable<TRow> items, IEnumerable<ColumnInfo<TRow>> columns)
        {
            var type = typeof(TRow);
            IList<string> header = new List<string>(columns.Select(c => c.DisplayName));
            IList<IList<string>> rows = new List<IList<string>>(items.Count());

            var fieldNames = columns.Where(c => !String.IsNullOrWhiteSpace(c.Field)).Select(c => c.Field).ToList();

            foreach (var item in items)
            {
                IList<string> row = new List<string>(columns.Count());

                foreach (var fieldName in fieldNames)
                {
                    var column = columns.Single(c => c.Field == fieldName);
                    row.Add(column.ValueFactory(item));
                }

                rows.Add(row);
            }

            return new ExcelObject
            {
                Header = header,
                Rows = rows
            };
        }
    }
}
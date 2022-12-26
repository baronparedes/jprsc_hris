using System;
using System.Linq;

namespace JPRSC.HRIS.Infrastructure.Excel
{
    public class ColumnInfo<TRow>
    {
        public ColumnInfo()
        {
            ValueFactory = GetDefaultValueFactory();
        }

        public ColumnInfo(string displayName, string field) : this()
        {
            DisplayName = displayName;
            Field = field;
        }

        public ColumnInfo(string displayName, string field, Func<TRow, string> valueFactory)
        {
            DisplayName = displayName;
            Field = field;
            ValueFactory = valueFactory;
        }

        public string DisplayName { get; set; }
        public string Field { get; set; }
        public Func<TRow, string> ValueFactory { get; set; }

        private Func<TRow, string> GetDefaultValueFactory() =>
            obj =>
            {
                var properties = typeof(TRow).GetProperties();

                var property = properties.SingleOrDefault(p => p.Name == Field);
                if (property == null) return null;

                return Convert.ToString(property.GetValue(obj));
            };
    }
}
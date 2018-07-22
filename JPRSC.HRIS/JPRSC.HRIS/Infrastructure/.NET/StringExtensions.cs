using System.Collections.Generic;

namespace System
{
    public static class StringExtensions
    {
        public static string Join(this IEnumerable<string> values, string separator)
        {
            return String.Join(separator, values);
        }

        public static object AsSqlParameterValue(this string s)
        {
            if (String.IsNullOrWhiteSpace(s))
            {
                return DBNull.Value;
            }
            else
            {
                return s;
            }
        }

        public static decimal? ToNullableDecimal(this string value)
        {
            if (String.IsNullOrWhiteSpace(value)) return null;

            return decimal.Parse(value);
        }

        public static double? ToNullableDouble(this string value)
        {
            if (String.IsNullOrWhiteSpace(value)) return null;

            return double.Parse(value);
        }
    }
}
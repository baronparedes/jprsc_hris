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
    }
}
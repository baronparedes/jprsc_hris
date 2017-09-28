using System.Collections.Generic;

namespace System
{
    public static class StringExtensions
    {
        public static string Join(this IEnumerable<string> values, string separator)
        {
            return String.Join(separator, values);
        }
    }
}
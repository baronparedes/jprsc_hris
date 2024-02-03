using System.Linq;

namespace System.Collections.Generic
{
    public static class IListExtensions
    {
        public static IList<string> RemoveTrailingWhitespace(this IList<string> values)
        {
            if (values == null || values.Count == 0) return values;

            var firstNonWhitespaceIndex = 0;

            for (var i = values.Count - 1; i > 0; i--)
            {
                if (!string.IsNullOrWhiteSpace((string)values[(int)i]))
                {
                    firstNonWhitespaceIndex = i;
                    break;
                }
            }

            if (firstNonWhitespaceIndex == values.Count - 1)
            {
                return values;
            }
            else
            {
                var returnValue = new List<string>();
                for (var i = 0; i <= firstNonWhitespaceIndex; i++)
                {
                    returnValue.Add(values[i]);
                }

                return returnValue;
            }
        }
    }
}

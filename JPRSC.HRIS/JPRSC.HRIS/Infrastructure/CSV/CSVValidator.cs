using System.Collections.Generic;
using System.Linq;

namespace JPRSC.HRIS.Infrastructure.CSV
{
    internal static class CSVValidator
    {
        private static List<char> validChars = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz`~!@#$%^&*()-_=+[{]};:'\",<.>/? ñÑ".ToList();

        internal static bool IsValidCSVLine(string line)
        {
            return line.All(c => validChars.Contains(c));
        }
    }
}

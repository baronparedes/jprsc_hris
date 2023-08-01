using System.Collections.Generic;
using System.Linq;

namespace JPRSC.HRIS.Infrastructure.CSV
{
    internal static class CSVValidator
    {
        private static string enye = "�";
        private static List<char> validChars = $"1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz`~!@#$%^&*()-_=+[{{]}};:'\",<.>/? ñÑ{enye}".ToList();

        internal static bool IsValidCSVLine(string line)
        {
            return line.All(c => validChars.Contains(c));
        }
    }
}

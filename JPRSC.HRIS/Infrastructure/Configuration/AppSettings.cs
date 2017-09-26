using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace JPRSC.HRIS.Infrastructure.Configuration
{
    public static class AppSettings
    {
        private const char _defaultSeparator = ',';

        public static bool Bool(string key)
        {
            if (System.String.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            return Convert.ToBoolean(String(key));
        }

        public static double Double(string key)
        {
            if (System.String.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            return Convert.ToDouble(String(key));
        }

        public static int Int(string key)
        {
            if (System.String.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            return Convert.ToInt32(String(key));
        }

        public static IEnumerable<int> Ints(string key)
        {
            if (System.String.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            return Ints(key, _defaultSeparator);
        }

        public static IEnumerable<int> Ints(string key, char separator)
        {
            if (System.String.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
            if (separator == default(char)) throw new ArgumentNullException(nameof(separator));

            return String(key).Split(separator).ToList().ConvertAll(Convert.ToInt32);
        }

        public static long Long(string key)
        {
            if (System.String.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            return Convert.ToInt64(String(key));
        }

        public static string String(string key)
        {
            if (System.String.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            return ConfigurationManager.AppSettings[key];
        }

        public static IEnumerable<string> Strings(string key)
        {
            if (System.String.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            return Strings(key, _defaultSeparator);
        }

        public static IEnumerable<string> Strings(string key, char separator)
        {
            if (System.String.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
            if (separator == default(char)) throw new ArgumentNullException(nameof(separator));

            return String(key).Split(separator).ToList();
        }
    }
}
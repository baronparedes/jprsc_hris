using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace JPRSC.HRIS.Infrastructure.NET
{
    public static class EnumHelper
    {
        public static object ConvertToLookup<TEnum>(TEnum enumValue, bool camelCase = true) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var displayName = GetDisplayName(enumValue);

            if (camelCase)
            {
                return new { value = Convert.ToInt32(enumValue), text = displayName ?? enumValue.ToString() };
            }

            return new { Value = Convert.ToInt32(enumValue), Text = displayName ?? enumValue.ToString() };
        }

        public static string GetDisplayName<TEnum>(TEnum enumValue) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return enumValue.GetType().GetMember(enumValue.ToString()).First().GetCustomAttribute<DisplayAttribute>()?.Name;
        }
    }
}
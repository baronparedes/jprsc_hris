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
            var displayAttributeName = enumValue.GetType().GetMember(enumValue.ToString()).First().GetCustomAttribute<DisplayAttribute>()?.Name;

            if (camelCase)
            {
                return new { value = Convert.ToInt32(enumValue), text = displayAttributeName ?? enumValue.ToString() };
            }

            return new { Value = Convert.ToInt32(enumValue), Text = displayAttributeName ?? enumValue.ToString() };
        }
    }
}
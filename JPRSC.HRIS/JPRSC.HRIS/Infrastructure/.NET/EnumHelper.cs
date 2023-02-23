using System;
using System.Collections.Generic;
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
            var memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
            if (memberInfo == null)
            {
                return enumValue.ToString();
            }

            return memberInfo.GetCustomAttribute<DisplayAttribute>()?.Name;
        }

        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
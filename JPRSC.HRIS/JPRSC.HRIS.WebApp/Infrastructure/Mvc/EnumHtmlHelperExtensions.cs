using JPRSC.HRIS.Infrastructure.NET;
using System;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Infrastructure.Mvc
{
    public static class EnumHtmlHelperExtensions
    {
        public static IHtmlString EnumLookup<TEnum>(this HtmlHelper helper, TEnum enumValue) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return helper.Raw(Json.Encode(EnumHelper.ConvertToLookup(enumValue)));
        }
    }
}
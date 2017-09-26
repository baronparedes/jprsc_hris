using JPRSC.HRIS.WebApp.Infrastructure.Logging;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Infrastructure.Mvc
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
            filters.Add(new ExceptionLoggerAttribute());
        }
    }
}
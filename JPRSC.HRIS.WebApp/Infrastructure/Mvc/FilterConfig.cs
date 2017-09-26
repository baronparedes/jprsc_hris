using JPRSC.HRIS.WebApp.Infrastructure.Logging;
using JPRSC.HRIS.WebApp.Infrastructure.Sidebar;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Infrastructure.Mvc
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new SidebarAttribute());
            filters.Add(new AuthorizeAttribute());
            filters.Add(new ExceptionLoggerAttribute());
        }
    }
}
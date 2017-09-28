using JPRSC.HRIS.WebApp.Infrastructure.Bundling;
using JPRSC.HRIS.WebApp.Infrastructure.Mapping;
using JPRSC.HRIS.WebApp.Infrastructure.Mvc;
using JPRSC.HRIS.WebApp.Infrastructure.Validation;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace JPRSC.HRIS.WebApp
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            ValidationConfig.Configure();
            MappingConfig.Configure();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
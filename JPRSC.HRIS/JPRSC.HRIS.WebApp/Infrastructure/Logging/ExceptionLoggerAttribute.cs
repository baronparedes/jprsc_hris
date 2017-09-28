using JPRSC.HRIS.WebApp.Infrastructure.Dependency;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Infrastructure.Logging
{
    public class ExceptionLoggerAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var logger = DependencyConfig.Instance.Container.GetInstance<IMVCLogger>();
            logger.Log(filterContext);
        }
    }
}
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Infrastructure.Logging
{
    public interface IMVCLogger
    {
        void Log(ExceptionContext filterContext);
    }
}
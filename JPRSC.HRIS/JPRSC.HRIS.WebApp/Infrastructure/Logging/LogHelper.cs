using JPRSC.HRIS.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Web;

namespace JPRSC.HRIS.WebApp.Infrastructure.Logging
{
    public class LogHelper
    {
        public static LogEntry CreateLogEntryFromCurrentContext()
        {
            return CreateLogEntryFromCurrentContext(null, null);
        }

        public static LogEntry CreateLogEntryFromCurrentContext(string level, string message)
        {
            return new LogEntry
            {
                Action = Convert.ToString(HttpContext.Current.Request.RequestContext.RouteData.Values["action"]),
                Controller = Convert.ToString(HttpContext.Current.Request.RequestContext.RouteData.Values["controller"]),
                Level = level,
                LoggedOn = DateTime.UtcNow,
                Message = message,
                Request = GetRequest(HttpContext.Current),
                UserId = HttpContext.Current.User.Identity.GetUserId()
            };
        }

        private static string GetRequest(HttpContext httpContext)
        {
            var headers = httpContext.Request.ServerVariables["ALL_RAW"].Replace("\r\n", Environment.NewLine);
            var form = httpContext.Request.Form.ToString();

            return headers + Environment.NewLine + form;
        }
    }
}
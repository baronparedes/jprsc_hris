using Microsoft.AspNet.Identity;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using System;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Infrastructure.Logging
{
    public class MVCLogger : IMVCLogger
    {
        public void Log(ExceptionContext filterContext)
        {
            var logEntry = new LogEntry
            {
                Action = Convert.ToString(filterContext.RouteData.Values["action"]),
                Controller = Convert.ToString(filterContext.RouteData.Values["controller"]),
                LoggedOn = DateTime.UtcNow,
                Message = filterContext.Exception.Message,
                StackTrace = filterContext.Exception.StackTrace,
                UserId = filterContext.HttpContext.User.Identity.GetUserId()
            };

            using (var db = new ApplicationDbContext())
            {
                db.LogEntries.Add(logEntry);
                db.SaveChanges();
            }
        }
    }
}
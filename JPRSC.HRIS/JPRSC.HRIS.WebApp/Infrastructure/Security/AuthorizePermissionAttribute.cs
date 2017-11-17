using Microsoft.AspNet.Identity;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Dependency;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JPRSC.HRIS.Infrastructure.Logging;
using System.Net;
using JPRSC.HRIS.WebApp.Infrastructure.Logging;

namespace JPRSC.HRIS.WebApp.Infrastructure.Security
{
    public class AuthorizePermissionAttribute : AuthorizeAttribute
    {
        public AuthorizePermissionAttribute(Permission permission)
        {
            Permission = permission;
        }

        public Permission Permission { get; private set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized) return false;

            var db = DependencyConfig.Instance.Container.GetInstance<ApplicationDbContext>();

            var currentUserId = httpContext.User.Identity.GetUserId();

            var currentUser = db.Users
                .Include(u => u.CustomRoles)
                .SingleOrDefault(u => u.Id == currentUserId);

            var authorizedViaPermission = currentUser
                .CustomRoles
                .Any(cr => cr.HasPermission(Permission));

            if (!authorizedViaPermission)
            {
                var logEntry = LogHelper.CreateLogEntryFromCurrentContext(LogLevel.Warn, "Unauthorized Request");
                Logger.Log(logEntry);
            }

            return authorizedViaPermission;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            filterContext.HttpContext.Response.End();
            filterContext.HttpContext.Response.Close();
        }
    }
}
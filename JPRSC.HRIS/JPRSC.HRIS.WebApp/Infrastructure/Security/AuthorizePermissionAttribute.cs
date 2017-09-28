using Microsoft.AspNet.Identity;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Dependency;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Infrastructure.Security
{
    public class AuthorizePermissionAttribute : AuthorizeAttribute
    {
        private readonly ApplicationDbContext _db;

        public AuthorizePermissionAttribute()
        {
            _db = DependencyConfig.Instance.Container.GetInstance<ApplicationDbContext>();
        }

        public Permission Permission { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized) return false;

            var currentUserId = httpContext.User.Identity.GetUserId();

            var currentUser = _db.Users
                .Include(u => u.CustomRoles)
                .SingleOrDefault(u => u.Id == currentUserId);

            return currentUser
                .CustomRoles
                .SelectMany(r => r.Permissions)
                .Distinct()
                .Any(p => p == Permission);
        }
    }
}
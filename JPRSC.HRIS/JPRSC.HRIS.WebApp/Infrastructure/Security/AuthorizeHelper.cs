using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Dependency;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace JPRSC.HRIS.WebApp.Infrastructure.Security
{
    public static class AuthorizeHelper
    {
        private static int _superAdminCustomRoleId = 1;

        public static bool IsAuthorized(Permission permission)
        {
            var db = DependencyConfig.Instance.Container.GetInstance<ApplicationDbContext>();

            var currentUserId = HttpContext.Current.User.Identity.GetUserId();

            var currentUser = db.Users
                .Include(u => u.CustomRoles)
                .SingleOrDefault(u => u.Id == currentUserId);

            var authorizedViaPermission = currentUser
                .CustomRoles
                .Any(cr => cr.HasPermission(permission));

            return authorizedViaPermission;
        }

        public static bool IsSuperAdmin()
        {
            var db = DependencyConfig.Instance.Container.GetInstance<ApplicationDbContext>();

            var currentUserId = HttpContext.Current.User.Identity.GetUserId();

            var currentUser = db.Users
                .Include(u => u.CustomRoles)
                .SingleOrDefault(u => u.Id == currentUserId);

            return currentUser.CustomRoles.Any(cr => cr.Id == _superAdminCustomRoleId);
        }
    }
}
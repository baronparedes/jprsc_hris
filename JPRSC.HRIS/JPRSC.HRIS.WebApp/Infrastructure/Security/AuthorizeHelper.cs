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
        public static bool IsAuthorized(Permission permission)
        {
            var db = DependencyConfig.Instance.Container.GetInstance<ApplicationDbContext>();

            return JPRSC.HRIS.Infrastructure.Security.AuthorizeHelper.IsAuthorized(db, permission);
        }

        public static bool IsSuperAdmin()
        {
            var db = DependencyConfig.Instance.Container.GetInstance<ApplicationDbContext>();

            return JPRSC.HRIS.Infrastructure.Security.AuthorizeHelper.IsSuperAdmin(db);
        }
    }
}
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Collections.Generic;

namespace JPRSC.HRIS.WebApp.Features.Manage
{
    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; } = new List<UserLoginInfo>();
        public IList<AuthenticationDescription> OtherLogins { get; set; } = new List<AuthenticationDescription>();
    }
}
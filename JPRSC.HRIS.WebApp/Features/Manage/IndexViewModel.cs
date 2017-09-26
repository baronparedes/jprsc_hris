using Microsoft.AspNet.Identity;
using System.Collections.Generic;

namespace JPRSC.HRIS.WebApp.Features.Manage
{
    public class IndexViewModel
    {
        public bool BrowserRemembered { get; set; }
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; } = new List<UserLoginInfo>();
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
    }
}
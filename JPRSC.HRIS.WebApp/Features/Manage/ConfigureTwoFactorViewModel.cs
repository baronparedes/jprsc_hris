using System.Collections.Generic;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Features.Manage
{
    public class ConfigureTwoFactorViewModel
    {
        public ICollection<SelectListItem> Providers { get; set; } = new List<SelectListItem>();
        public string SelectedProvider { get; set; }
    }
}
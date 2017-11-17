using JPRSC.HRIS.Models;
using System.Collections.Generic;
using System.Linq;

namespace JPRSC.HRIS.WebApp.Infrastructure.Sidebar
{
    public class SidebarMenuItem
    {
        public SidebarMenuItem(string linkText, string actionName, string controllerName, string iconClass, Permission permission, params SidebarMenuItem[] subMenus)
        {
            Action = actionName;
            Controller = controllerName;
            IconClass = iconClass;
            Title = linkText;
            Permission = permission;
            SubMenus = subMenus != null ? subMenus.ToList() : new List<SidebarMenuItem>();
        }

        public string Action { get; private set; }
        public string Controller { get; private set; }
        public string IconClass { get; private set; }
        public string Title { get; private set; }
        public Permission Permission { get; private set; }
        public IList<SidebarMenuItem> SubMenus { get; private set; }
    }
}
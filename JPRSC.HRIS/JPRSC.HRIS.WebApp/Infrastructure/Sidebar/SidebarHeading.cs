using System.Collections.Generic;

namespace JPRSC.HRIS.WebApp.Infrastructure.Sidebar
{
    public class SidebarHeading
    {
        public SidebarHeading(string text, params SidebarMenuItem[] subMenus)
        {
            Text = text;
            SubMenus = subMenus;
        }

        public string Text { get; private set; }
        public IList<SidebarMenuItem> SubMenus { get; private set; }
    }
}
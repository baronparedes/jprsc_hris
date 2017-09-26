using System;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Infrastructure.Sidebar
{
    public static class SidebarHtmlHelperExtensions
    {
        public static bool HasSelectedMenu<T>(this HtmlHelper<T> helper, string actionName, string controllerName)
        {
            if (helper == null) throw new ArgumentNullException(nameof(helper));
            if (actionName == null) throw new ArgumentNullException(nameof(actionName));
            if (controllerName == null) throw new ArgumentNullException(nameof(controllerName));

            var selectedSidebarItem = GetSelectedSidebarItem(helper);

            if (selectedSidebarItem != null)
            {
                var sameAction = String.Equals(actionName, selectedSidebarItem.ActionName, StringComparison.CurrentCultureIgnoreCase);
                var sameController = String.Equals(controllerName, selectedSidebarItem.ControllerName, StringComparison.CurrentCultureIgnoreCase);

                return sameAction && sameController;
            }

            return false;
        }

        public static bool HasSelectedMenu<T>(this HtmlHelper<T> helper, string controllerName)
        {
            if (helper == null) throw new ArgumentNullException(nameof(helper));
            if (controllerName == null) throw new ArgumentNullException(nameof(controllerName));

            var selectedSidebarItem = GetSelectedSidebarItem(helper);

            if (selectedSidebarItem != null)
            {
                var sameController = String.Equals(controllerName, selectedSidebarItem.ControllerName, StringComparison.CurrentCultureIgnoreCase);

                return sameController;
            }

            return false;
        }

        private static SelectedSidebarItem GetSelectedSidebarItem<T>(HtmlHelper<T> helper)
        {
            return helper.ViewContext.Controller.ViewBag.SelectedSidebarItem as SelectedSidebarItem;
        }
    }
}
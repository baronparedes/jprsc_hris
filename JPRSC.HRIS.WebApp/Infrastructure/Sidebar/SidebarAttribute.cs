using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Infrastructure.Sidebar
{
    public class SidebarAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.Controller.ViewBag.SelectedSidebarItem = new SelectedSidebarItem
            {
                ActionName = filterContext.ActionDescriptor.ActionName,
                ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName
            };
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }
    }
}
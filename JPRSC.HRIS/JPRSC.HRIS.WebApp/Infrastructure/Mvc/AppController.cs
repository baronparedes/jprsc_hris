using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Infrastructure.Mvc
{
    public class AppController : Controller
    {
        protected string BaseUrl
        {
            get
            {
                return $"{Request.Url.Scheme}://{Request.Url.Authority}{Request.ApplicationPath.TrimEnd('/')}";
            }
        }

        protected JsonCamelCaseResult JsonCamelCase(object data)
        {
            return new JsonCamelCaseResult(data);
        }

        protected new RedirectToRouteResult RedirectToAction(string actionName, string controllerName)
        {
            var formattedControllerName = controllerName.Contains("Controller") ? controllerName.Substring(0, controllerName.Length - 10) : controllerName;

            return base.RedirectToAction(actionName, formattedControllerName);
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect(Url.RouteUrl("Default"));
        }
    }
}
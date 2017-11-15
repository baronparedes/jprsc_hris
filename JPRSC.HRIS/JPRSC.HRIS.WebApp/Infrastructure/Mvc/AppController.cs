using System.Collections.Generic;
using System.Linq;
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

        protected JsonCamelCaseResult JsonValidationError()
        {
            // An errors dictionary where
            // The keys are property names and
            // The values are errors for those property names
            var errorsDictionary = new Dictionary<string, IEnumerable<string>>();

            foreach (var modelState in ModelState)
            {
                if (modelState.Value.Errors.Any())
                {
                    errorsDictionary.Add(modelState.Key, modelState.Value.Errors.Select(e => e.ErrorMessage));
                }
            }

            Response.StatusCode = 400;
            Response.TrySkipIisCustomErrors = true;

            return JsonCamelCase(errorsDictionary);
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
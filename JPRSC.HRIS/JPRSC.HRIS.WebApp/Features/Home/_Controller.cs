using JPRSC.HRIS.Infrastructure.Mvc;
using JPRSC.HRIS.Models;
using JPRSC.HRIS.WebApp.Infrastructure.Security;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Controllers
{
    [AuthorizePermission(Permission.HomeDefault)]
    public class HomeController : AppController
    {
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
        public ActionResult Index()
        {
            return View();
        }
    }
}
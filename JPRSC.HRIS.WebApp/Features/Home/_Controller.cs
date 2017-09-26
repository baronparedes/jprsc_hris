using JPRSC.HRIS.WebApp.Infrastructure.Mvc;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Controllers
{
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
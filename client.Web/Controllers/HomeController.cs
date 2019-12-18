using System.Web.Mvc;

namespace client.Web.Views.Shared
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult getStarted_Doctor()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult getStarted_Patient()
        {
            return View();
        }
        
        
    }
}

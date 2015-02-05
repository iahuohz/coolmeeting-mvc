using System.Web.Mvc;

namespace CoolMeetingWeb.Controllers
{
    public class DefaultController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
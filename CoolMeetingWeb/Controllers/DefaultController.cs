using System.Data.Entity;
using System.Linq;
using CoolMeetingWeb.Models;
using System.Web.Mvc;

namespace CoolMeetingWeb.Controllers
{
    public class DefaultController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult IndexPost()
        {
            using(CoolMeetingDbContext db = new CoolMeetingDbContext())
            {
                db.Departments.ToList();
            }
            ViewBag.Result = "数据库生成成功！";
            return View();
        }
    }
}
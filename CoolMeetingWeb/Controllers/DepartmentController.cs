using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;

using CoolMeetingWeb.Models;
using CoolMeetingWeb.ViewModels;

namespace CoolMeetingWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private CoolMeetingDbContext db = new CoolMeetingDbContext();

        public async Task<ActionResult> Index()
        {
            return View(await db.Departments.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }

            ViewBag.DepartmentName = department.DepartmentName;
            var employees = db.Employees.Where(e => e.DepartmentID == id && !e.IsDeleted);

            return View(await employees.ToListAsync());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "DepartmentID, DepartmentName")] 
            Department department)
        {
            if (ModelState.IsValid)
            {
                Department existed = await db.Departments.FirstOrDefaultAsync(d => 
                    string.Compare(d.DepartmentName, department.DepartmentName, true) == 0);

                if (existed != null)
                {
                    ModelState.AddModelError("opresult", "部门名称已经存在！");
                }
                else
                {
                    db.Departments.Add(department);
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
            }

            return View(department);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }

            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Include = "DepartmentID,DepartmentName")] 
            Department department)
        {
            if (ModelState.IsValid)
            {
                Department existed = db.Departments.FirstOrDefault(d => 
                    (string.Compare(d.DepartmentName, department.DepartmentName, true) == 0) &&
                    d.DepartmentID != department.DepartmentID);

                if (existed != null)
                {
                    ModelState.AddModelError("opresult", "部门名称已经存在！");
                }
                else
                {
                    db.Entry(department).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
            }
            return View(department);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }

            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Department department = await db.Departments.FindAsync(id);
            if (department != null)
            {
                if (department.Employees.FirstOrDefault() != null)
                {
                    ModelState.AddModelError("opresult", "此部门下有员工信息，无法删除部门！");
                    return View(department);
                }
                else 
                { 
                    db.Departments.Remove(department);
                    await db.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

        public JsonResult EmployeesByDepartment(int departmentID)
        {
            var result = db.Employees.Where(e => e.DepartmentID == departmentID).
                Select(emp => new { employeeid = emp.EmployeeID, employeename = emp.EmployeeName });

            return Json(result.ToArray(), JsonRequestBehavior.AllowGet);
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

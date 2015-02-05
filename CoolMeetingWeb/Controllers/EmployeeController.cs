using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using CoolMeetingWeb.Models;
using CoolMeetingWeb.ViewModels;
using CoolMeetingWeb.Biz;
using System.Collections.Generic;

namespace CoolMeetingWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {
        private CoolMeetingDbContext db = new CoolMeetingDbContext();

        public ActionResult Index(string employeeName, string userName, string email, 
            int? department, int? status, int? pageIndex, string sortFieldName, bool? descending)
        {
            ViewBag.Departments = new SelectList(db.Departments.ToList(), "DepartmentID", "DepartmentName");

            int totalRows;
            int index = pageIndex.HasValue ? (pageIndex.Value - 1) : 0;
            List<Employee> result = EntityFinder.GetEmployeesSortedAndPaged(
                employeeName, userName, email, department, status, index, 
                sortFieldName, descending, out totalRows);

            // 设置分页标签的数据模型
            PagedNavigatorViewModel navigatorModel = new PagedNavigatorViewModel
            {
                TotalRows = totalRows,
                CurrentPageIndex = index + 1
            };
            ViewBag.PagedNavigatorModel = navigatorModel;

            return View(result);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = await db.Employees.FindAsync(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = await db.Employees.FindAsync(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.Departments = new SelectList(db.Departments, "DepartmentID", "DepartmentName", employee.DepartmentID);
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Include = "EmployeeID,EmployeeName,Gender,DepartmentID")] Employee employee,
            string returl)
        {
            // 因为UserName, Email, Password等字段并不在Edit页面中出现，因此在验证模型时，仅检查下列三个字段
            if (ModelState.IsValidField("EmployeeName") && 
                ModelState.IsValidField("Gender") &&
                ModelState.IsValidField("DepartmentID"))            
            {
                // Employee表中实际上不需要Password列，因此employee.Password将为null
                // 但是Password加了Required验证，在SaveChange时会导致验证失败
                // 设置ValidateOnSaveEnabled为false，将禁用验证 
                db.Configuration.ValidateOnSaveEnabled = false;
                var emp = await db.Employees.FindAsync(employee.EmployeeID);
                emp.EmployeeName = employee.EmployeeName;
                emp.Gender = employee.Gender;
                emp.DepartmentID = employee.DepartmentID;
                await db.SaveChangesAsync();

                // 更新图片
                if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    file.SaveAs(System.IO.Path.Combine(Server.MapPath("/Photos"), emp.PhotoUrl));
                }

                if (!string.IsNullOrEmpty(returl))
                {
                    return Redirect(returl);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Departments = new SelectList(db.Departments, "DepartmentID", "DepartmentName", employee.DepartmentID);
            return View(employee);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Employee employee = db.Employees.Find(id);
            if (employee != null)
            {
                // 删除账号
                ApplicationUserManager mgr = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                ApplicationUser user = mgr.FindByName(employee.UserName);
                if (user != null)
                {
                    mgr.Delete(user);
                }
                mgr.Dispose();

                // 删除员工信息
                employee.IsDeleted = true;
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
            }
            return Json(true);
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

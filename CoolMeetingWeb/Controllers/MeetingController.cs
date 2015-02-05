using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CoolMeetingWeb.ViewModels;
using CoolMeetingWeb.Models;
using CoolMeetingWeb.Biz;

namespace CoolMeetingWeb.Controllers
{
    [Authorize]
    public class MeetingController : Controller
    {
        private CoolMeetingDbContext db = new CoolMeetingDbContext();

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult MeetingList(string type)
        {
            int employeeid = db.Employees.First(emp => emp.UserName == User.Identity.Name).EmployeeID;
            var meetings = db.Meetings.Include(m => m.Reservationist).
                Include(m => m.RoomReserved).Where(m => m.StartTime > DateTime.Now);
            ViewData["Type"] = type;
            MeetingListViewModel model = new MeetingListViewModel();
            if (type == "canceled")
            {
                model.MeetingListType = MeetingListTypeDef.Canceled;
                meetings = meetings.Where(m => m.IsCanceled);
            }
            else if (type == "reserved")
            {
                model.MeetingListType = MeetingListTypeDef.Reserved;
                meetings = meetings.Where(m => m.IsCanceled == false && m.ReservationistID == employeeid);
            }
            else
            {
                model.MeetingListType = MeetingListTypeDef.Participated;
                meetings = meetings.Where(m => m.IsCanceled == false && 
                    m.Participants.FirstOrDefault(e => e.EmployeeID == employeeid) != null);
            }
            model.Meetings = meetings.OrderByDescending(m => m.ReservationTime).ToList();
            return View(model);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meeting meeting = await db.Meetings.Include("Participants").Include("RoomReserved").FirstOrDefaultAsync(
                m => m.MeetingID == id.Value);
            if (meeting == null)
            {
                return HttpNotFound();
            }
            return View(meeting);
        }

        public ActionResult Create()
        {
            ViewBag.Rooms = new SelectList(
                db.Rooms.Where(r => r.RoomStatus == RoomStatusType.Active),
                "RoomID", "RoomCode");
            ViewBag.Departments = new SelectList(db.Departments, "DepartmentID", "DepartmentName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Meeting meeting)
        {
            if (ModelState.IsValidField("MeetingName") &&
                ModelState.IsValidField("NumberOfParticipants") &&
                ModelState.IsValidField("RoomID") &&
                ModelState.IsValidField("StartTime") &&
                ModelState.IsValidField("EndTime"))
            {
                // 检查会议室是否可用
                string errorMessage;
                bool result = ReservationRules.CheckRoom(meeting, out errorMessage);
                if (!result)
                {
                    ModelState.AddModelError("", errorMessage);
                }
                else
                {
                    // 添加参会人员
                    if (!string.IsNullOrEmpty(Request.Form["Participants"]))
                    {
                        string[] ids = Request.Form["Participants"].Split(',');
                        foreach (string id in ids)
                        {
                            meeting.Participants.Add(db.Employees.Find(int.Parse(id)));
                        }
                    }

                    // 预定者编号
                    Employee reservationist = db.Employees.First(emp => emp.UserName == User.Identity.Name);
                    meeting.ReservationistID = reservationist.EmployeeID;

                    db.Meetings.Add(meeting);
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
            }

            ViewBag.Rooms = new SelectList(
                db.Rooms.Where(r => r.RoomStatus == RoomStatusType.Active),
                "RoomID", "RoomCode", meeting.RoomID);
            ViewBag.Departments = new SelectList(db.Departments, "DepartmentID", "DepartmentName");

            return View(meeting);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meeting meeting = await db.Meetings.FindAsync(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }

            MeetingCancelViewModel model = new MeetingCancelViewModel();
            model.MeetingID = meeting.MeetingID;
            model.MeetingName = meeting.MeetingName;
            model.CanCancel = meeting.StartTime > DateTime.Now;

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(
            [Bind(Include="MeetingID, MeetingName, Reason, CanCancel")]
            MeetingCancelViewModel model)
        {
            if (ModelState.IsValidField("Reason"))
            {
                Meeting meeting = await db.Meetings.FindAsync(model.MeetingID);
                meeting.IsCanceled = true;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult Search(string meetingName, string reservationistName, 
            DateTime? fromDate, DateTime? toDate, bool? canceled,
            int? pageIndex, string sortFieldName, bool? descending)
        {
            int totalRows;
            int index = pageIndex.HasValue ? (pageIndex.Value - 1) : 0;
            List<Meeting> result = EntityFinder.GetMeetingsSortedAndPaged(meetingName,
                reservationistName, fromDate, toDate, canceled, 
                index, sortFieldName, descending, out totalRows);

            // 设置分页标签的数据模型
            PagedNavigatorViewModel navigatorModel = new PagedNavigatorViewModel
            {
                TotalRows = totalRows,
                CurrentPageIndex = index + 1
            };
            ViewBag.PagedNavigatorModel = navigatorModel;

            return View(result);
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

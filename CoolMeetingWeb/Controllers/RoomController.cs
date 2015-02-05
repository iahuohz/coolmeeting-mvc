using System;
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
    public class RoomController : Controller
    {
        private CoolMeetingDbContext db = new CoolMeetingDbContext();

        public async Task<ActionResult> Index()
        {
            return View(await db.Rooms.OrderByDescending(r => r.RoomStatus).ToListAsync());
        }

        // GET: Room/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "RoomID,RoomCode,RoomName,Capacity,Descrption")] Room room)
        {
            if (ModelState.IsValid)
            {
                var existed = db.Rooms.FirstOrDefault(r => r.RoomStatus != RoomStatusType.Retired &&
                    (r.RoomCode == room.RoomCode || r.RoomName == room.RoomName) );
                if (existed != null)
                {
                    ModelState.AddModelError("opresult", "会议室编号或名称已经存在！");
                }
                else
                {
                    db.Rooms.Add(room);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            return View(room);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = await db.Rooms.FindAsync(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = await db.Rooms.FindAsync(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "RoomID,RoomCode,RoomName,Capacity,Descrption,RoomStatus")] Room room)
        {
            if (ModelState.IsValid)
            {
                var existed = db.Rooms.FirstOrDefault(r => r.RoomStatus != RoomStatusType.Retired
                    && (r.RoomCode == room.RoomCode || r.RoomName == room.RoomName) 
                    && r.RoomID != room.RoomID);
                if (existed != null)
                {
                    ModelState.AddModelError("opresult", "会议室编号或名称已经存在！");
                }
                else
                {
                    db.Entry(room).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return View(room);
        }

        public async Task<ActionResult> Maintain(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = await db.Rooms.FindAsync(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            
            // 只有处于"活动"状态的会议室才能被"维护"
            if (room.RoomStatus != RoomStatusType.Active)
            {
                return RedirectToAction("Index");
            }

            MeetingsInRoomViewModel model = new MeetingsInRoomViewModel();
            model.MeetingRoom = room;
            // 查询该会议室所有已预定但尚未进行的会议，并且提示用户
            model.Meetings = await
                db.Meetings.Include("Reservationist").Where(m => m.RoomID == id.Value && 
                    m.StartTime >= DateTime.Now).ToListAsync();

            return View(model);
        }

        [HttpPost]
        [ActionName("Maintain")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MaintainConfirmed(int id)
        {
            // 修改会议室状态
            db.Rooms.Find(id).RoomStatus = RoomStatusType.Maintained;
            await db.SaveChangesAsync();

            // 将所有相关会议置为"取消"
            await db.Meetings.Where(m => m.RoomID == id).ForEachAsync(
                    (m) =>
                    {
                        m.IsCanceled = true;
                        m.CancelReason = "会议室维护中";
                    }
                );
            
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Restore(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = await db.Rooms.FindAsync(id);
            if (room == null)
            {
                return HttpNotFound();
            }

            // 只有处于"维护"状态的会议室才能恢复
            if (room.RoomStatus == RoomStatusType.Maintained)
            {
                room.RoomStatus = RoomStatusType.Active;
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = await db.Rooms.FindAsync(id);
            if (room == null)
            {
                return HttpNotFound();
            }

            MeetingsInRoomViewModel model = new MeetingsInRoomViewModel();
            model.MeetingRoom = room;
            // 查询该会议室所有已预定但尚未进行的会议，并且提示用户
            model.Meetings = await
                db.Meetings.Include("Reservationist").Where(m => m.RoomID == id.Value &&
                    m.StartTime >= DateTime.Now).ToListAsync();

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Room room = await db.Rooms.FindAsync(id);
            room.RoomStatus = RoomStatusType.Retired;
            await db.SaveChangesAsync();

            // 将所有相关会议置为"取消"
            await db.Meetings.Where(m => m.RoomID == id).ForEachAsync(
                    (m) => 
                    { 
                        m.IsCanceled = true;
                        m.CancelReason = "会议室被删除";
                    }
                );

            return RedirectToAction("Index");
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

using System.Linq;
using CoolMeetingWeb.Models;
using System;

namespace CoolMeetingWeb.Biz
{
    public class ReservationRules
    {
        public static bool CheckRoom(Meeting meeting, out string errorMessage)
        {
            using (CoolMeetingDbContext db = new CoolMeetingDbContext())
            {
                if (meeting.StartTime <= DateTime.Now)
                {
                    errorMessage = "会议起始时间不能早于当前时间";
                    return false;
                }

                if (meeting.StartTime >= meeting.EndTime)
                {
                    errorMessage = "会议结束时间不能早于起始时间";
                    return false;
                }

                Room room = db.Rooms.Find(meeting.RoomID);
                // 检查会议室容量
                if (room.Capacity < meeting.NumberOfParticipants)
                {
                    errorMessage = "会议室容量太小";
                    return false;
                }

                // 检查会议室的时间段是否可用
                var existed = db.Meetings.Where(m => !m.IsCanceled && m.RoomID == meeting.RoomID).
                    FirstOrDefault(m =>
                        (m.StartTime > meeting.StartTime && m.StartTime < meeting.EndTime) ||
                        (m.EndTime > meeting.StartTime && m.EndTime < meeting.EndTime));
                if (existed != null)
                {
                    errorMessage = "会议室时间冲突";
                    return false;
                }
                errorMessage = "";
                return true;
            }
        }
    }
}
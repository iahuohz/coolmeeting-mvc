using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using CoolMeetingWeb.Models;

namespace CoolMeetingWeb.Biz
{
    public class EntityFinder
    {
        public const int PAGE_SIZE = 3;
        public static List<Employee> GetEmployeesSortedAndPaged(
            string employeeName, string userName, string email,
            int? department, int? status, int pageIndex, 
            string sortFieldName, bool? descending,
            out int total)
        {
            using (CoolMeetingDbContext db = new CoolMeetingDbContext())
            {
                var query = db.Employees.Include(e => e.Department);

                if (!string.IsNullOrEmpty(employeeName))
                {
                    query = query.Where(emp => emp.EmployeeName.Contains(employeeName));
                }
                if (!string.IsNullOrEmpty(userName))
                {
                    query = query.Where(emp => emp.UserName.Contains(userName));
                }
                if (!string.IsNullOrEmpty(email))
                {
                    query = query.Where(emp => emp.Email == email);
                }
                if (department.HasValue)
                {
                    query = query.Where(emp => emp.DepartmentID == department.Value);
                }
                if (status.HasValue && status.Value == 1)
                {
                    query = query.Where(emp => emp.IsDeleted == true);
                }
                else
                {
                    query = query.Where(emp => emp.IsDeleted == false);
                }

                // 处理排序
                sortFieldName = string.IsNullOrEmpty(sortFieldName) ? "EmployeeID" : sortFieldName;
                descending = descending.HasValue ? descending.Value : false;
                query = EFSortHelper.PagedSort(query, sortFieldName, descending.Value);

                // 获取查询结果总数
                total = query.Count();

                // 分页
                query = query.Skip(PAGE_SIZE * pageIndex).Take(PAGE_SIZE);

                return query.ToList();
            }
        }



        public static List<Meeting> GetMeetingsSortedAndPaged(string meetingName, string reservationistName, 
            DateTime? fromDate, DateTime? toDate, bool? canceled,
            int pageIndex, string sortFieldName, bool? descending, out int total)
        {
            using (CoolMeetingDbContext db = new CoolMeetingDbContext())
            {
                var query = db.Meetings.Include(m => m.Reservationist).Include(m => m.RoomReserved);

                if (!string.IsNullOrEmpty(meetingName))
                {
                    query = query.Where(m => m.MeetingName.Contains(meetingName));
                }
                if (!string.IsNullOrEmpty(reservationistName))
                {
                    query = query.Where(m => m.Reservationist.EmployeeName.Contains(reservationistName));
                }
                if (fromDate.HasValue)
                {
                    query = query.Where(m => m.StartTime >= fromDate.Value);
                }
                if (toDate.HasValue)
                {
                    query = query.Where(m => m.StartTime <= toDate.Value);
                }
                if (canceled.HasValue)
                {
                    query = query.Where(m => m.IsCanceled == canceled.Value);
                }
                else
                {
                    query = query.Where(m => m.IsCanceled == false);
                }

                // 处理排序
                if (string.IsNullOrEmpty(sortFieldName))        // 默认情况下按照ID逆序排列
                {
                    sortFieldName = "MeetingID";
                    descending = true;
                }
                else
                {
                    descending = descending.HasValue ? descending.Value : false;
                }
                query = EFSortHelper.PagedSort(query, sortFieldName, descending.Value);

                // 获取查询结果总数
                total = query.Count();

                // 分页
                query = query.Skip(PAGE_SIZE * pageIndex).Take(PAGE_SIZE);

                return query.ToList();
            }
        }
    }
}
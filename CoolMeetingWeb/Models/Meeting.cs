using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CoolMeetingWeb.Models
{
    public class Meeting
    {
        public Meeting()
        {
            IsCanceled = false;
            ReservationTime = DateTime.Now;
        }

        [Display(Name = "会议编号")]
        public int MeetingID { get; set; }

        [Display(Name = "会议名称")]
        [Required(ErrorMessage = "*")]
        [StringLength(50)]
        public string MeetingName { get; set; }

        [Display(Name = "预计参会人数")]
        [Required(ErrorMessage = "*")]
        [Range(2, 1000, ErrorMessage = "预计参会人数在2~1000之间")]
        public int NumberOfParticipants { get; set; }

        [Display(Name = "预计起始时间")]
        [Required(ErrorMessage = "*")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime StartTime { get; set; }

        [Display(Name = "预计结束时间")]
        [Required(ErrorMessage = "*")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime EndTime { get; set; }

        [Display(Name = "会议描述")]
        [StringLength(200)]
        public string Description { get; set; }

        [Display(Name = "取消预定")]
        public bool IsCanceled { get; set; }

        [Display(Name = "取消原因")]
        [StringLength(400)]
        public string CancelReason { get; set; }

        [Display(Name = "预定时间")]
        public DateTime ReservationTime { get; set; }

        [Display(Name = "预定者")]
        public int ReservationistID { get; set; }
        [ForeignKey("ReservationistID")]
        public virtual Employee Reservationist { get; set; }

        [Display(Name = "会议室")]
        [Required(ErrorMessage = "*")]
        public int RoomID { get; set; }
        public virtual Room RoomReserved { get; set; }

        public virtual ICollection<Employee> Participants { get; set; }
    }
}
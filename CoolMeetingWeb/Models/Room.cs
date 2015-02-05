using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoolMeetingWeb.Models
{

    public enum RoomStatusType
    {
        Retired = -1,
        Maintained = 0,
        Active = 1
    }

    public class Room
    {
        public Room()
        {
            this.RoomStatus = RoomStatusType.Active;
        }

        [Display(Name = "会议室编号")]
        public int RoomID { get; set; }

        [Display(Name = "房间号")]
        [Required(ErrorMessage="*")]
        [StringLength(50)]
        public string RoomCode { get; set; }

        [Display(Name = "会议室名称")]
        [Required(ErrorMessage = "*")]
        [StringLength(50)]
        public string RoomName { get; set; }

        [Display(Name = "容量(人数)")]
        [Required(ErrorMessage = "*")]
        [Range(2, 1000, ErrorMessage="会议室容量为2~1000人")]
        public int Capacity { get; set; }

        [Display(Name = "描述")]
        [StringLength(200)]
        public string Descrption { get; set; }

        [Display(Name = "状态")]
        public RoomStatusType RoomStatus { get; set; }

        public virtual ICollection<Meeting> Meetings { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoolMeetingWeb.Models
{
    public enum GenderType
    {
        Female = 0,
        Male = 1
    }

    public class Employee
    {
        public Employee()
        {
            this.IsDeleted = false;
            this.CreatedTime = DateTime.Now;
        }

        [Display(Name = "员工编号")]
        public int EmployeeID { get; set; }

        [Display(Name = "员工姓名")]
        [Required(ErrorMessage = "*")]
        [StringLength(50)]
        public string EmployeeName { get; set; }

        [Display(Name = "账号名称")]
        [Required(ErrorMessage = "*")]
        [StringLength(50)]
        public string UserName { get; set; }

        [Display(Name = "电子邮件")]
        [Required(ErrorMessage = "*")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@([a-z0-9-]+(\.[a-z0-9-]+)*?\.[a-z]{2,6}|(\d{1,3}\.){3}\d{1,3})(:\d{4})?$",
            ErrorMessage = "电子邮件地址格式错误")]
        public string Email { get; set; }

        [NotMapped]
        [Display(Name = "密码")]
        [Required(ErrorMessage = "*")]
        [StringLength(50)]
        public string Password { get; set; }

        [NotMapped]
        [Display(Name = "确认密码")]
        [StringLength(50)]
        [Compare("Password", ErrorMessage = "密码不匹配")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "性别")]
        [Required(ErrorMessage = "*")]
        public GenderType Gender { get; set; }

        [Display(Name = "照片")]
        [StringLength(255)]
        public string PhotoUrl { get; set; }            // 员工照片路径

        [Display(Name = "注册时间")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime CreatedTime { get; set; }

        [Display(Name = "已删除")]
        public bool IsDeleted { get; set; }

        [Display(Name = "所属部门")]
        [Required(ErrorMessage = "*")]
        public int DepartmentID { get; set; }
        public virtual Department Department { get; set; }

        [InverseProperty("Reservationist")]
        public virtual ICollection<Meeting> MeetingsReserved { get; set; }

        public virtual ICollection<Meeting> MeetingsParticipated { get; set; }
    }
}
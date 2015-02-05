using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoolMeetingWeb.Models
{
    public class Department
    {
        [Display(Name = "部门编号")]
        public int DepartmentID { get; set; }

        [Display(Name = "部门名称")]
        [Required(ErrorMessage = "*")]
        [StringLength(50, ErrorMessage = "最大长度50个字符")]
        public string DepartmentName { get; set; }

        public virtual List<Employee> Employees { get; set; }
    }
}
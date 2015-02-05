using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoolMeetingWeb.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "账号")]
        [StringLength(50)]
        public string AccountName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "*")]
        [Display(Name = "用户名")]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [MinLength(5, ErrorMessage="密码不能少于{1}个字符")]
        [StringLength(50)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码不匹配")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "电子邮件")]
        public string Email { get; set; }
    }

    public class ChangePasswordViewModel
    {
        public ChangePasswordViewModel()
        {
            this.Success = false;
        }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(100, ErrorMessage = "密码长度不能少于{2}个字符", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("NewPassword", ErrorMessage = "两次输入的密码不匹配")]
        public string ConfirmPassword { get; set; }

        public bool Success { get; set; }
    }
}

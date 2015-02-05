using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using CoolMeetingWeb.Models;
using CoolMeetingWeb.ViewModels;


namespace CoolMeetingWeb.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private CoolMeetingDbContext db = new CoolMeetingDbContext();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = null;
            if (model.AccountName.Contains('@'))        // 电子邮件
            {
                user = UserManager.FindByEmail(model.AccountName);
            }
            else
            {
                user = UserManager.FindByName(model.AccountName);
            }

            if (user == null)
            {
                ModelState.AddModelError("", "错误的用户名或密码!");
                return View(model);
            }

            if (!user.EmailConfirmed)                   // 电子邮件地址没有验证
            {                
                return await GetEmailConfirmationView(user);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "错误的用户名或密码!");
                    return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.Departments = new SelectList(db.Departments, "DepartmentID", "DepartmentName");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(
            [Bind(Include = "EmployeeID,EmployeeName,UserName,Email,Password,ConfirmPassword,Gender,DepartmentID")] 
            Employee employee)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Departments = new SelectList(db.Departments, "DepartmentID", "DepartmentName", employee.DepartmentID);

                ApplicationUser user = UserManager.FindByName(employee.UserName);
                if (user != null)
                {
                    ModelState.AddModelError("dupusername", "此用户名已经存在！");
                    return View(employee);
                }

                user = UserManager.FindByEmail(employee.Email);
                if (user != null)
                {
                    ModelState.AddModelError("dupemail", "此电子邮件地址已经存在！");
                    return View(employee);
                }

                IdentityResult result = await UserManager.PasswordValidator.ValidateAsync(employee.Password);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("invalidpwd", result.Errors.First());
                    return View(employee);
                }

                user = new ApplicationUser { UserName = employee.UserName, Email = employee.Email };
                result = await UserManager.CreateAsync(user, employee.Password);
                if (result.Succeeded)
                {
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
                    // 在Employee数据库中保存员工信息
                    employee.PhotoUrl = Guid.NewGuid().ToString() + ".png";
                    db.Employees.Add(employee);
                    await db.SaveChangesAsync();

                    // 保存照片
                    if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        file.SaveAs(Path.Combine(Server.MapPath("/Photos"), employee.PhotoUrl));
                    }
                    
                    // 发送验证邮件
                    return await GetEmailConfirmationView(user);
                }

                AddErrors(result);
            }

            return View(employee);
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmSucceeded" : "Error");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "密码重置", "要重置您的密码，请点击 <a href=\"" + callbackUrl + "\"><b>此处</b></a>");

                return RedirectToAction("ForgotPasswordConfirmation", new { callbackUrl = callbackUrl });
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation(string callbackUrl)
        {
            return View((object)callbackUrl);
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Default");
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    model.Success = true;
                }
                else
                {
                    AddErrors(result);
                }
            }
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();

                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region 自定义辅助函数
        private async Task<ActionResult> GetEmailConfirmationView(ApplicationUser user)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

            await UserManager.SendEmailAsync(user.Id, "账号验证邮件", "要验证您的电子邮件信息，请点击 <a href=\"" + callbackUrl + "\"><b>此处</b></a>");

            return View("ConfirmNotify", (object)callbackUrl);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        #endregion

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
       
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Default");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}
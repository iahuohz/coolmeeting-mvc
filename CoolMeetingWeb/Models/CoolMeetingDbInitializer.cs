using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CoolMeetingWeb.Models
{
    public class CoolMeetingDbInitializer : DropCreateDatabaseIfModelChanges<CoolMeetingDbContext>
    {
        protected override void Seed(CoolMeetingDbContext context)
        {
            Department[] departments = new Department[] 
            {
                new Department{DepartmentName = "财务部"},
                new Department{DepartmentName = "技术部"},
                new Department{DepartmentName = "市场部"},
                new Department{DepartmentName = "销售部"}
            };
            context.Departments.AddRange(departments);
            context.SaveChanges();

            string initialPassword = "11111";
            Employee[] employees = new Employee[]
            {
                new Employee{
                    EmployeeName = "赵一", 
                    UserName = "zhaoyi",
                    Gender = GenderType.Male,
                    Email = "zhaoyi@etc.com",
                    Password = initialPassword,
                    ConfirmPassword = initialPassword,
                    PhotoUrl = Guid.NewGuid().ToString() + ".png",
                    DepartmentID = 1
                },
                new Employee{
                    EmployeeName = "赵二", 
                    UserName = "zhaoer",
                    Gender = GenderType.Male,
                    Email = "zhaoer@etc.com",
                    Password = initialPassword,
                    ConfirmPassword = initialPassword,
                    PhotoUrl = Guid.NewGuid().ToString() + ".png",
                    DepartmentID = 1
                },
                new Employee{
                    EmployeeName = "赵三",
                    UserName = "zhaosan",
                    Gender = GenderType.Female,
                    Email = "zhaosan@etc.com",
                    Password = initialPassword,
                    ConfirmPassword = initialPassword,
                    PhotoUrl = Guid.NewGuid().ToString() + ".png",
                    DepartmentID = 1
                },
                new Employee{
                    EmployeeName = "钱一",
                    UserName = "qianyi",
                    Gender = GenderType.Male,
                    Email = "qianyi@etc.com",
                    Password = initialPassword,
                    ConfirmPassword = initialPassword,
                    PhotoUrl = Guid.NewGuid().ToString() + ".png",
                    DepartmentID = 2
                },
                new Employee{
                    EmployeeName = "钱二", 
                    UserName = "qianer",
                    Gender = GenderType.Male,
                    Email = "qianer@etc.com",
                    Password = initialPassword,
                    ConfirmPassword = initialPassword,
                    PhotoUrl = Guid.NewGuid().ToString() + ".png",
                    DepartmentID = 2
                },
                new Employee{
                    EmployeeName = "钱三", 
                    UserName = "qiansan",
                    Gender = GenderType.Male,
                    Email = "qiansan@etc.com",
                    Password = initialPassword,
                    ConfirmPassword = initialPassword,
                    PhotoUrl = Guid.NewGuid().ToString() + ".png",
                    DepartmentID = 2
                },
                new Employee{
                    EmployeeName = "钱四", 
                    UserName = "qiansi",
                    Gender = GenderType.Female,
                    Email = "qiansi@etc.com",
                    Password = initialPassword,
                    ConfirmPassword = initialPassword,
                    PhotoUrl = Guid.NewGuid().ToString() + ".png",
                    DepartmentID = 2
                },
                new Employee{
                    EmployeeName = "钱五", 
                    UserName = "qianwu",
                    Gender = GenderType.Male,
                    Email = "qianwu@etc.com",
                    Password = initialPassword,
                    ConfirmPassword = initialPassword,
                    PhotoUrl = Guid.NewGuid().ToString() + ".png",
                    DepartmentID = 2
                },
                new Employee{
                    EmployeeName = "孙一", 
                    UserName = "sunyi",
                    Gender = GenderType.Female,
                    Email = "sunyi@etc.com",
                    Password = initialPassword,
                    ConfirmPassword = initialPassword,
                    PhotoUrl = Guid.NewGuid().ToString() + ".png",
                    DepartmentID = 3
                },
                new Employee{
                    EmployeeName = "孙二", 
                    UserName = "suner",
                    Gender = GenderType.Female,
                    Email = "suner@etc.com",
                    Password = initialPassword,
                    ConfirmPassword = initialPassword,
                    PhotoUrl = Guid.NewGuid().ToString() + ".png",
                    DepartmentID = 3
                }
            };
            context.Employees.AddRange(employees);
            context.SaveChanges();

            Room[] rooms = new Room[]
            {
                new Room
                {
                    RoomCode = "101",
                    RoomName = "第一会议室",
                    Capacity = 50,
                    Descrption = "大型综合性会议室",
                    RoomStatus = RoomStatusType.Active
                },
                new Room
                {
                    RoomCode = "102",
                    RoomName = "第二会议室",
                    Capacity = 20,
                    Descrption = "中型多媒体会议室",
                    RoomStatus = RoomStatusType.Active
                }
            };
            context.Rooms.AddRange(rooms);
            context.SaveChanges();

            InitialAccount(employees);
        }

        // 创建初始账号 
        private void InitialAccount(Employee[] employees)
        {
            ApplicationDbContext context = ApplicationDbContext.Create();
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            // 添加管理员角色
            string adminRole = "Admin";
            string adminUserName = "admin";
            string adminPassword = "admin";
            string adminEmail = "admin@etc.com";
            roleManager.Create(new IdentityRole(adminRole));
            // 添加管理员账号
            ApplicationUser admin = new ApplicationUser { UserName = adminUserName, Email = adminEmail, EmailConfirmed = true };
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 5,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            IdentityResult result = userManager.Create(admin, adminPassword);
            userManager.AddToRole(admin.Id, adminRole);
            
            // 添加初始员工账号
            foreach(Employee employee in employees)
            {
                userManager.Create(new ApplicationUser 
                    { UserName = employee.UserName, Email = employee.Email, EmailConfirmed = true },
                    employee.Password);
            }
        }
    }
}
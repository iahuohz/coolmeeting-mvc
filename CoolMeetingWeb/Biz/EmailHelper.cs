using System.Net.Mail;
using System.Text;
using System.Net;

namespace CoolMeetingWeb.Biz
{
    public class EmailHelper
    {
        const string fromEmail = "发件人邮件地址";
        const string hostName = "邮件服务器域名";
        const string userName = "邮箱账号名称";
        const string password = "邮箱账号密码";

        public static void Send(string destination, string subject, string body)
        {
            MailMessage msg = new MailMessage(fromEmail, destination);
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = true;
            msg.BodyEncoding = Encoding.UTF8;

            SmtpClient smtp = new SmtpClient(hostName);
            smtp.Credentials = new NetworkCredential(userName, password);
            smtp.SendMailAsync(msg);
        }
    }
}
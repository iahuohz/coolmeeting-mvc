using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CoolMeetingWeb.Startup))]
namespace CoolMeetingWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

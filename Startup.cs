using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(interview_log.Startup))]
namespace interview_log
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

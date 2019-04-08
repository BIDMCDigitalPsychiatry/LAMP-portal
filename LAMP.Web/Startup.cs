using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LAMP.Web.Startup))]
namespace LAMP.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

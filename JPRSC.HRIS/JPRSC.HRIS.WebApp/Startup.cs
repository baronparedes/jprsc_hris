using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JPRSC.HRIS.WebApp.Startup))]
namespace JPRSC.HRIS.WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CECHarmonization.Startup))]
namespace CECHarmonization
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

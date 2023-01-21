using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HealthCare.Startup))]
namespace HealthCare
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

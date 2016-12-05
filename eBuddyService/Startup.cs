using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(eBuddyService.Startup))]

namespace eBuddyService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}
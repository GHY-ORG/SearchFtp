using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UpdateFtp.Startup))]
namespace UpdateFtp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

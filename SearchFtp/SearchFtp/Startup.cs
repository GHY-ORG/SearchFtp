using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SearchFtp.Startup))]
namespace SearchFtp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

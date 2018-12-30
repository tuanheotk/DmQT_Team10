using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DIENMAYQUYETTIEN2.Startup))]
namespace DIENMAYQUYETTIEN2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

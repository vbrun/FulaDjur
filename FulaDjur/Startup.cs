using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FulaDjur.Startup))]
namespace FulaDjur
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

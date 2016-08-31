using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Orator.Startup))]
namespace Orator
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

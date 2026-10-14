using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Parikatan.Gift.Startup))]
namespace Parikatan.Gift

{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

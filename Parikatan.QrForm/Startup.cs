using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Parikatan.QrForm.Startup))]
namespace Parikatan.QrForm

{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LeNgocHueTran_BigSchool.Startup))]
namespace LeNgocHueTran_BigSchool
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

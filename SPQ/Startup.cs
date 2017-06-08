using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SPQ.Startup))]
namespace SPQ
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

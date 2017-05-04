using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Pixel_Color_Counter.Startup))]
namespace Pixel_Color_Counter
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

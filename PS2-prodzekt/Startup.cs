using Microsoft.Owin;
using Owin;
[assembly: OwinStartup(typeof(PS2_prodzekt.Startup))]
namespace PS2_prodzekt
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
            
        }
    }
}
using Microsoft.Owin;

[assembly: OwinStartup(typeof(BumpDetector.Web.Startup))]
namespace BumpDetector.Web
{
    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            try
            {
                // Any connection or hub wire up and configuration should go here
                app.MapSignalR();
            }
            catch (System.Exception e)
            {
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync(e.ToString());
                });
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Owin;
using Owin;
using noochAdminNew.Classes.Utility;

[assembly: OwinStartup(typeof(noochAdminNew.Startup2))]

namespace noochAdminNew
{
    public class Startup2
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            Logger.Info("Owin startup hit 2");

            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            GlobalConfiguration.Configuration.UseSqlServerStorage("NOOCHEntities");
            // configuring hangfire
            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}

using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Owin;
using noochAdminNew.Classes.Utility;
using Owin;

[assembly: OwinStartup(typeof(noochAdminNew.Startup))]

namespace noochAdminNew
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888


            // setting up hangfire
            GlobalConfiguration.Configuration.UseSqlServerStorage("data source=54.201.43.89;initial catalog=NOOCH;user id=sa;password=Singh@123;");

            //app.UseHangfireDashboard();
            app.UseHangfireServer();


            //RecurringJob.AddOrUpdate(() => Logger.Info("Auto Task Running"), "0 12 * */2");
            //RecurringJob.AddOrUpdate(() => Logger.Info("Auto Task Running"), Cron.Minutely);
        }
    }
}

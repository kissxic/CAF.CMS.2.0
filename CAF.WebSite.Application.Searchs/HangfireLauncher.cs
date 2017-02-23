using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.SqlServer;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Searchs
{
    public class HangfireLauncher
    {
        private readonly HangfireOptions _options;

        public HangfireLauncher(HangfireOptions options)
        {
            _options = options;
        }
        public void ConfigureOwin(IAppBuilder app)
        {
            JobStorage.Current = CreateJobStorage(Stage.ConfigureOwin);

            // Configure Hangfire dashboard
            var appPath = "/";

            var dashboardOptions = new DashboardOptions
            {
                AppPath = appPath,
            };

            app.UseHangfireDashboard(appPath + "hangfire", dashboardOptions);

            // Configure Hangfire server
            if (_options.StartServer)
            {
                app.UseHangfireServer(new BackgroundJobServerOptions { Activator = new Hangfire.JobActivator() });
            }
        }
        private JobStorage CreateJobStorage(Stage stage)
        {
            JobStorage result = null;

            if (string.Equals(_options.JobStorageType, "SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                var sqlServerStorageOptions = new SqlServerStorageOptions
                {
                    PrepareSchemaIfNecessary = stage == Stage.ConfigureDatabase,
                    QueuePollInterval = TimeSpan.FromSeconds(60) /* Default is 15 seconds */
                };

                result = new SqlServerStorage(_options.DatabaseConnectionStringName, sqlServerStorageOptions);
            }
            else if (string.Equals(_options.JobStorageType, "Memory", StringComparison.OrdinalIgnoreCase))
            {
                result = new MemoryStorage();
            }

            return result;
        }
        private enum Stage
        {
            ConfigureDatabase,
            ConfigureOwin
        }
    }
}

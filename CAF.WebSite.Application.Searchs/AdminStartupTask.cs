
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Searchs.BackgroundJobs;

namespace CAF.WebSite.Application.Searchs
{
    public class AdminStartupTask : IStartupTask
    {
        public void Execute()
        {
          var searchIndexJobsScheduler=  EngineContext.Current.Resolve<SearchIndexJobsScheduler>();
            searchIndexJobsScheduler.ScheduleJobs();
        }

        public int Order
        {
            get { return 100; }
        }
    }
}
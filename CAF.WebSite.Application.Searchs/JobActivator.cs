using System;
using Hangfire;
using CAF.Infrastructure.Core;

namespace CAF.WebSite.Application.Searchs
{
    [CLSCompliant(false)]
    public class JobActivator : Hangfire.JobActivator
    {
        public JobActivator()
        {
           
        }

        public override object ActivateJob(Type jobType)
        {
            return EngineContext.Current.Resolve(jobType);
        }
    }
}

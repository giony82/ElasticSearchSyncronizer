using System;

namespace Hangfire.Business.Jobs
{
    public class Jobs
    {
        public static void Configure()
        {
            RecurringJob.AddOrUpdate<SynchronizeElasticSearchJob>(x => x.DoWork(), Environment.GetEnvironmentVariable(EnvVarNameConstants.SyncJobCronExpression));
        }
    }
}

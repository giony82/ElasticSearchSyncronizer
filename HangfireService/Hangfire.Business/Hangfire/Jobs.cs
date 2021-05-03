using System;
using Hangfire;

namespace Hangfire.Business.Hangfire
{
    public class Jobs
    {
        public static void Configure()
        {
            RecurringJob.AddOrUpdate<SyncronizeElasticSearchJob>(x => x.DoWork(), Environment.GetEnvironmentVariable(EnvVarNameConstants.SyncJobCronExpression));
        }
    }
}

using ElasticSearch.Business.Interfaces;

namespace Hangfire.Business.Hangfire
{
    public class SyncronizeElasticSearchJob
    {
        private readonly IElasticSearchSyncService elasticSearchSyncService;

        public SyncronizeElasticSearchJob(IElasticSearchSyncService elasticSearchSyncService)
        {
            this.elasticSearchSyncService = elasticSearchSyncService;
        }

        [DisableConcurrentExecution(10)]
        [AutomaticRetry(Attempts = 0, LogEvents = false, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public void DoWork()
        {
            elasticSearchSyncService.Syncronize();
        }
    }
}

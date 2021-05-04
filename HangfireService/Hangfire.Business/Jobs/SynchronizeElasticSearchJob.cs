using ElasticSearch.Business.Interfaces;

namespace Hangfire.Business.Jobs
{
    public class SynchronizeElasticSearchJob
    {
        private readonly IElasticSearchSyncService _elasticSearchSyncService;

        public SynchronizeElasticSearchJob(IElasticSearchSyncService elasticSearchSyncService)
        {
            this._elasticSearchSyncService = elasticSearchSyncService;
        }

        [DisableConcurrentExecution(10)]
        [AutomaticRetry(Attempts = 0, LogEvents = false, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public void DoWork()
        {
            _elasticSearchSyncService.Syncronize();
        }
    }
}

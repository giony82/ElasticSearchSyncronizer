using Hangfire.Business.Interfaces;
using Hangfire.Repository.Interfaces;
using Serilog;

namespace Hangfire.Business.Services
{
    public class ElasticSearchSyncService : IElasticSearchSyncService
    {
        private readonly IElasticSearchSyncRepository _elasticSearchSyncRepo;

        private static readonly ILogger Logger = Log.ForContext<ElasticSearchSyncService>();
        
        public ElasticSearchSyncService(IElasticSearchSyncRepository elasticSearchSyncRepo)
        {
            _elasticSearchSyncRepo = elasticSearchSyncRepo;
        }

        public void Synchronize()
        {
            Logger.Debug("Synchronize called");

            _elasticSearchSyncRepo.Synchronize();            
        }
    }
}

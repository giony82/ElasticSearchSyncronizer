using ElasticSearch.Business.Interfaces;
using Hangfire.Repository.Interfaces;

namespace ElasticSearch.Syncronizer
{
    public class ElasticSearchSyncService : IElasticSearchSyncService
    {
        private readonly IElasticSearchSyncRepository elasticSearchSyncRepo;
        
        public ElasticSearchSyncService(IElasticSearchSyncRepository elasticSearchSyncRepo)
        {
            this.elasticSearchSyncRepo = elasticSearchSyncRepo;
        }

        public void Syncronize()
        {
            this.elasticSearchSyncRepo.Syncronize();            
        }
    }
}

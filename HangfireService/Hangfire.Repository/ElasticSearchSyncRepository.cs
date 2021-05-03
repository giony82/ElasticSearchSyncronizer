using Hangfire.Repository.Interfaces;
using RestSharp;
using SchoolUtils;

namespace Hangfire.Repository
{
    public class ElasticSearchSyncRepository : IElasticSearchSyncRepository
    {
        private readonly RestClient restClient;
        private readonly string resource = "/student";

        public ElasticSearchSyncRepository(IAppSettings appSettings)
        {
            var url = appSettings.Get<string>("ElasticSearchSyncronizerServiceURL");
            restClient = new RestClient(url)
            {
                ThrowOnAnyError = true
            };
        }

        public void Syncronize()
        {
            RestRequest request = new RestRequest($"{resource}/syncronize", Method.POST);

            //TODO add logic for 404
            var result = restClient.Execute(request);
        }
    }
}

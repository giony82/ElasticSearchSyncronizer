using Hangfire.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestSharp;
using SchoolUtils;

namespace Hangfire.Repository
{
    public class ElasticSearchSyncRepository : IElasticSearchSyncRepository
    {
        private readonly ILogger<ElasticSearchSyncRepository> _logger;
        private readonly string _resource = "/student";
        private readonly RestClient _restClient;

        public ElasticSearchSyncRepository(IAppSettings appSettings, ILogger<ElasticSearchSyncRepository> logger)
        {
            _logger = logger;
            var url = appSettings.Get<string>(RepositoryConstants.ElasticSearchSynchronizerServiceURL);

            //TODO add some kind of factory to hide RestClient
            _restClient = new RestClient(url)
            {
                ThrowOnAnyError = true,
                ThrowOnDeserializationError = true
            };
        }

        public void Synchronize()
        {
            _logger.LogDebug("Synchronizing students..");
            var request = new RestRequest($"{_resource}/synchronize", Method.POST);

            //TODO add polly
            IRestResponse result = _restClient.Execute(request);

            if (!result.IsSuccessful)
            {
                _logger.LogError(result.ErrorMessage);

                throw result.ErrorException;
            }
        }
    }

  
}
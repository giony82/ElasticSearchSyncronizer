using ElasticSearch.Repository.Entities;
using ElasticSearch.Repository.Interfaces;
using RestSharp;
using SchoolUtils;

namespace ElasticSearch.Repository
{
    public class StudentRepository: IStudentRepository
    {
        private readonly RestClient _restClient;
        private readonly string resource = "/student/";
        
        public StudentRepository(IAppSettings appSettings)
        {
            var url = appSettings.Get<string>("StudentServiceURL");

            //TODO add factory
            _restClient = new RestClient(url); 
        }

        public StudentEntity Get(string id)
        {
            var request = new RestRequest($"{resource}{{id}}", Method.GET);
            request.AddUrlSegment("id", id);

            var response = this._restClient.Execute<StudentEntity>(request);

            if (!response.IsSuccessful)
            {
                throw response.ErrorException;
            }

            return response.Data;
        }
    }
}

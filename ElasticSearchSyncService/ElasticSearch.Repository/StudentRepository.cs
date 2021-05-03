using ElasticSearch.Repository.Entities;
using ElasticSearch.Repository.Interfaces;
using RestSharp;
using SchoolUtils;

namespace ElasticSearch.Repository
{
    public class StudentRepository: IStudentRepository
    {
        private RestClient restClient;
        private readonly string resource = "/student/";
        
        public StudentRepository(IAppSettings appSettings)
        {
            var url = appSettings.Get<string>("StudentServiceURL");
            this.restClient = new RestClient(url); 
        }

        public StudentEntity Get(string id)
        {
            RestRequest request = new RestRequest($"{resource}{{id}}", Method.GET);
            request.AddUrlSegment("id", id);

            IRestResponse<StudentEntity> response = this.restClient.Execute<StudentEntity>(request);

            return response.Data;
        }
    }
}

using AutoMapper;
using ElasticSearch.Business.Models;
using ElasticSearch.Repository.Entities;

namespace ElasticSearch.Business.Automapper
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<StudentEntity, StudentModel>();
        }
    }
}

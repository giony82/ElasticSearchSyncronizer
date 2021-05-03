using ElasticSearch.Business.Automapper;
using ElasticSearch.Repository;
using AutoMapper;
using ElasticSearch.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ElasticSearch.Business
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureBusinessServices(this IServiceCollection services)
        {
            services.AddTransient<IStudentRepository, StudentRepository>();

            services.AddAutoMapper(typeof(AutoMapperProfiles));
        }
    }
}

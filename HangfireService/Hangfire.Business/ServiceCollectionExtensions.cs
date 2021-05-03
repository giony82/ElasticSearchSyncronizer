using Hangfire.Repository;
using Hangfire.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Hangfire.Business
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureBusinessServices(this IServiceCollection services)
        {
            services.AddTransient<IElasticSearchSyncRepository, ElasticSearchSyncRepository>();
        }
    }
}

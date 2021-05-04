using Common;
using Common.Services;
using ElasticSearch.Business;
using ElasticSearch.Business.Interfaces;
using ElasticSearch.Business.Services;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Redis.Business;
using Redis.Interfaces;
using SchoolUtils;

namespace ElasticSearchWebService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<ElasticSynchronizer>();

            services.AddTransient<IStudentSynchronizer,StudentSynchronizer>();
            services.AddTransient<IAppSettings,AppSettings>();
            services.AddSingleton<IRedisService,RedisService>();
            services.AddTransient<IStudentService, StudentService>();
            services.AddTransient<ServiceRetry>();
            services.ConfigureBusinessServices();    
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

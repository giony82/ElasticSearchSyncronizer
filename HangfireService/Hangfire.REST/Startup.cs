using System;
using ElasticSearch.Business.Interfaces;
using ElasticSearch.Syncronizer;
using Hangfire;
using Hangfire.Business;
using Hangfire.Business.Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            services.AddTransient<IElasticSearchSyncService, ElasticSearchSyncService>();
            services.AddTransient<IAppSettings, AppSettings>();

            services.ConfigureBusinessServices();

            services.AddHangfire(x => x.UseSqlServerStorage(Environment.GetEnvironmentVariable(EnvVarNameConstants.HangfireConnectionString)).UseRecommendedSerializerSettings()).AddHangfireServer();            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseHangfireServer();

            Jobs.Configure();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

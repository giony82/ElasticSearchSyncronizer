using System;
using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Redis.Business;
using Redis.Interfaces;
using SchoolUtils;
using SqlChangeTrackerService.Business;

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
            
            services.AddTransient<IAppSettings,AppSettings>();
            services.AddSingleton<IRedisService,RedisService>();            
            services.AddTransient<ServiceRetry>();

            string connectionString = Environment.GetEnvironmentVariable(EnvVarNameConstants.ConnectionString);


            services.AddSingleton((s)=> {
				return new SqlEntityChangeTracker(connectionString, s.GetService<IRedisService>());								
			});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SqlEntityChangeTracker sqlEntityChangeTracker)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            sqlEntityChangeTracker.Start();

            app.UseRouting();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

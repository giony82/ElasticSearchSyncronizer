using System;
using System.Threading;
using Hangfire.Business;
using Hangfire.Business.Interfaces;
using Hangfire.Business.Jobs;
using Hangfire.Business.Services;
using Hangfire.Data;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchoolUtils;
using EnvVarNameConstants = ElasticSearchWebService.EnvVarNameConstants;

namespace Hangfire.REST
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

            var connectionString = Environment.GetEnvironmentVariable(EnvVarNameConstants.HangfireConnectionString) ??
                                   throw new NullReferenceException(EnvVarNameConstants.HangfireConnectionString);

            services.AddDbContext<HangfireContext>(x => x.UseSqlServer(connectionString));

            CreateDbForNonProdMode(services);

            services.AddTransient<IElasticSearchSyncService, ElasticSearchSyncService>();
            services.AddTransient<IAppSettings, AppSettings>();

            services.ConfigureBusinessServices();

            services.AddHangfire(x => x.UseSqlServerStorage(connectionString,
                    new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    })
                .UseRecommendedSerializerSettings()
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseRecommendedSerializerSettings()).AddHangfireServer();
        }

        private static void CreateDbForNonProdMode(IServiceCollection services)
        {
            // Build an intermediate service provider to get back the HF context and create the DB
            ServiceProvider sp = services.BuildServiceProvider();

            var env = sp.GetService<IWebHostEnvironment>();
            if (env.IsDevelopment())
            {
                var db = sp.GetService<HangfireContext>();
                db.Create();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, HangfireContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseHangfireServer();
            app.UseHangfireDashboard();

            Jobs.Configure();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

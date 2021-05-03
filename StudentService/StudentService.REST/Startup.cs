using System;
using System.IO;
using System.Reflection;
using AutoMapper;
using CommandsStack.Infrastructure;
using Common;
using Common.Data;
using Common.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Redis.Business;
using Redis.Interfaces;
using SchoolUtils;
using StudentService.Business;
using StudentService.Business.Interfaces;
using StudentService.REST.Automapper;

namespace School.API
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

            string connectionString = Environment.GetEnvironmentVariable(EnvVarNameConstants.ConnectionString);

            services.AddDbContext<SchoolContext>(x => x.UseSqlServer(connectionString));

            services.AddTransient<IStudentService, StudentService.Business.StudentService>();
            services.AddTransient<IAppSettings, AppSettings>();            
            services.AddTransient<ServiceRetry>();            
            
            services.AddAutoMapper(typeof(AutoMapperProfiles));
            services.AddSingleton<IRedisService, RedisService>();
            
            services.ConfigureBusinessServices();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Student API",                    
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SchoolContext dataContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

         
            try
            {
                // migrate any database changes on startup (includes initial db creation)
                // TODO create separated project to migrate the DB
                dataContext.Database.Migrate();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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

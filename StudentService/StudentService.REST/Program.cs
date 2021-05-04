using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace StudentService.REST
{
    public class Program
    {
        private static readonly string AspNetCoreEnvironment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        private const string ElasticSearchLogUrl = "ElasticSearchLogURL";

        public static void Main(string[] args)
        {
            ConfigureLogging();

            CreateHostBuilder(args).Build().Run();
        }

        private static ElasticsearchSinkOptions ConfigureElasticSink(string environment)
        {

            var elasticSearchUrl = Environment.GetEnvironmentVariable(ElasticSearchLogUrl);
            return new ElasticsearchSinkOptions(new Uri(elasticSearchUrl ??
                                                        throw new NullReferenceException(ElasticSearchLogUrl)))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"applogs-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }

        private static void ConfigureLogging()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{AspNetCoreEnvironment}.json", optional: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Elasticsearch(ConfigureElasticSink(AspNetCoreEnvironment))
                .Enrich.WithProperty("Service", "HangFire")
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureAppConfiguration(configuration =>
                {
                    configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    configuration.AddJsonFile($"appsettings.{AspNetCoreEnvironment}.json", optional: true);
                })
                .UseSerilog();
    }
}

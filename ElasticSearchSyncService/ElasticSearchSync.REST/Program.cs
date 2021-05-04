using System;
using ElasticSearchWebService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace ElasticSearchSync.REST
{
    public class Program
    {
        private static readonly string AspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        private static string _appSettingsJson;
        private const string ElasticSearchLogUrl = "ElasticSearchLogURL";

        public static void Main(string[] args)
        {
            _appSettingsJson = "appsettings.json";

            ConfigureLogging();

            CreateHostBuilder(args).Build().Run();
        }

        private static ElasticsearchSinkOptions ConfigureElasticSink(string environment)
        {
            var environmentVariable = Environment.GetEnvironmentVariable(ElasticSearchLogUrl);
            if (environmentVariable == null) throw new NullReferenceException(ElasticSearchLogUrl);

            return new ElasticsearchSinkOptions(new Uri(environmentVariable))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"applogs-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }

        private static void ConfigureLogging()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile(_appSettingsJson, false, true)
                .AddJsonFile($"appsettings.{AspNetCoreEnvironment}.json", true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Elasticsearch(ConfigureElasticSink(AspNetCoreEnvironment))
                .Enrich.WithProperty("Service", "ElasticSearchSyncService")
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureAppConfiguration(configuration =>
                {
                    configuration.AddJsonFile(_appSettingsJson, false, true);
                    configuration.AddJsonFile($"appsettings.{AspNetCoreEnvironment}.json", true);
                })
                .UseSerilog();
        }
    }
}
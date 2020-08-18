using System.Linq;
using System.Net.Http;
using HealthCheck.Domains;
using HealthCheck.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;

namespace HealthCheck
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var envConfig = new ConfigurationBuilder().AddEnvironmentVariables().Build();

            var metricServer = new MetricServer(port: int.Parse(envConfig["config:metricport"]));
            metricServer.Start();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(
                builder =>
                {
                    builder.AddJsonFile("appsettings.json");
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient("MyHttp")
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    });

                    services.AddSingleton<ISlackLoggerService, SlackLoggerService>();
                    services.AddTransient<IWebSiteService, WebSiteService>();
                    services.AddHostedService<Worker>();
                    
                    var a = hostContext.Configuration.GetSection("ServerConfigs").GetChildren().Select(c => c.Value).ToList();
                    var servers = a.ToDictionary(item => item.Split(":")[0], item => item.Split(":")[1]);

                    services.AddSingleton(s => new Config(
                        hostContext.Configuration.GetSection("WebsiteName").Value,
                        servers,
                        hostContext.Configuration.GetSection("SlackLoggerEndPoint").Value,
                        hostContext.Configuration.GetSection("SlackLoggerWebhookUrl").Value,
                        int.Parse(hostContext.Configuration.GetSection("CheckingTimeInterval").Value)
                    ));
                });
    }
}

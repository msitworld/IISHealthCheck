using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthCheck.Domains;
using HealthCheck.Models;
using HealthCheck.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HealthCheck
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISlackLoggerService _slackLoggerService;
        private readonly IWebSiteService _webSiteService;
        private readonly Config _config;

        public Worker(ILogger<Worker> logger,
            IWebSiteService webSiteService,
            Config config, ISlackLoggerService slackLoggerService)
        {
            _logger = logger;
            _webSiteService = webSiteService;
            _config = config;
            _slackLoggerService = slackLoggerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                foreach (var server in _config.TargetServers)
                {
                    try
                    {
                        _webSiteService.SetServerAddress(server.Key, server.Value);

                        var sites = await _webSiteService.GetWebSitesListAsync();
                        var website = sites?.FirstOrDefault(c => string.Equals(c.Name, _config.WebSiteName, StringComparison.CurrentCultureIgnoreCase));

                        if (website == null) continue;

                        var site = await _webSiteService.GetWebSiteAsync(website.Id);

                        if (site.ApplicationPool.Status != Status.Started)
                        {
                            await _slackLoggerService.LogAsync(LogLevel.Warning, $"{server.Key} ApplicationPool status is Stopped!");

                            if (await _webSiteService.SetAppPoolStatus(site.ApplicationPool.Id, Status.Started))
                                await _slackLoggerService.LogAsync(LogLevel.Information, $"{server.Key} ApplicationPool status started successfully.");

                        }

                        if (site.Status == Status.Started) continue;

                        await _slackLoggerService.LogAsync(LogLevel.Warning, $"{server.Key} WebSite status is Stopped!");

                        if (await _webSiteService.SetWebSiteStatus(site.Id, Status.Started))
                            await _slackLoggerService.LogAsync(LogLevel.Information, $"{server.Key} WebSite status started successfully.");

                    }
                    catch (Exception ex)
                    {
                        await _slackLoggerService.LogAsync(LogLevel.Error, $"Exception thrown in at working on server {server.Key} | Reason: {ex.Message}");

                        _logger.LogError(ex, $"Exception thrown in at working on server {server.Key}");
                    }
                }

                await Task.Delay(_config.CheckingTimeInterval * 1000, stoppingToken);
            }
        }
    }
}

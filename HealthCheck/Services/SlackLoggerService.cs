using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HealthCheck.Domains;

namespace HealthCheck.Services
{
    public class SlackLoggerService : ISlackLoggerService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SlackLoggerService> _logger;
        private readonly Config _config;

        public SlackLoggerService(IHttpClientFactory httpClientFactory,
            ILogger<SlackLoggerService> logger, Config config)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _config = config;
        }

        public async Task LogAsync(LogLevel level, string message)
        {
            var client = _httpClientFactory.CreateClient("MyHttp");

            var content = JsonConvert.SerializeObject(new
            {
                WebhookUrl = _config.SlackLoggerWebhookUrl,
                Message = message,
                Type = level.ToString(),
            }, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            try
            {
                await client.PostAsync(_config.SlackLoggerEndPoint, new StringContent(content, Encoding.UTF8, "application/json"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown in sending log to Slack");
            }
        }

    }
}

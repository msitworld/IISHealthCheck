using AutoMapper;
using HealthCheck.Mapping;
using HealthCheck.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;

namespace HealthCheck.Services
{
    public class WebSiteService : IWebSiteService
    {
        private string _baseAddress;
        private string _token;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WebSiteService> _logger;
        private readonly ISlackLoggerService _slackLoggerService;

        public WebSiteService(IHttpClientFactory httpClientFactory,
            ILogger<WebSiteService> logger, ISlackLoggerService slackLoggerService)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _slackLoggerService = slackLoggerService;
        }

        public async Task<IList<WebSiteDto>> GetWebSitesListAsync()
        {
            HttpResponseMessage result = 
                await SendHttpRequestAsync("/api/webserver/websites", HttpMethod.Get);

            if (result == null || !result.IsSuccessStatusCode)
            {
                await _slackLoggerService.LogAsync(LogLevel.Error, $"Can not GetWebSitesListAsync | Reason: {result?.ReasonPhrase}");

                return null;
            }

            var body = await result.Content.ReadAsStringAsync();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<JObject, WebSiteDto>();
                cfg.AddProfile<WebSitesMappingProfile>();
            });

            var mapper = config.CreateMapper();

            var jsonObj = JObject.Parse(body);

            return mapper.Map<List<WebSiteDto>>(jsonObj);

        }

        public async Task<SiteDto> GetWebSiteAsync(string id)
        {
            HttpResponseMessage result = await SendHttpRequestAsync($"/api/webserver/websites/{id}",
                            HttpMethod.Get);

            if (result == null || !result.IsSuccessStatusCode)
            {
                await _slackLoggerService.LogAsync(LogLevel.Error, $"Can not GetWebSiteAsync | Reason: {result?.ReasonPhrase}");

                return null;
            }

            var body = await result.Content.ReadAsStringAsync();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<JObject, SiteDto>();
                cfg.AddProfile<MappingProfile>();
            });

            var mapper = config.CreateMapper();

            var jsonObj = JObject.Parse(body);

            return mapper.Map<SiteDto>(jsonObj);
        }

        public async Task<bool> SetWebSiteStatus(string id, Status status)
        {
            dynamic sts = new
            {
                status = status.ToString().ToLower()
            };

            HttpResponseMessage result = await SendHttpRequestAsync($"/api/webserver/websites/{id}", 
                HttpMethod.Patch, 
                JsonConvert.SerializeObject(sts));

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> SetAppPoolStatus(string id, Status status)
        {
            dynamic sts = new { status = status.ToString().ToLower() };

            HttpResponseMessage result = await SendHttpRequestAsync($"/api/webserver/application-pools/{id}",
                HttpMethod.Patch,
                JsonConvert.SerializeObject(sts));

            return result != null && result.IsSuccessStatusCode;
        }

        private async Task<HttpResponseMessage> SendHttpRequestAsync(string uri, HttpMethod httpMethod, string body = null)
        {
            var client = _httpClientFactory.CreateClient("MyHttp");

            client.DefaultRequestHeaders.Add("Access-Token", $"Bearer {_token}");
            client.DefaultRequestHeaders.Add("Accept", "application/hal+json");
            client.BaseAddress = new Uri(_baseAddress);

            HttpRequestMessage httpRequest = new HttpRequestMessage(httpMethod, uri);

            if (!string.IsNullOrEmpty(body))
            {
                httpRequest.Content = new StringContent(body, Encoding.UTF8, "application/json"); ;
            }

            try
            {
                var result = await client.SendAsync(httpRequest);

                if (result != null) return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception thrown in sending HTTP request at server {_baseAddress}");
            }

            return null;

        }

        public void SetServerAddress(string address, string token)
        {
            _baseAddress = $"https://{address}:55539";
            _token = token;
        }
    }
}

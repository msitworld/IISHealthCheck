using AutoMapper;
using HealthCheck.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace HealthCheck.Mapping
{
    public class JObjectToWebSiteListConverter : ITypeConverter<JObject, List<WebSiteDto>>
    {
        public List<WebSiteDto> Convert(JObject source, List<WebSiteDto> destination, ResolutionContext context)
        {

            var sitesList = new List<WebSiteDto>();
            var tokenCountItems = source.SelectTokens("websites").Children().Count();
            for (int i = 0; i < tokenCountItems; i++)
            {
                var token = source.SelectToken($"websites[{i}]");
                var result = new WebSiteDto();

                if (token != null)
                {
                    result = context.Mapper.Map<JToken, WebSiteDto>(token);
                }
                sitesList.Add(result);
            }

            return sitesList;

        }

    }
}

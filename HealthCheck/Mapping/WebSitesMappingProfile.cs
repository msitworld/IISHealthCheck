using AutoMapper;
using HealthCheck.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace HealthCheck.Mapping
{
    public class WebSitesMappingProfile : Profile
    {
        public WebSitesMappingProfile()
        {
            CreateMap<JObject, List<WebSiteDto>>().ConvertUsing<JObjectToWebSiteListConverter>();

            var a = CreateMap<JObject, WebSiteDto>()
                .ForMember(x => x.Id, cfg => cfg.MapFrom(jo => jo.SelectToken(".id")))
                .ForMember(x => x.Name, cfg => cfg.MapFrom(jo => jo.SelectToken(".name")))
                .ForMember(x => x.Status, cfg => cfg.MapFrom(jo => jo.SelectToken(".status").ToString().ToUpperInvariant()));
        }
    }
}

using AutoMapper;
using HealthCheck.Models;
using Newtonsoft.Json.Linq;

namespace HealthCheck.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            var a = CreateMap<JObject, SiteDto>()
                .ForMember(x => x.Id, cfg => cfg.MapFrom(jo => jo.SelectToken(".id")))
                .ForMember(x => x.Name, cfg => cfg.MapFrom(jo => jo.SelectToken(".name")))
                .ForMember(x => x.Status, cfg => cfg.MapFrom(jo => jo.SelectToken(".status").ToString().ToUpperInvariant()))
                .ForMember(x => x.PhysicalPath, cfg => cfg.MapFrom(jo => jo.SelectToken(".physical_path")))

                .ForPath(x => x.ApplicationPool.Id, cfg => cfg.MapFrom(jo => jo.SelectToken(".application_pool.id")))
                .ForPath(x => x.ApplicationPool.Status, cfg => cfg.MapFrom(jo => jo.SelectToken(".application_pool.status").ToString().ToUpperInvariant()))
                .ForPath(x => x.ApplicationPool.Name, cfg => cfg.MapFrom(jo => jo.SelectToken(".application_pool.name")));
        }
    }

}

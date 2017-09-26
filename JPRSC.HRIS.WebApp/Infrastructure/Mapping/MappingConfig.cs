using AutoMapper;

namespace JPRSC.HRIS.WebApp.Infrastructure.Mapping
{
    public class MappingConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfiles(typeof(MappingConfig));
            });
        }
    }
}
using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.SystemSettings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<JPRSC.HRIS.Models.SystemSettings, Search.QueryResult.SystemSettings>();
            CreateMap<JPRSC.HRIS.Models.SystemSettings, Edit.Command>();
        }
    }
}
using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.Religions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Religion, Search.QueryResult.Religion>();
            CreateMap<Religion, Edit.Command>();
        }
    }
}
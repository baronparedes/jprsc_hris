using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.CustomRoles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CustomRole, Search.QueryResult.CustomRole>();
            CreateMap<CustomRole, Edit.Command>();
        }
    }
}
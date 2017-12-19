using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.Companies
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, Search.QueryResult.Company>();
            CreateMap<Company, Edit.Command>();
        }
    }
}
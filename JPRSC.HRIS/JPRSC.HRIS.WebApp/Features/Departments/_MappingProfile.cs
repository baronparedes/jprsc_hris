using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.Departments
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Department, Search.QueryResult.Department>();
            CreateMap<Department, Edit.Command>();
        }
    }
}
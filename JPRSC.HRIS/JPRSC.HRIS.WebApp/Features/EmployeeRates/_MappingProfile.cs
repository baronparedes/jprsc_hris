using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.EmployeeRates
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, Search.QueryResult.Employee>();
            CreateMap<Employee, Edit.Command>();
        }
    }
}
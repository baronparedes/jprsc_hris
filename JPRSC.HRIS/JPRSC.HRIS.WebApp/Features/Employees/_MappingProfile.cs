using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.Employees
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Client, Index.QueryResult.Client>();
            CreateMap<Employee, Search.QueryResult.Employee>();
            CreateMap<Employee, Edit.Command>();
        }
    }
}
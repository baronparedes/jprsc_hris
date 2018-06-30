using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.DailyTimeRecords
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, Search.QueryResult.Employee>();
            CreateMap<DailyTimeRecord, Edit.Command>();
            CreateMap<Client, Index.QueryResult.Client>();
        }
    }
}
using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.DailyTimeRecords
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DailyTimeRecord, Search.QueryResult.DailyTimeRecord>();
            CreateMap<DailyTimeRecord, Edit.Command>();
        }
    }
}
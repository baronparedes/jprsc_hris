using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.DailyTimeRecords
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, Search.QueryResult.Employee>();
            CreateMap<DailyTimeRecord, Search.QueryResult.DailyTimeRecord>();
            CreateMap<DailyTimeRecord, Edit.Command>();
            CreateMap<Client, Index.QueryResult.Client>();
            CreateMap<EarningDeduction, Index.QueryResult.EarningDeduction>();
            CreateMap<PayPercentage, Index.QueryResult.PayPercentage>();
        }
    }
}
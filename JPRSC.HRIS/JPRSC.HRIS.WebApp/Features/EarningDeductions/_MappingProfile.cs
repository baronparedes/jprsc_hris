using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.EarningDeductions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EarningDeduction, Search.QueryResult.EarningDeduction>();
            CreateMap<EarningDeduction, Edit.Command>();
        }
    }
}
using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.PayPercentages
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PayPercentage, Search.QueryResult.PayPercentage>();
            CreateMap<PayPercentage, Edit.Command>();
        }
    }
}
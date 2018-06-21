using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.LoanTypes
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LoanType, Search.QueryResult.LoanType>();
            CreateMap<LoanType, Edit.Command>();
        }
    }
}
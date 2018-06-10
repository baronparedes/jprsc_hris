using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.Banks
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Bank, Search.QueryResult.Bank>();
            CreateMap<Bank, Edit.Command>();
        }
    }
}
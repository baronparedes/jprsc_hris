using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.TaxStatuses
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TaxStatus, Search.QueryResult.TaxStatus>();
            CreateMap<TaxStatus, Edit.Command>();
            CreateMap<TaxRange, Edit.Command.TaxRange>();
            CreateMap<TaxRange, Search.QueryResult.TaxRange>();
        }
    }
}
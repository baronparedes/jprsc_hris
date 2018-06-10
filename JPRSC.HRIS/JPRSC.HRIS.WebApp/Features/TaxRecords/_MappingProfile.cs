using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.TaxRecords
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TaxRecord, Search.QueryResult.TaxRecord>();
            CreateMap<TaxRecord, Edit.Command>();
            CreateMap<TaxRange, Edit.Command.TaxRange>();
            CreateMap<TaxRange, Search.QueryResult.TaxRange>();
        }
    }
}
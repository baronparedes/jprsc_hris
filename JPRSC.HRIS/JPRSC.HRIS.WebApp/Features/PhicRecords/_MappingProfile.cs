using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.PhicRecords
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PhicRecord, Search.QueryResult.PhicRecord>();
            CreateMap<PhicRecord, Edit.Command>();
        }
    }
}
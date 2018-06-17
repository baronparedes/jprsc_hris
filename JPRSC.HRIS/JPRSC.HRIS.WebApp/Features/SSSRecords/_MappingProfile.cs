using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.SSSRecords
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SSSRecord, Search.QueryResult.SSSRecord>();
            CreateMap<SSSRecord, Edit.Command>();
        }
    }
}
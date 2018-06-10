using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.PagIbigRecords
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PagIbigRecord, Search.QueryResult.PagIbigRecord>();
            CreateMap<PagIbigRecord, Edit.Command>();
        }
    }
}
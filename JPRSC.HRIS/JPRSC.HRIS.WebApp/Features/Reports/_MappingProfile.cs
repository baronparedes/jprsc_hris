using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.Reports
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Client, Index.QueryResult.Client>();
        }
    }
}
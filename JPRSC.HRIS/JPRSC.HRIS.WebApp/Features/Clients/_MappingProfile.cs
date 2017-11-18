using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.Clients
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Client, Search.QueryResult.Client>();
            CreateMap<Client, Edit.Command>();
        }
    }
}
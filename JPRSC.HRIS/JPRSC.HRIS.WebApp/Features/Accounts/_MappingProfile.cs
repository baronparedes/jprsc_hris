using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.Accounts
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, Search.QueryResult.Account>();
            CreateMap<User, Edit.Command>();
            CreateMap<User, ChangePassword.Command>();
        }
    }
}
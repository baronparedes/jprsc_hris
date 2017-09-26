using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.Companies
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CompanyProfile, Index.Result.CompanyProfile>();
            CreateMap<CompanyProfile, Edit.Command>();
        }
    }
}
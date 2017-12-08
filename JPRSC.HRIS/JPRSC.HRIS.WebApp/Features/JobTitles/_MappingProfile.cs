using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.JobTitles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<JobTitle, Search.QueryResult.JobTitle>();
            CreateMap<JobTitle, Edit.Command>();
        }
    }
}
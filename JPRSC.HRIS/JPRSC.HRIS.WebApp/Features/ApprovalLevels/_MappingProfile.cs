using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.ApprovalLevels
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApprovalLevel, Search.QueryResult.ApprovalLevel>();
            CreateMap<ApprovalLevel, Edit.Command>();
        }
    }
}
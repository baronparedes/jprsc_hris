using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.Loans
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Loan, Search.QueryResult.Loan>();
            CreateMap<Loan, Edit.Command>();
        }
    }
}
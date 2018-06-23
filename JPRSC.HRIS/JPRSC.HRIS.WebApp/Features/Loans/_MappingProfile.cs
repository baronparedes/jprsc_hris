using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.Loans
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Client, Index.QueryResult.Client>();
            CreateMap<Employee, Search.QueryResult.Employee>();
            CreateMap<Loan, Search.QueryResult.Loan>();
            CreateMap<LoanType, Search.QueryResult.LoanType>();
            CreateMap<Loan, Edit.Command>();
        }
    }
}
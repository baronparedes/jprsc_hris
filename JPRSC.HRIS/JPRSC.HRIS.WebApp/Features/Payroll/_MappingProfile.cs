using AutoMapper;
using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.WebApp.Features.Payroll
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PayrollProcessBatch, Search.QueryResult.PayrollProcessBatch>();
            CreateMap<Client, Search.QueryResult.Client>();
        }
    }
}
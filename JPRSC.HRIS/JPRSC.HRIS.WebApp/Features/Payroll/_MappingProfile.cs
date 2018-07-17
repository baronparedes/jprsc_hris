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

            CreateMap<PayrollProcessBatch, Report.QueryResult.PayrollProcessBatch>();
            CreateMap<PayrollRecord, Report.QueryResult.PayrollRecord>();
            CreateMap<Employee, Report.QueryResult.Employee>();
            CreateMap<Client, Report.QueryResult.Client>();
        }
    }
}
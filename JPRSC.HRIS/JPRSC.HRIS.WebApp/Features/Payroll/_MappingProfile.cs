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

            CreateMap<PayrollProcessBatch, EndProcess.QueryResult.PayrollProcessBatch>();

            CreateMap<PayrollProcessBatch, EndProcess.EndProcessQueryResult.PayrollProcessBatch>();

            CreateMap<PayrollProcessBatch, BankReport.QueryResult.PayrollProcessBatch>();
            CreateMap<PayrollRecord, BankReport.QueryResult.PayrollRecord>();
            CreateMap<Employee, BankReport.QueryResult.Employee>();
            CreateMap<Client, BankReport.QueryResult.Client>();
        }
    }
}
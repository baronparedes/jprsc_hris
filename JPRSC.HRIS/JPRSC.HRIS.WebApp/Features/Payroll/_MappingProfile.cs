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

            CreateMap<PayrollProcessBatch, PayrollReport.QueryResult.PayrollProcessBatch>();
            CreateMap<PayrollRecord, PayrollReport.QueryResult.PayrollRecord>();
            CreateMap<Employee, PayrollReport.QueryResult.Employee>();
            CreateMap<Client, PayrollReport.QueryResult.Client>();

            CreateMap<PayrollProcessBatch, EndProcess.QueryResult.PayrollProcessBatch>();

            CreateMap<PayrollProcessBatch, EndProcess.EndProcessQueryResult.PayrollProcessBatch>();
        }
    }
}
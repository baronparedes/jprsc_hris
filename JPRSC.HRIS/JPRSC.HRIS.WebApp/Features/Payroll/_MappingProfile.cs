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

            CreateMap<PayrollProcessBatch, BankReport.QueryResult.PayrollProcessBatch>();
            CreateMap<PayrollRecord, BankReport.QueryResult.PayrollRecord>();
            CreateMap<Employee, BankReport.QueryResult.Employee>();
            CreateMap<Client, BankReport.QueryResult.Client>();

            CreateMap<PayrollProcessBatch, PayslipReport.QueryResult.PayrollProcessBatch>();
            CreateMap<PayrollRecord, PayslipReport.QueryResult.PayrollRecord>();
            CreateMap<Employee, PayslipReport.QueryResult.Employee>();
            CreateMap<Client, PayslipReport.QueryResult.Client>();
            CreateMap<Department, PayslipReport.QueryResult.Department>();
        }
    }
}
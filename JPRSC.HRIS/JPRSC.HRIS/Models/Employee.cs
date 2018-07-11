using System;
using System.Collections.Generic;

namespace JPRSC.HRIS.Models
{
    public class Employee
    {
        public AccountType? AccountType { get; set; }
        public DateTime AddedOn { get; set; }
        public string ATMAccountNumber { get; set; }
        public string CelNo { get; set; }
        public Citizenship? Citizenship { get; set; }
        public string CityAddress { get; set; }
        public CivilStatus? CivilStatus { get; set; }
        public Client Client { get; set; }
        public int? ClientId { get; set; }
        public decimal? COLADaily { get; set; }
        public decimal? COLAHourly { get; set; }
        public Company Company { get; set; }
        public int? CompanyId { get; set; }
        public decimal? DailyRate { get; set; }
        public ICollection<DailyTimeRecord> DailyTimeRecords { get; set; } = new List<DailyTimeRecord>();
        public DateTime? DateHired { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateResigned { get; set; }
        public DateTime? DeletedOn { get; set; }
        public Department Department { get; set; }
        public int? DepartmentId { get; set; }
        public string Email { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeStatus { get; set; }
        public string FirstName { get; set; }
        public Gender? Gender { get; set; }
        public decimal? HourlyRate { get; set; }
        public int Id { get; set; }
        public string LastName { get; set; }
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
        public string MiddleName { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Nickname { get; set; }
        public ICollection<Overtime> Overtimes { get; set; } = new List<Overtime>();
        public string PagIbig { get; set; }
        public bool? PagIbigExempt { get; set; }
        public string PhilHealth { get; set; }
        public bool? PhilHealthExempt { get; set; }
        public string Position { get; set; }
        public Religion Religion { get; set; }
        public int? ReligionId { get; set; }
        public string ResignStatus { get; set; }
        public string SalaryStatus { get; set; }
        public string SSS { get; set; }
        public bool? TaxExempt { get; set; }
        public TaxStatus TaxStatus { get; set; }
        public int? TaxStatusId { get; set; }
        public string TelNo { get; set; }
        public bool? ThirteenthMonthExempt { get; set; }
        public string TIN { get; set; }
        public string ZipCode { get; set; }
    }
}
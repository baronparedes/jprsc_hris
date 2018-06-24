using System;

namespace JPRSC.HRIS.Models
{
    public class Client
    {
        public DateTime AddedOn { get; set; }
        public string Code { get; set; }
        public int? CurrentPayrollPeriod { get; set; }
        public int? DaysPerWeek { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string Description { get; set; }
        public int? HoursPerDay { get; set; }
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Name { get; set; }
        public int? NumberOfPayrollPeriodsAMonth { get; set; }
        public int? NumberOfWorkingDaysForThisPayrollPeriod { get; set; }
        public PayrollCode? PayrollCode { get; set; }
        public DateTime? PayrollPeriodFrom { get; set; }
        public Month? PayrollPeriodMonth { get; set; }
        public DateTime? PayrollPeriodTo { get; set; }
        public TaxTable? TaxTable { get; set; }
        public bool? ZeroBasic { get; set; }
    }
}
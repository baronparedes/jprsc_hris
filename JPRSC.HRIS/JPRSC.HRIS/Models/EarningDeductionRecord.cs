using System;

namespace JPRSC.HRIS.Models
{
    public class EarningDeductionRecord
    {
        public DateTime AddedOn { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? DeletedOn { get; set; }
        public EarningDeduction EarningDeduction { get; set; }
        public int? EarningDeductionId { get; set; }
        public Employee Employee { get; set; }
        public int? EmployeeId { get; set; }
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? PayrollPeriodFrom { get; set; }
        public Month? PayrollPeriodMonth { get; set; }
        public DateTime? PayrollPeriodTo { get; set; }
    }
}
using System;

namespace JPRSC.HRIS.Models
{
    public class PayrollRecord
    {
        public DateTime AddedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public Employee Employee { get; set; }
        public int? EmployeeId { get; set; }
        public PayrollProcessBatch PayrollProcessBatch { get; set; }
        public int? PayrollProcessBatchId { get; set; }

        public decimal? DaysWorkedValue { get; set; }
        public decimal? HoursWorkedValue { get; set; }
        public decimal? OvertimeValue { get; set; }
        public decimal? HoursLateValue { get; set; }
        public decimal? HoursUndertimeValue { get; set; }
        public decimal? COLADailyValue { get; set; }
        public decimal? EarningsValue { get; set; }
        public decimal? DeductionsValue { get; set; }

        public decimal? SSSValueEmployee { get; set; }
        public decimal? SSSValueEmployer { get; set; }
        public decimal? PHICValueEmployee { get; set; }
        public decimal? PHICValueEmployer { get; set; }
        public decimal? PagIbigValue { get; set; }
        public decimal? TaxValue { get; set; }

        public decimal? LoanPaymentValue { get; set; }
    }
}
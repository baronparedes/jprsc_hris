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
        public decimal? COLAHourlyValue { get; set; }
        public decimal? COLAMonthlyValue { get; set; }
        public decimal? EarningsValue { get; set; }
        public decimal? DeductionsValue { get; set; }

        public decimal? SSSValueEmployee { get; set; }
        public decimal? SSSValueEmployer { get; set; }
        public decimal? PHICValueEmployee { get; set; }
        public decimal? PHICValueEmployer { get; set; }
        public decimal? PagIbigValueEmployee { get; set; }
        public decimal? PagIbigValueEmployer { get; set; }
        public decimal? TaxValue { get; set; }

        public decimal? LoanPaymentValue { get; set; }

        public decimal? SSSDeductionBasis { get; set; }
        public decimal? PHICDeductionBasis { get; set; }
        public decimal? PagIbigDeductionBasis { get; set; }
        public bool GovDeductionsDeducted { get; set; }
        public bool LoansDeducted { get; set; }
        public bool AnythingDeducted { get; set; }
        public decimal DeductionBasis { get; set; }

        public decimal BasicPayValue => DaysWorkedValue.GetValueOrDefault() + HoursWorkedValue.GetValueOrDefault();
        public decimal TotalEarningsValue => BasicPayValue + OvertimeValue.GetValueOrDefault() - HoursUndertimeValue.GetValueOrDefault() - HoursLateValue.GetValueOrDefault() + COLADailyValue.GetValueOrDefault() + COLAHourlyValue.GetValueOrDefault() + COLAMonthlyValue.GetValueOrDefault() + EarningsValue.GetValueOrDefault();
        public decimal TotalGovDeductionsValue => SSSValueEmployee.GetValueOrDefault() + PagIbigValueEmployee.GetValueOrDefault() + PHICValueEmployee.GetValueOrDefault() + TaxValue.GetValueOrDefault();
        public decimal TotalDeductionsValue => TotalGovDeductionsValue + DeductionsValue.GetValueOrDefault() + LoanPaymentValue.GetValueOrDefault();
        public decimal NetPayValue { get; set; }
    }
}
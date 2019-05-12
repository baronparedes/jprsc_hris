namespace JPRSC.HRIS.Models
{
    public class LoanDeduction
    {
        public int Id { get; set; }
        public decimal? DeductionAmount { get; set; }
        public decimal? NewAmountPaid { get; set; }
        public decimal? NewRemainingBalance { get; set; }
        public Loan Loan { get; set; }
        public int? LoanId { get; set; }
        public PayrollRecord PayrollRecord { get; set; }
        public int? PayrollRecordId { get; set; }
    }
}
using System;

namespace JPRSC.HRIS.Models
{
    public class Loan
    {
        public DateTime AddedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public Employee Employee { get; set; }
        public int? EmployeeId { get; set; }
        public int Id { get; set; }
        public double? InterestRate { get; set; }
        public DateTime? LoanDate { get; set; }
        public LoanType LoanType { get; set; }
        public int? LoanTypeId { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? MonthsPayable { get; set; }
        public int? PayrollPeriod { get; set; }
        public decimal? PrincipalAmount { get; set; }
        public string TransactionNumber { get; set; }
        public DateTime? ZeroedOutOn { get; set; }
    }
}
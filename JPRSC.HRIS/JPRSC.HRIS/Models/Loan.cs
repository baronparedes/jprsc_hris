using System;
using System.Collections.Generic;
using System.Linq;

namespace JPRSC.HRIS.Models
{
    public class Loan
    {
        public DateTime AddedOn { get; set; }
        public decimal? DeductionAmount { get; set; }
        public DateTime? DeletedOn { get; set; }
        public Employee Employee { get; set; }
        public int? EmployeeId { get; set; }
        public int Id { get; set; }
        public decimal? InterestAmount { get; set; }
        public DateTime? LoanDate { get; set; }
        public string LoanPayrollPeriod { get; set; }
        public IList<int> LoanPayrollPeriods => String.IsNullOrWhiteSpace(LoanPayrollPeriod) ? new List<int>() : LoanPayrollPeriod.Split(',').Select(p => Convert.ToInt32(p)).ToList();
        public LoanType LoanType { get; set; }
        public int? LoanTypeId { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? MonthsPayable { get; set; }
        public decimal? PrincipalAmount { get; set; }
        public decimal? PrincipalAndInterestAmount { get; set; }
        public decimal? RemainingBalance { get; set; }
        public DateTime? StartDeductionDate { get; set; }
        public string TransactionNumber { get; set; }
        public DateTime? ZeroedOutOn { get; set; }
    }
}
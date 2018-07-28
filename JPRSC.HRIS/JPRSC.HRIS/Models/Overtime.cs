using System;

namespace JPRSC.HRIS.Models
{
    public class Overtime
    {
        public DateTime AddedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public Employee Employee { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? From { get; set; }
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public double? NumberOfHours { get; set; }
        public decimal? NumberOfHoursValue { get; set; }
        public PayPercentage PayPercentage { get; set; }
        public int? PayPercentageId { get; set; }
        public string PayPercentageName { get; set; }
        public double? PayPercentagePercentage { get; set; }
        public DateTime? PayrollPeriodFrom { get; set; }
        public DateTime? PayrollPeriodTo { get; set; }
        public string Reference { get; set; }
        public DateTime? To { get; set; }
    }
}
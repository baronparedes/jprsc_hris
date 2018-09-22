using System;

namespace JPRSC.HRIS.Models
{
    public class PhicRecord
    {
        public DateTime AddedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public double? EmployeePercentageShare { get; set; }
        public double? EmployerPercentageShare => EmployeePercentageShare.HasValue ? 100 - EmployeePercentageShare.Value : (double?)null;
        public int Id { get; set; }
        public decimal? MaximumDeduction { get; set; }
        public decimal? MinimumDeduction { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public double? Percentage { get; set; }
    }
}
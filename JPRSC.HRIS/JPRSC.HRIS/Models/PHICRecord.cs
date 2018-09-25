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

        public decimal PercentageForComputation => !Percentage.HasValue ? default(decimal) : (decimal)(Percentage.Value) / 100;
        public decimal EmployeePercentageShareForComputation => !EmployeePercentageShare.HasValue ? default(decimal) : (decimal)(EmployeePercentageShare.Value) / 100;
        public decimal EmployerPercentageShareForComputation => !EmployerPercentageShare.HasValue ? default(decimal) : (decimal)(EmployerPercentageShare.Value) / 100;
    }
}
using System;

namespace JPRSC.HRIS.Models
{
    public class PagIbigRecord
    {
        public DateTime AddedOn { get; set; }
        public bool? ApplyToSalary { get; set; }
        public string Code { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string Description { get; set; }
        public decimal? DeductionAmount { get; set; }
        public double? EmployeePercentage { get; set; }
        public double? EmployerPercentage { get; set; }
        public int Id { get; set; }
        public decimal? MinimumDeduction { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Name { get; set; }
    }
}
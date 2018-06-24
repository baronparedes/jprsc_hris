using System;

namespace JPRSC.HRIS.Models
{
    public class SSSRecord
    {
        public DateTime AddedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public decimal? ECC { get; set; }
        public decimal? Employee { get; set; }
        public decimal? Employer { get; set; }
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? Number { get; set; }
        public decimal? Range1 { get; set; }
        public decimal? Total => Employer + Employee;
    }
}
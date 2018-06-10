using System;

namespace JPRSC.HRIS.Models
{
    public class PayPercentage
    {
        public DateTime? ModifiedOn { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public double? Percentage { get; set; }
    }
}
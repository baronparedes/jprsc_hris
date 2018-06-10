using System;
using System.Collections.Generic;

namespace JPRSC.HRIS.Models
{
    public class TaxRecord
    {
        public DateTime AddedOn { get; set; }
        public string Code { get; set; }
        public DateTime? DeletedOn { get; set; }
        public double? Exemption { get; set; }
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Name { get; set; }
        public ICollection<TaxRange> TaxRanges { get; set; } = new List<TaxRange>();
    }
}
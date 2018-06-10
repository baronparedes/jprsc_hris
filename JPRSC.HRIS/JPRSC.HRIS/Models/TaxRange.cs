using System;

namespace JPRSC.HRIS.Models
{
    public class TaxRange
    {
        public DateTime AddedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public decimal? From { get; set; }
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public double? Percentage { get; set; }
        public decimal? Plus { get; set; }
        public TaxRecord TaxRecord { get; set; }
        public int? TaxRecordId { get; set; }
        public decimal? To { get; set; }
    }
}
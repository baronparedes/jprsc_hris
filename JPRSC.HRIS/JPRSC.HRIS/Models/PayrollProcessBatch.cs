using System;
using System.Collections.Generic;

namespace JPRSC.HRIS.Models
{
    public class PayrollProcessBatch
    {
        public DateTime AddedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public Client Client { get; set; }
        public int? ClientId { get; set; }
        public bool? DeductedSSS { get; set; }
        public bool? DeductedPHIC { get; set; }
        public bool? DeductedPagIbig { get; set; }
        public bool? DeductedTax { get; set; }
        public int? PayrollPeriod { get; set; }
        public DateTime? PayrollPeriodFrom { get; set; }
        public DateTime? PayrollPeriodTo { get; set; }
        public DateTime? DateOverwritten { get; set; }
        public Month? PayrollPeriodMonth { get; set; }
        public DateTime? EndProcessedOn { get; set; }

        public ICollection<PayrollRecord> PayrollRecords { get; set; } = new List<PayrollRecord>();
    }
}
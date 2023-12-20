using System;
using System.Collections.Generic;
using System.Linq;

namespace JPRSC.HRIS.Models
{
    public class ForProcessingBatch
    {
        public DateTime AddedOn { get; set; }
        public Client Client { get; set; }
        public int? ClientId { get; set; }
        public string DateFormatted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string EmployeeIds { get; set; }
        public IList<int> EmployeeIdsList => String.IsNullOrWhiteSpace(EmployeeIds) ? new List<int>() : EmployeeIds.Split(',').Select(id => Convert.ToInt32(id)).ToList();
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Name { get; set; }
        public DateTime? ProcessedOn { get; set; }

        public DateTime? PayrollPeriodFrom { get; set; }
        public Month? PayrollPeriodMonth { get; set; }
        public DateTime? PayrollPeriodTo { get; set; }
    }
}
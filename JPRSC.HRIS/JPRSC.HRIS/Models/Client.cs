using System;
using System.Collections.Generic;
using System.Linq;

namespace JPRSC.HRIS.Models
{
    public class Client
    {
        public DateTime AddedOn { get; set; }
        public string Code { get; set; }
        public int? CurrentPayrollPeriod { get; set; }
        public int? DaysPerWeek { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string Description { get; set; }
        public int? HoursPerDay { get; set; }
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Name { get; set; }
        public int? NumberOfPayrollPeriodsAMonth { get; set; }
        public int? NumberOfWorkingDaysForThisPayrollPeriod { get; set; }
        public bool? PagIbigBasic { get; set; }
        public bool? PagIbigCola { get; set; }
        public bool? PagIbigOvertime { get; set; }
        public string PagIbigPayrollPeriod { get; set; }
        public IEnumerable<int> PagIbigPayrollPeriods => String.IsNullOrWhiteSpace(PagIbigPayrollPeriod) ? new List<int>() : PagIbigPayrollPeriod.Split(',').Select(p => Convert.ToInt32(p));
        public PayrollCode? PayrollCode { get; set; }
        public DateTime? PayrollPeriodFrom { get; set; }
        public Month? PayrollPeriodMonth { get; set; }
        public DateTime? PayrollPeriodTo { get; set; }
        public bool? PHICBasic { get; set; }
        public bool? PHICCola { get; set; }
        public bool? PHICOvertime { get; set; }
        public string PHICPayrollPeriod { get; set; }
        public IEnumerable<int> PHICPayrollPeriods => String.IsNullOrWhiteSpace(PHICPayrollPeriod) ? new List<int>() : PHICPayrollPeriod.Split(',').Select(p => Convert.ToInt32(p));
        public bool? SSSBasic { get; set; }
        public bool? SSSCola { get; set; }
        public bool? SSSOvertime { get; set; }
        public string SSSPayrollPeriod { get; set; }
        public IEnumerable<int> SSSPayrollPeriods => String.IsNullOrWhiteSpace(SSSPayrollPeriod) ? new List<int>() : SSSPayrollPeriod.Split(',').Select(p => Convert.ToInt32(p));
        public bool? TaxBasic { get; set; }
        public bool? TaxCola { get; set; }
        public bool? TaxOvertime { get; set; }
        public string TaxPayrollPeriod { get; set; }
        public IEnumerable<int> TaxPayrollPeriods => String.IsNullOrWhiteSpace(TaxPayrollPeriod) ? new List<int>() : TaxPayrollPeriod.Split(',').Select(p => Convert.ToInt32(p));
        public TaxTable? TaxTable { get; set; }
        public bool? ZeroBasic { get; set; }

        public ICollection<PayrollProcessBatch> PayrollProcessBatches { get; set; } = new List<PayrollProcessBatch>();
    }
}
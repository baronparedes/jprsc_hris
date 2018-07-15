using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Models
{
    public class PayrollRecord
    {
        public DateTime AddedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public Employee Employee { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? PayrollPeriodFrom { get; set; }
        public DateTime? PayrollPeriodTo { get; set; }

        public decimal? DaysWorkedValue { get; set; }
        public decimal? HoursWorkedValue { get; set; }
        public decimal? OvertimeValue { get; set; }
        public decimal? HoursLateValue { get; set; }
        public decimal? HoursUndertimeValue { get; set; }
        public decimal? COLADailyValue { get; set; }
        public decimal? EarningsValue { get; set; }
        public decimal? DeductionsValue { get; set; }

        public bool? DeductedSSS { get; set; }
        public bool? DeductedPHIC { get; set; }
        public bool? DeductedPagIbig { get; set; }
        public bool? DeductedTax { get; set; }

        public decimal? SSSValueEmployee { get; set; }
        public decimal? SSSValueEmployer { get; set; }
        public decimal? PHICValue { get; set; }
        public decimal? PagIbigValue { get; set; }
        public decimal? TaxValue { get; set; }
    }
}

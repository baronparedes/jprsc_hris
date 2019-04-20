using System;

namespace JPRSC.HRIS.Models
{
    public class DailyTimeRecord
    {
        public DateTime AddedOn { get; set; }
        public decimal? COLADailyValue { get; set; }
        public decimal? COLAHourlyValue { get; set; }
        public decimal? COLAHourlyOTValue { get; set; }
        public decimal? COLAMonthlyValue { get; set; }
        public decimal? DailyRate { get; set; }
        public double? DaysWorked { get; set; }
        public decimal? DaysWorkedValue { get; set; }
        public DateTime? DeletedOn { get; set; }
        public Employee Employee { get; set; }
        public int? EmployeeId { get; set; }
        public decimal? HourlyRate { get; set; }
        public double? HoursLate { get; set; }
        public decimal? HoursLateValue { get; set; }
        public double? HoursUndertime { get; set; }
        public decimal? HoursUndertimeValue { get; set; }
        public double? HoursWorked { get; set; }
        public decimal? HoursWorkedValue { get; set; }
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public decimal? MonthlyRate { get; set; }
        public DateTime? PayrollPeriodFrom { get; set; }
        public Month? PayrollPeriodMonth { get; set; }
        public DateTime? PayrollPeriodTo { get; set; }

        public decimal TimeWorkedValue => DaysWorkedValue.GetValueOrDefault() + HoursWorkedValue.GetValueOrDefault();
        public decimal COLATotalValue => COLADailyValue.GetValueOrDefault() + COLAHourlyValue.GetValueOrDefault() + COLAHourlyOTValue.GetValueOrDefault() + COLAMonthlyValue.GetValueOrDefault();
        public decimal TimeNotWorkedValue => HoursLateValue.GetValueOrDefault() + HoursUndertimeValue.GetValueOrDefault();
    }
}
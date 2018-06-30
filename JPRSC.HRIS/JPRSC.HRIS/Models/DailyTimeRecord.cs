﻿using System;

namespace JPRSC.HRIS.Models
{
    public class DailyTimeRecord
    {
        public int Id { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public Employee Employee { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? PayrollPeriodFrom { get; set; }
        public DateTime? PayrollPeriodTo { get; set; }
        public double? DaysWorked { get; set; }
        public double? HoursWorked { get; set; }
        public double? HoursLate { get; set; }
        public double? HoursUndertime { get; set; }
        public decimal? DaysWorkedValue { get; set; }
        public decimal? HoursWorkedValue { get; set; }
        public decimal? HoursLateValue { get; set; }
        public decimal? HoursUndertimeValue { get; set; }
    }
}
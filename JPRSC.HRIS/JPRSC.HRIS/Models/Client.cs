using System;

namespace JPRSC.HRIS.Models
{
    public class Client
    {
        public DateTime AddedOn { get; set; }
        public CutOffPeriod? CutOffPeriod { get; set; }
        public int? DaysPerWeek { get; set; }
        public DateTime? DeletedOn { get; set; }
        public int? HoursPerDay { get; set; }
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Name { get; set; }
    }
}
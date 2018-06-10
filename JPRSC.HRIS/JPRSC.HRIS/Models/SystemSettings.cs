using System;

namespace JPRSC.HRIS.Models
{
    public class SystemSettings
    {
        public int Id { get; set; }
        public double? SSSRate { get; set; }
        public double? PHICRate { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
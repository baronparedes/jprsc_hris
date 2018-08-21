using System;

namespace JPRSC.HRIS.Models
{
    public class AuditTrailEntry
    {
        public string Action { get; set; }
        public DateTime AddedOn { get; set; }
        public int Id { get; set; }
        public string Module { get; set; }
        public int? RecordId { get; set; }
        public string UserName { get; set; }
    }
}
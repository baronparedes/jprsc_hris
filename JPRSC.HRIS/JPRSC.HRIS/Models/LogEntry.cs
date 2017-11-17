using System;

namespace JPRSC.HRIS.Models
{
    public class LogEntry
    {
        public string Action { get; set; }
        public string Controller { get; set; }
        public int Id { get; set; }
        public string Level { get; set; }
        public DateTime? LoggedOn { get; set; }
        public string Message { get; set; }
        public string Request { get; set; }
        public string StackTrace { get; set; }
        public string UserId { get; set; }
    }
}
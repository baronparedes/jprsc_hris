using System;

namespace JPRSC.HRIS.Models
{
    public class RehireTransferEvent
    {
        public DateTime AddedOn { get; set; }
        public DateTime RehireTransferDateLocal { get; set; }
        public int Id { get; set; }
        public Employee Employee { get; set; }
        public int? EmployeeId { get; set; }
        public Client Client { get; set; }
        public int? ClientId { get; set; }
    }
}
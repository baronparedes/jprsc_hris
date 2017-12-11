using System;

namespace JPRSC.HRIS.Models
{
    public class ApprovalLevel
    {
        public DateTime AddedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public int Id { get; set; }
        public int? Level { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
    }
}
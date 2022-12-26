using System;

namespace JPRSC.HRIS.Models
{
    public class BackgroundProcess
    {
        public DateTime AddedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public DateTime? DoneErrorOn { get; set; }
        public DateTime? DoneSuccessOn { get; set; }
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? StartedOn { get; set; }
        public BackgroundProcessStatus? Status { get; set; }
        public BackgroundProcessType? Type { get; set; }
    }
}

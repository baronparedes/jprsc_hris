using System.ComponentModel.DataAnnotations;

namespace JPRSC.HRIS.Models
{
    public enum BackgroundProcessStatus
    {
        [Display(Name = "Pending")]
        Pending = 10,

        [Display(Name = "Started")]
        Started = 20,

        [Display(Name = "Done - Success")]
        DoneSuccess = 30,

        [Display(Name = "Done - Error")]
        DoneError = 40
    }
}

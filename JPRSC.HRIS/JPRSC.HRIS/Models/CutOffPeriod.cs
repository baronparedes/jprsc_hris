using System.ComponentModel.DataAnnotations;

namespace JPRSC.HRIS.Models
{
    public enum CutOffPeriod
    {
        [Display(Name = "Daily")]
        Daily = 10,

        [Display(Name = "Bi-Monthly")]
        BiMonthly = 20,

        [Display(Name = "Monthly")]
        Monthly = 30
    }
}
using System.ComponentModel.DataAnnotations;

namespace JPRSC.HRIS.Models
{
    public enum PayrollCode
    {
        [Display(Name = "Daily")]
        Daily = 10,

        [Display(Name = "Semi-Monthly")]
        SemiMonthly = 20,

        [Display(Name = "Monthly")]
        Monthly = 30
    }
}
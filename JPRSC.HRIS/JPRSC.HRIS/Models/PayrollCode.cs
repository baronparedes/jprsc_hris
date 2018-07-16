using System.ComponentModel.DataAnnotations;

namespace JPRSC.HRIS.Models
{
    public enum PayrollCode
    {
        [Display(Name = "Weekly")]
        Weekly = 10,

        [Display(Name = "Semi-Monthly")]
        SemiMonthly = 20,

        [Display(Name = "Monthly")]
        Monthly = 30
    }
}
using System.ComponentModel.DataAnnotations;

namespace JPRSC.HRIS.Models
{
    public enum TaxTable
    {
        [Display(Name = "Weekly")]
        Weekly = 10,

        [Display(Name = "Semi-Monthly")]
        SemiMonthly = 20,

        [Display(Name = "Monthly")]
        Monthly = 30,

        [Display(Name = "Monthly")]
        Yearly = 40
    }
}
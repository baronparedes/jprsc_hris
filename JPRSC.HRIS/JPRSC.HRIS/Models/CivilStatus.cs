using System.ComponentModel.DataAnnotations;

namespace JPRSC.HRIS.Models
{
    public enum CivilStatus
    {
        [Display(Name = "Single")]
        Single = 10,

        [Display(Name = "Married")]
        Married = 20,

        [Display(Name = "Divorced")]
        Divorced = 30,

        [Display(Name = "Widowed")]
        Widowed = 40,

        [Display(Name = "Separated")]
        Separated
    }
}
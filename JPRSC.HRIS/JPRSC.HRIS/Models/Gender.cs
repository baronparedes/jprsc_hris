using System.ComponentModel.DataAnnotations;

namespace JPRSC.HRIS.Models
{
    public enum Gender
    {
        [Display(Name = "Male")]
        Male = 10,

        [Display(Name = "Female")]
        Female = 20
    }
}
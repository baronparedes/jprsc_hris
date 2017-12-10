using System.ComponentModel.DataAnnotations;

namespace JPRSC.HRIS.Models
{
    public enum AccountType
    {
        [Display(Name = "Savings")]
        Savings = 10,

        [Display(Name = "Current")]
        Current = 20,

        [Display(Name = "Checking")]
        Checking = 30
    }
}
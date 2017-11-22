using System.ComponentModel.DataAnnotations;

namespace JPRSC.HRIS.Models
{
    public enum EarningDeductionType
    {
        [Display(Name = "Earnings")]
        Earnings = 10,

        [Display(Name = "Deductions")]
        Deductions = 20
    }
}
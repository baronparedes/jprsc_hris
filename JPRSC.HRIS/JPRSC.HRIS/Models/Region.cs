using System.ComponentModel.DataAnnotations;

namespace JPRSC.HRIS.Models
{
    public enum Region
    {
        [Display(Name = "NCR")]
        NCR = 10,

        [Display(Name = "ARMM")]
        ARMM = 20,

        [Display(Name = "CAR")]
        CAR = 30,

        [Display(Name = "I")]
        Region1 = 40,

        [Display(Name = "II")]
        Region2 = 50,

        [Display(Name = "III")]
        Region3 = 60,

        [Display(Name = "IV-A")]
        Region4A = 70,

        [Display(Name = "IV-B")]
        Region4B = 80,

        [Display(Name = "V")]
        Region5 = 90,

        [Display(Name = "VI")]
        Region6 = 100,

        [Display(Name = "VII")]
        Region7 = 110,

        [Display(Name = "VIII")]
        Region8 = 120,

        [Display(Name = "IX")]
        Region9 = 130,

        [Display(Name = "X")]
        Region10 = 140,

        [Display(Name = "XI")]
        Region11 = 150,

        [Display(Name = "XII")]
        Region12 = 160,

        [Display(Name = "XIII")]
        Region13 = 170
    }
}
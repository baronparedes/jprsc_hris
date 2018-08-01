using System.ComponentModel.DataAnnotations;

namespace JPRSC.HRIS.Models
{
    public enum Region
    {
        [Display(Name = "NCR (National Capital Region)")]
        NCR = 10,

        [Display(Name = "ARMM (Autonomous Region in Muslim Mindanao)")]
        ARMM = 20,

        [Display(Name = "CAR (Cordillera Administrative Region)")]
        CAR = 30,

        [Display(Name = "Region 1 (Ilocos Region)")]
        Region1 = 40,

        [Display(Name = "Region 2 (Cagayan Valley)")]
        Region2 = 50,

        [Display(Name = "Region 3 (Central Luzon)")]
        Region3 = 60,

        [Display(Name = "Region 4A (CALABARZON)")]
        Region4A = 70,

        [Display(Name = "Region 4B (MIMAROPA)")]
        Region4B = 80,

        [Display(Name = "Region 5 (Bicol Region)")]
        Region5 = 90,

        [Display(Name = "Region 6 (Western Visayas)")]
        Region6 = 100,

        [Display(Name = "Region 7 (Central Visayas)")]
        Region7 = 110,

        [Display(Name = "Region 8 (Eastern Visayas)")]
        Region8 = 120,

        [Display(Name = "Region 9 (Zamboanga Peninsula)")]
        Region9 = 130,

        [Display(Name = "Region 10 (Northern Mindanao)")]
        Region10 = 140,

        [Display(Name = "Region 11 (Davao Region)")]
        Region11 = 150,

        [Display(Name = "Region 12 (SOCCSKSARGEN)")]
        Region12 = 160,

        [Display(Name = "Region 13 (Caraga Region)")]
        Region13 = 170
    }
}
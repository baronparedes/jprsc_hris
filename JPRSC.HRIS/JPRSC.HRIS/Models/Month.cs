using System.ComponentModel.DataAnnotations;

namespace JPRSC.HRIS.Models
{
    public enum Month
    {
        [Display(Name = "January")]
        January = 10,

        [Display(Name = "February")]
        February = 20,

        [Display(Name = "March")]
        March = 30,

        [Display(Name = "April")]
        April = 40,

        [Display(Name = "May")]
        May = 50,

        [Display(Name = "June")]
        June = 60,

        [Display(Name = "July")]
        July = 70,

        [Display(Name = "August")]
        August = 80,

        [Display(Name = "September")]
        September = 90,

        [Display(Name = "October")]
        October = 100,

        [Display(Name = "November")]
        November = 110,

        [Display(Name = "December")]
        December = 120
    }
}
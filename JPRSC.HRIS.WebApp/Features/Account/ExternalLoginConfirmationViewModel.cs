using System.ComponentModel.DataAnnotations;

namespace JPRSC.HRIS.WebApp.Features.Account
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
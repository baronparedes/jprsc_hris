using System.ComponentModel.DataAnnotations;

namespace JPRSC.HRIS.Models
{
    public enum Permission
    {
        // Home
        [Display(Name = "Home")]
        HomeDefault = 100,

        // Company
        [Display(Name = "Companies")]
        CompanyDefault = 200,

        // Account
        [Display(Name = "Accounts")]
        AccountDefault = 300,

        // Religion
        [Display(Name = "Religions")]
        ReligionDefault = 400,

        // Client
        [Display(Name = "Clients")]
        ClientDefault = 500,

        // Custom Role
        [Display(Name = "Roles")]
        CustomRoleDefault = 600
    }
}
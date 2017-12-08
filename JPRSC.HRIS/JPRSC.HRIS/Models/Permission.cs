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
        AccountEditOwn = 301,

        // Religion
        [Display(Name = "Religions")]
        ReligionDefault = 400,

        // Client
        [Display(Name = "Clients")]
        ClientDefault = 500,

        // Custom Role
        [Display(Name = "Roles")]
        CustomRoleDefault = 600,

        // Earnings and Deductions
        [Display(Name = "Earnings and Deductions")]
        EarningDeductionDefault = 700,

        // Job Title
        [Display(Name = "Job Title")]
        JobTitleDefault = 800
    }
}
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
        [Display(Name = "Job Titles")]
        JobTitleDefault = 800,

        // Department
        [Display(Name = "Departments")]
        DepartmentDefault = 900,

        // Tax Status
        [Display(Name = "Tax Statuses")]
        TaxStatusDefault = 1000,

        [Display(Name = "Employees")]
        EmployeeDefault = 1100,

        [Display(Name = "Approval Levels")]
        ApprovalLevelDefault = 1200
    }
}
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

        [Display(Name = "Edit Own")]
        AccountEditOwn = 301,

        // Religion
        [Display(Name = "Religions")]
        ReligionDefault = 400,

        // Client
        [Display(Name = "Clients")]
        ClientDefault = 500,

        [Display(Name = "Add")]
        ClientAdd = 501,

        [Display(Name = "Edit")]
        ClientEdit = 502,

        [Display(Name = "Delete")]
        ClientDelete = 503,

        // Custom Role
        [Display(Name = "Roles")]
        CustomRoleDefault = 600,

        // Earnings and Deductions
        [Display(Name = "Earnings and Deductions")]
        EarningDeductionDefault = 700,

        // Job Title
        [Display(Name = "Job Titles")]
        JobTitleDefault = 800,

        // Branch, formerly known as Department
        [Display(Name = "Branches")]
        BranchDefault = 900,

        // Tax Status
        [Display(Name = "Tax Statuses")]
        TaxStatusDefault = 1000,

        // Employees
        [Display(Name = "Employees")]
        EmployeeDefault = 1100,

        [Display(Name = "Edit ATM")]
        EmployeeEditATM = 1101,

        [Display(Name = "Add")]
        EmployeeAdd = 1102,

        [Display(Name = "Edit")]
        EmployeeEdit = 1103,

        [Display(Name = "Delete")]
        EmployeeDelete = 1104,

        // Approval Levels
        [Display(Name = "Approval Levels")]
        ApprovalLevelDefault = 1200,

        // PHIC Record
        [Display(Name = "PHIC Records")]
        PhicRecordDefault = 1300,

        // Pag Ibig Record
        [Display(Name = "Pag Ibig Records")]
        PagIbigRecordDefault = 1400,

        // Bank
        [Display(Name = "Banks")]
        BankDefault = 1500,

        // Pay Percentage
        [Display(Name = "Pay Percentages")]
        PayPercentageDefault = 1600,

        // SSS Record
        [Display(Name = "SSS Records")]
        SSSRecordDefault = 1700,

        // Employee Rate
        [Display(Name = "Employee Rates")]
        EmployeeRateDefault = 1800,

        // Loan Type
        [Display(Name = "Loan Types")]
        LoanTypeDefault = 1900,

        // Loan
        [Display(Name = "Loans")]
        LoanDefault = 2000,

        [Display(Name = "Add")]
        LoanAdd = 2001,

        [Display(Name = "Zero Out")]
        LoanZeroOut = 2002,

        [Display(Name = "Details")]
        LoanDetails = 2003,

        // Daily Time Record
        [Display(Name = "Daily Time Records")]
        DailyTimeRecordDefault = 2100,

        // Overtime
        [Display(Name = "Overtimes")]
        OvertimeDefault = 2200,

        // Payroll
        [Display(Name = "Payroll")]
        PayrollDefault = 2300,

        [Display(Name = "Process")]
        PayrollProcess = 2301,

        [Display(Name = "Delete")]
        PayrollDelete = 2302,

        [Display(Name = "End Process")]
        PayrollEndProcess = 2303,

        // Earnings and Deductions Records
        [Display(Name = "Earnings and Deductions Records")]
        EarningDeductionRecordDefault = 2400,

        // Reports
        [Display(Name = "Reports")]
        ReportsDefault = 2500,

        // Audit trail
        [Display(Name = "Audit Trails")]
        AuditTrailDefault = 2600
    }
}
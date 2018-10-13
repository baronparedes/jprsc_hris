using JPRSC.HRIS.Models;
using System;
using System.Collections.Generic;

namespace JPRSC.HRIS.Infrastructure.Data.Seed
{
    public class CustomRoleSeed
    {
        internal const int SuperAdminCustomRoleId = 1;
        internal const int DefaultCustomRoleId = 2;

        internal static CustomRole[] CustomRoles()
        {
            var superAdmin = new CustomRole
            {
                AddedOn = DateTime.UtcNow,
                Id = SuperAdminCustomRoleId,
                Name = "Super Admin"
            };

            superAdmin.AddPermissions(new List<Permission>
            {
                Permission.HomeDefault,
                Permission.CompanyDefault,
                Permission.AccountDefault,
                Permission.AccountEditOwn,
                Permission.ReligionDefault,
                Permission.ClientDefault,
                Permission.ClientAdd,
                Permission.ClientEdit,
                Permission.ClientDelete,
                Permission.CustomRoleDefault,
                Permission.EarningDeductionDefault,
                Permission.JobTitleDefault,
                Permission.BranchDefault,
                Permission.TaxStatusDefault,
                Permission.EmployeeDefault,
                Permission.EmployeeEditATM,
                Permission.EmployeeAdd,
                Permission.EmployeeEdit,
                Permission.EmployeeDelete,
                Permission.ApprovalLevelDefault,
                Permission.PhicRecordDefault,
                Permission.PagIbigRecordDefault,
                Permission.BankDefault,
                Permission.PayPercentageDefault,
                Permission.SSSRecordDefault,
                Permission.EmployeeRateDefault,
                Permission.LoanTypeDefault,
                Permission.LoanDefault,
                Permission.LoanAdd,
                Permission.LoanZeroOut,
                Permission.LoanDetails,
                Permission.DailyTimeRecordDefault,
                Permission.OvertimeDefault,
                Permission.PayrollDefault,
                Permission.PayrollProcess,
                Permission.PayrollDelete,
                Permission.PayrollEndProcess,
                Permission.EarningDeductionRecordDefault,
                Permission.ReportsDefault,
                Permission.AuditTrailDefault,
                Permission.SystemSettingsDefault,
                Permission.ReportsModuleDefault
            });

            var defaultRole = new CustomRole
            {
                AddedOn = DateTime.UtcNow,
                Id = DefaultCustomRoleId,
                Name = "Default"
            };

            defaultRole.AddPermissions(new List<Permission>
            {
                Permission.HomeDefault,
                Permission.AccountEditOwn
            });

            return new List<CustomRole>
            {
                superAdmin,
                defaultRole
            }
            .ToArray();
        }
    }
}
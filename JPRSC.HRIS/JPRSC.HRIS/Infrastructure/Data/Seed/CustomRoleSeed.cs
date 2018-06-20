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
                Permission.CustomRoleDefault,
                Permission.EarningDeductionDefault,
                Permission.JobTitleDefault,
                Permission.DepartmentDefault,
                Permission.TaxStatusDefault,
                Permission.EmployeeDefault,
                Permission.ApprovalLevelDefault,
                Permission.PhicRecordDefault,
                Permission.PagIbigRecordDefault,
                Permission.BankDefault,
                Permission.PayPercentageDefault,
                Permission.SSSRecordDefault,
                Permission.EmployeeRateDefault
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
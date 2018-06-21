namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using JPRSC.HRIS.Infrastructure.Configuration;
    using JPRSC.HRIS.Infrastructure.Data.Seed;
    using JPRSC.HRIS.Infrastructure.Identity;
    using JPRSC.HRIS.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Infrastructure\Data\Migrations";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            if (!AppSettings.Bool("ShouldSeedDummyData")) return;

            // Order matters!
            SeedCompanies(context);
            SeedJobTitles(context);
            SeedDepartments(context);
            SeedReligions(context);
            SeedTaxStatuses(context);
            SeedClients(context);
            SeedCustomRoles(context);
            SeedEarningDeductions(context);
            SeedLoanTypes(context);
            SeedPagIbigRecords(context);
            SeedPHICRecords(context);
            SeedBanks(context);
            SeedPayPercentages(context);
            SeedSSSRecords(context);
            context.SaveChanges();

            SeedUsers(context);
            SeedApprovalLevels(context);
            SeedUserCustomRoles(context);
            SeedAllowedUserCompanies(context);
            SeedEmployees(context);
        }

        private void SeedBanks(ApplicationDbContext context)
        {
            context.Banks.AddOrUpdate(b => b.Id, BankSeed.Banks);
        }

        private static void SeedAllowedUserCompanies(ApplicationDbContext context)
        {
            for (var i = 0; i < UserSeed.DefaultUsers.Length; i++)
            {
                var userId = UserSeed.DefaultUsers[i].Id;
                var user = context.Users.Include(u => u.AllowedCompanies).Single(u => u.Id == userId);

                var companyId = CompanySeed.Companies[i].Id;

                if (!user.AllowedCompanies.Any(c => c.Id == companyId))
                {
                    var company = context.Companies.Single(c => c.Id == companyId);
                    user.AllowedCompanies.Add(company);
                }
            }
        }

        private static void SeedApprovalLevels(ApplicationDbContext context)
        {
            context.ApprovalLevels.AddOrUpdate(al => al.Id, ApprovalLevelSeed.ApprovalLevels(context));
        }

        private static void SeedClients(ApplicationDbContext context)
        {
            context.Clients.AddOrUpdate(c => c.Id, ClientSeed.Clients);
        }

        private static void SeedCompanies(ApplicationDbContext context)
        {
            context.Companies.AddOrUpdate(c => c.Id, CompanySeed.Companies);
        }

        private static void SeedCustomRoles(ApplicationDbContext context)
        {
            context.CustomRoles.AddOrUpdate(cr => cr.Id, CustomRoleSeed.CustomRoles());
        }

        private static void SeedDepartments(ApplicationDbContext context)
        {
            context.Departments.AddOrUpdate(d => d.Id, DepartmentSeed.Departments);
        }

        private static void SeedEarningDeductions(ApplicationDbContext context)
        {
            context.EarningDeductions.AddOrUpdate(ed => ed.Id, EarningDeductionSeed.EarningDeductions);
        }

        private void SeedLoanTypes(ApplicationDbContext context)
        {
            context.LoanTypes.AddOrUpdate(lt => lt.Id, LoanTypeSeed.LoanTypes);
        }

        private static void SeedEmployees(ApplicationDbContext context)
        {
            context.Employees.AddOrUpdate(e => e.Id, EmployeeSeed.Employees);
        }

        private static void SeedJobTitles(ApplicationDbContext context)
        {
            context.JobTitles.AddOrUpdate(jt => jt.Id, JobTitleSeed.JobTitles);
        }

        private void SeedPagIbigRecords(ApplicationDbContext context)
        {
            context.PagIbigRecords.AddOrUpdate(pir => pir.Id, PagIbigRecordSeed.PagIbigRecords);
        }

        private void SeedPayPercentages(ApplicationDbContext context)
        {
            var existingPayPercentages = context.PayPercentages.ToList();
            if (existingPayPercentages.Any()) return;

            context.PayPercentages.AddRange(PayPercentageSeed.PayPercentages);
        }

        private void SeedPHICRecords(ApplicationDbContext context)
        {
            var existingPhicRecords = context.PhicRecords.ToList();
            if (existingPhicRecords.Any()) return;

            context.PhicRecords.AddRange(PHICRecordSeed.PHICRecords);
        }

        private static void SeedReligions(ApplicationDbContext context)
        {
            context.Religions.AddOrUpdate(r => r.Id, ReligionSeed.Religions);
        }

        private void SeedSSSRecords(ApplicationDbContext context)
        {
            if (context.SSSRecords.Any()) return;

            var sssRecords = new List<SSSRecord>();
            for (var i = 0; i < 60; i++)
            {
                sssRecords.Add(new SSSRecord
                {
                    AddedOn = new DateTime(2017, 9, 1),
                    Id = i + 1,
                    Number = i + 1
                });
            }

            context.SSSRecords.AddRange(sssRecords);
        }

        private static void SeedTaxStatuses(ApplicationDbContext context)
        {
            context.TaxStatuses.AddOrUpdate(ts => ts.Id, TaxStatusSeed.TaxStatuses);
        }

        private static void SeedUserCustomRoles(ApplicationDbContext context)
        {
            var superAdminCustomRole = context.CustomRoles.Single(cr => cr.Id == CustomRoleSeed.SuperAdminCustomRoleId);
            var adminUsernames = UserSeed.Admins.Select(u => u.UserName).ToList();
            var adminUsers = context.Users.Include(u => u.CustomRoles).Where(u => adminUsernames.Contains(u.UserName));

            foreach (var adminUser in adminUsers)
            {
                if (!adminUser.CustomRoles.Any(cr => cr.Id == superAdminCustomRole.Id))
                {
                    adminUser.CustomRoles.Add(superAdminCustomRole);
                }
            }

            var defaultCustomRole = context.CustomRoles.Single(cr => cr.Id == CustomRoleSeed.DefaultCustomRoleId);
            var defaultUsernames = UserSeed.DefaultUsers.Select(u => u.UserName).ToList();
            var defaultUsers = context.Users.Include(u => u.CustomRoles).Where(u => defaultUsernames.Contains(u.UserName));

            foreach (var defaultUser in defaultUsers)
            {
                if (!defaultUser.CustomRoles.Any(cr => cr.Id == defaultCustomRole.Id))
                {
                    defaultUser.CustomRoles.Add(defaultCustomRole);
                }
            }
        }

        private static void SeedUsers(ApplicationDbContext context)
        {
            var userManager = new UserManager(new UserStore<User>(context));
            var commonPassword = AppSettings.String("DefaultPassword");

            var allUsers = new List<User>();
            allUsers.AddRange(UserSeed.Admins);
            allUsers.AddRange(UserSeed.DefaultUsers);

            foreach (var user in allUsers)
            {
                var existingUser = userManager.FindById(user.Id);

                if (existingUser != null)
                {
                    userManager.Update(existingUser);
                }
                else
                {
                    userManager.Create(user, commonPassword);
                }
            }
        }
    }
}
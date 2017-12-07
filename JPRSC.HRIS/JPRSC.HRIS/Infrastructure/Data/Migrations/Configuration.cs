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
            SeedUsers(context);
            SeedCustomRoles(context);
            context.SaveChanges();

            SeedUserCustomRoles(context);
        }

        private static void SeedCustomRoles(ApplicationDbContext context)
        {
            var superAdmin = new CustomRole
            {
                AddedOn = DateTime.UtcNow,
                Id = 1,
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
                Permission.EarningDeductionDefault
            });

            var defaultRole = new CustomRole
            {
                AddedOn = DateTime.UtcNow,
                Id = 2,
                Name = "Default"
            };

            defaultRole.AddPermissions(new List<Permission>
            {
                Permission.HomeDefault,
                Permission.AccountEditOwn
            });

            var customRolesSeed = new List<CustomRole>
            {
                superAdmin,
                defaultRole
            }
            .ToArray();

            context.CustomRoles.AddOrUpdate(cr => cr.Id, customRolesSeed);
        }

        private static void SeedUserCustomRoles(ApplicationDbContext context)
        {
            var superAdminCustomRoleId = 1;
            var admin01 = context.Users.Include(u => u.CustomRoles).Single(u => u.UserName == "admin01@email.com");

            if (!admin01.CustomRoles.Any(cr => cr.Id == superAdminCustomRoleId))
            {
                var superAdmin = context.CustomRoles.Single(cr => cr.Id == superAdminCustomRoleId);
                admin01.CustomRoles.Add(superAdmin);
            }
        }

        private void SeedUsers(ApplicationDbContext context)
        {
            var userManager = new UserManager(new UserStore<User>(context));
            var commonPassword = AppSettings.String("DefaultPassword");

            var allUsers = new List<User>();
            allUsers.AddRange(UserSeed.Admins);

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
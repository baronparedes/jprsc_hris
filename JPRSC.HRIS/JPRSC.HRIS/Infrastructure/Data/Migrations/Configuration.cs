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
    using System.Data.Entity.Migrations;

    public sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public ApplicationDbContext _db { get; private set; }

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Infrastructure\Data\Migrations";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            _db = context;

            SeedUsers();
        }

        private void SeedUsers()
        {
            var userManager = new UserManager(new UserStore<User>(_db));
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
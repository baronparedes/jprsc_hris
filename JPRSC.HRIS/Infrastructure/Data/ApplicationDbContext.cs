using Microsoft.AspNet.Identity.EntityFramework;
using JPRSC.HRIS.Models;
using System.Data.Entity;

namespace JPRSC.HRIS.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext() : base("ApplicationDbContext", false)
        {
        }

        public DbSet<CompanyProfile> CompanyProfiles { get; set; }
        public DbSet<CustomRole> CustomRoles { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.AddFromAssembly(typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
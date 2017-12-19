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

        public DbSet<ApprovalLevel> ApprovalLevels { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CustomRole> CustomRoles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<EarningDeduction> EarningDeductions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<Religion> Religions { get; set; }
        public DbSet<TaxStatus> TaxStatuses { get; set; }

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
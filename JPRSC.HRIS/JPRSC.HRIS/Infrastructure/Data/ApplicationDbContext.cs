﻿using Microsoft.AspNet.Identity.EntityFramework;
using JPRSC.HRIS.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace JPRSC.HRIS.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext() : base("ApplicationDbContext", false)
        {
        }

        public DbSet<ApprovalLevel> ApprovalLevels { get; set; }
        public DbSet<AuditTrailEntry> AuditTrailEntries { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CustomRole> CustomRoles { get; set; }
        public DbSet<DailyTimeRecord> DailyTimeRecords { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<EarningDeductionRecord> EarningDeductionRecords { get; set; }
        public DbSet<EarningDeduction> EarningDeductions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<ForProcessingBatch> ForProcessingBatches { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<LoanDeduction> LoanDeductions { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanType> LoanTypes { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<Overtime> Overtimes { get; set; }
        public DbSet<PagIbigRecord> PagIbigRecords { get; set; }
        public DbSet<PayPercentage> PayPercentages { get; set; }
        public DbSet<PayrollProcessBatch> PayrollProcessBatches { get; set; }
        public DbSet<PayrollRecord> PayrollRecords { get; set; }
        public DbSet<PhicRecord> PhicRecords { get; set; }
        public DbSet<RehireTransferEvent> RehireTransferEvents { get; set; }
        public DbSet<Religion> Religions { get; set; }
        public DbSet<SSSRecord> SSSRecords { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }
        public DbSet<TaxRange> TaxRanges { get; set; }
        public DbSet<TaxStatus> TaxStatuses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.AddFromAssembly(typeof(ApplicationDbContext).Assembly);

            modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
            modelBuilder.Conventions.Add(new DecimalPropertyConvention(18, 4));

            base.OnModelCreating(modelBuilder);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
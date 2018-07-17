namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPayrollProcessBatchPayrollPeriod : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayrollProcessBatches", "PayrollPeriodFrom", c => c.DateTime());
            AddColumn("dbo.PayrollProcessBatches", "PayrollPeriodTo", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PayrollProcessBatches", "PayrollPeriodTo");
            DropColumn("dbo.PayrollProcessBatches", "PayrollPeriodFrom");
        }
    }
}

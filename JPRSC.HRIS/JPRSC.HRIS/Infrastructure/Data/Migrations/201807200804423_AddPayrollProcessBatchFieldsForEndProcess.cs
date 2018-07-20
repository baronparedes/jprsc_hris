namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPayrollProcessBatchFieldsForEndProcess : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayrollProcessBatches", "PayrollPeriodMonth", c => c.Int());
            AddColumn("dbo.PayrollProcessBatches", "EndProcessedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PayrollProcessBatches", "EndProcessedOn");
            DropColumn("dbo.PayrollProcessBatches", "PayrollPeriodMonth");
        }
    }
}

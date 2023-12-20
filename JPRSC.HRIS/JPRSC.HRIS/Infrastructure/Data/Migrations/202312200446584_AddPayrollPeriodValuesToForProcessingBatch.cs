namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPayrollPeriodValuesToForProcessingBatch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ForProcessingBatches", "PayrollPeriodFrom", c => c.DateTime());
            AddColumn("dbo.ForProcessingBatches", "PayrollPeriodMonth", c => c.Int());
            AddColumn("dbo.ForProcessingBatches", "PayrollPeriodTo", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ForProcessingBatches", "PayrollPeriodTo");
            DropColumn("dbo.ForProcessingBatches", "PayrollPeriodMonth");
            DropColumn("dbo.ForProcessingBatches", "PayrollPeriodFrom");
        }
    }
}

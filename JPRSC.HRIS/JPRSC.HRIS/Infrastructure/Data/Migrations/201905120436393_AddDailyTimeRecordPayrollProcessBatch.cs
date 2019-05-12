namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDailyTimeRecordPayrollProcessBatch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DailyTimeRecords", "PayrollProcessBatchId", c => c.Int());
            CreateIndex("dbo.DailyTimeRecords", "PayrollProcessBatchId");
            AddForeignKey("dbo.DailyTimeRecords", "PayrollProcessBatchId", "dbo.PayrollProcessBatches", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DailyTimeRecords", "PayrollProcessBatchId", "dbo.PayrollProcessBatches");
            DropIndex("dbo.DailyTimeRecords", new[] { "PayrollProcessBatchId" });
            DropColumn("dbo.DailyTimeRecords", "PayrollProcessBatchId");
        }
    }
}

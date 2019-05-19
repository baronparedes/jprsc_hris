namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPayrollProcessBatchIdToOvertimeAndEDRRecord : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Overtimes", "PayrollProcessBatchId", c => c.Int());
            AddColumn("dbo.EarningDeductionRecords", "PayrollProcessBatchId", c => c.Int());
            CreateIndex("dbo.Overtimes", "PayrollProcessBatchId");
            CreateIndex("dbo.EarningDeductionRecords", "PayrollProcessBatchId");
            AddForeignKey("dbo.Overtimes", "PayrollProcessBatchId", "dbo.PayrollProcessBatches", "Id");
            AddForeignKey("dbo.EarningDeductionRecords", "PayrollProcessBatchId", "dbo.PayrollProcessBatches", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EarningDeductionRecords", "PayrollProcessBatchId", "dbo.PayrollProcessBatches");
            DropForeignKey("dbo.Overtimes", "PayrollProcessBatchId", "dbo.PayrollProcessBatches");
            DropIndex("dbo.EarningDeductionRecords", new[] { "PayrollProcessBatchId" });
            DropIndex("dbo.Overtimes", new[] { "PayrollProcessBatchId" });
            DropColumn("dbo.EarningDeductionRecords", "PayrollProcessBatchId");
            DropColumn("dbo.Overtimes", "PayrollProcessBatchId");
        }
    }
}

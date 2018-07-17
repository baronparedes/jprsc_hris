namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoveMoreFieldsToPayrollPeriodBatch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayrollProcessBatches", "DeductedSSS", c => c.Boolean());
            AddColumn("dbo.PayrollProcessBatches", "DeductedPHIC", c => c.Boolean());
            AddColumn("dbo.PayrollProcessBatches", "DeductedPagIbig", c => c.Boolean());
            AddColumn("dbo.PayrollProcessBatches", "DeductedTax", c => c.Boolean());
            AddColumn("dbo.PayrollProcessBatches", "PayrollPeriod", c => c.Int());
            DropColumn("dbo.PayrollRecords", "PayrollPeriod");
            DropColumn("dbo.PayrollRecords", "DeductedSSS");
            DropColumn("dbo.PayrollRecords", "DeductedPHIC");
            DropColumn("dbo.PayrollRecords", "DeductedPagIbig");
            DropColumn("dbo.PayrollRecords", "DeductedTax");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PayrollRecords", "DeductedTax", c => c.Boolean());
            AddColumn("dbo.PayrollRecords", "DeductedPagIbig", c => c.Boolean());
            AddColumn("dbo.PayrollRecords", "DeductedPHIC", c => c.Boolean());
            AddColumn("dbo.PayrollRecords", "DeductedSSS", c => c.Boolean());
            AddColumn("dbo.PayrollRecords", "PayrollPeriod", c => c.Int());
            DropColumn("dbo.PayrollProcessBatches", "PayrollPeriod");
            DropColumn("dbo.PayrollProcessBatches", "DeductedTax");
            DropColumn("dbo.PayrollProcessBatches", "DeductedPagIbig");
            DropColumn("dbo.PayrollProcessBatches", "DeductedPHIC");
            DropColumn("dbo.PayrollProcessBatches", "DeductedSSS");
        }
    }
}

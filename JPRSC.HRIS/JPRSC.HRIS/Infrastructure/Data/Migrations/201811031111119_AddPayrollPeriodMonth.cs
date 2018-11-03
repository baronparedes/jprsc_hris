namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPayrollPeriodMonth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DailyTimeRecords", "PayrollPeriodMonth", c => c.Int());
            AddColumn("dbo.Overtimes", "PayrollPeriodMonth", c => c.Int());
            AddColumn("dbo.EarningDeductionRecords", "PayrollPeriodMonth", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EarningDeductionRecords", "PayrollPeriodMonth");
            DropColumn("dbo.Overtimes", "PayrollPeriodMonth");
            DropColumn("dbo.DailyTimeRecords", "PayrollPeriodMonth");
        }
    }
}

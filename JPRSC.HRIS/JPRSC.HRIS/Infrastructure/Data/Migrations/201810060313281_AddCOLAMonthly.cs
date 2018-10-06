namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCOLAMonthly : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayrollRecords", "COLAMonthlyValue", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Employees", "COLAMonthly", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.DailyTimeRecords", "COLAMonthlyValue", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DailyTimeRecords", "COLAMonthlyValue");
            DropColumn("dbo.Employees", "COLAMonthly");
            DropColumn("dbo.PayrollRecords", "COLAMonthlyValue");
        }
    }
}

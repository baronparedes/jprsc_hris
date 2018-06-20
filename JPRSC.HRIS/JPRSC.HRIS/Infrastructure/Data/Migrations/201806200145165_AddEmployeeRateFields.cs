namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmployeeRateFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "HourlyRate", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Employees", "DailyRate", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Employees", "COLAHourly", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Employees", "COLADaily", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "COLADaily");
            DropColumn("dbo.Employees", "COLAHourly");
            DropColumn("dbo.Employees", "DailyRate");
            DropColumn("dbo.Employees", "HourlyRate");
        }
    }
}

namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCOLAValuesToDailyTimeRecord : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DailyTimeRecords", "COLADailyValue", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.DailyTimeRecords", "COLAHourlyValue", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DailyTimeRecords", "COLAHourlyValue");
            DropColumn("dbo.DailyTimeRecords", "COLADailyValue");
        }
    }
}

namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRatesToDailyTimeRecord : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DailyTimeRecords", "DailyRate", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.DailyTimeRecords", "HourlyRate", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DailyTimeRecords", "HourlyRate");
            DropColumn("dbo.DailyTimeRecords", "DailyRate");
        }
    }
}

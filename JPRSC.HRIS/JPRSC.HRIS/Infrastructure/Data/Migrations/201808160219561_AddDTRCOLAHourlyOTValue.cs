namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDTRCOLAHourlyOTValue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DailyTimeRecords", "COLAHourlyOTValue", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DailyTimeRecords", "COLAHourlyOTValue");
        }
    }
}

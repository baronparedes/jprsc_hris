namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOvertimeNumberOfHoursValue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Overtimes", "NumberOfHoursValue", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Overtimes", "NumberOfHoursValue");
        }
    }
}

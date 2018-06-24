namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveClientCutOffPeriodNumberOfHoursInADay : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Clients", "CutOffPeriod");
            DropColumn("dbo.Clients", "NumberOfHoursInADay");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Clients", "NumberOfHoursInADay", c => c.Int());
            AddColumn("dbo.Clients", "CutOffPeriod", c => c.Int());
        }
    }
}

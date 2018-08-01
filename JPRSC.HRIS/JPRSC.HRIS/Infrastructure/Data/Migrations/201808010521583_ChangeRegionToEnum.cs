namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeRegionToEnum : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Employees", "Region", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Employees", "Region", c => c.String());
        }
    }
}

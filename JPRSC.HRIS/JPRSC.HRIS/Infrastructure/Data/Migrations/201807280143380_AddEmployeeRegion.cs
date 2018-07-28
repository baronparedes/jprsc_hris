namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmployeeRegion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "Region", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "Region");
        }
    }
}

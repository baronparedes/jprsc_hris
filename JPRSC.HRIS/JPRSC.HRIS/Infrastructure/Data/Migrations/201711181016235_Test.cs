namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "Name");
        }
    }
}

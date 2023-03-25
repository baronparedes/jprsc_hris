namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClientSSSRangeOffset : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "SSSRangeOffset", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "SSSRangeOffset");
        }
    }
}

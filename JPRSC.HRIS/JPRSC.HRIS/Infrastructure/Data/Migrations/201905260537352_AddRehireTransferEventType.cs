namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRehireTransferEventType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RehireTransferEvents", "Type", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RehireTransferEvents", "Type");
        }
    }
}

namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveBankAddress2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Banks", "Address2");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Banks", "Address2", c => c.String());
        }
    }
}

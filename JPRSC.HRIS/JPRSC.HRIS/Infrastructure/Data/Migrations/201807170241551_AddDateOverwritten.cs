namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDateOverwritten : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayrollProcessBatches", "DateOverwritten", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PayrollProcessBatches", "DateOverwritten");
        }
    }
}

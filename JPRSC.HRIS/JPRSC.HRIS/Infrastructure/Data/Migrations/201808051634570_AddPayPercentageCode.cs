namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPayPercentageCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayPercentages", "Code", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PayPercentages", "Code");
        }
    }
}

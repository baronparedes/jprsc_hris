namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPayPercentageIncludeCOLA : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayPercentages", "IncludeCOLA", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PayPercentages", "IncludeCOLA");
        }
    }
}

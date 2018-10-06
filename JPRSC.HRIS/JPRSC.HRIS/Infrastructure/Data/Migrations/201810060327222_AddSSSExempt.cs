namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSSSExempt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "SSSExempt", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "SSSExempt");
        }
    }
}

namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompanyCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "Code", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "Code");
        }
    }
}

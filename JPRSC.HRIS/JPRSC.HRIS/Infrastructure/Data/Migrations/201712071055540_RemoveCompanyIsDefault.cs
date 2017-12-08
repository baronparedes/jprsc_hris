namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveCompanyIsDefault : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.CompanyProfiles", "IsDefault");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CompanyProfiles", "IsDefault", c => c.Boolean());
        }
    }
}

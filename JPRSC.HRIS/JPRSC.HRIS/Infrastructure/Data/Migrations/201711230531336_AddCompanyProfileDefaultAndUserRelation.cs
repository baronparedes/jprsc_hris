namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompanyProfileDefaultAndUserRelation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompanyProfiles", "IsDefault", c => c.Boolean());
            AddColumn("dbo.AspNetUsers", "CompanyProfileId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "CompanyProfileId");
            AddForeignKey("dbo.AspNetUsers", "CompanyProfileId", "dbo.CompanyProfiles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "CompanyProfileId", "dbo.CompanyProfiles");
            DropIndex("dbo.AspNetUsers", new[] { "CompanyProfileId" });
            DropColumn("dbo.AspNetUsers", "CompanyProfileId");
            DropColumn("dbo.CompanyProfiles", "IsDefault");
        }
    }
}

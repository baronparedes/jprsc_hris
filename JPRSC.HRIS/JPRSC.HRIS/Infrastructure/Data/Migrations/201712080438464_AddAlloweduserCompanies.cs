namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAlloweduserCompanies : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AllowedUserCompanies",
                c => new
                    {
                        CompanyProfileId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.CompanyProfileId, t.UserId })
                .ForeignKey("dbo.CompanyProfiles", t => t.CompanyProfileId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.CompanyProfileId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AllowedUserCompanies", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AllowedUserCompanies", "CompanyProfileId", "dbo.CompanyProfiles");
            DropIndex("dbo.AllowedUserCompanies", new[] { "UserId" });
            DropIndex("dbo.AllowedUserCompanies", new[] { "CompanyProfileId" });
            DropTable("dbo.AllowedUserCompanies");
        }
    }
}

namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompanyClientTags : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompanyClientTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        CompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .Index(t => new { t.ClientId, t.CompanyId }, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompanyClientTags", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.CompanyClientTags", "ClientId", "dbo.Clients");
            DropIndex("dbo.CompanyClientTags", new[] { "ClientId", "CompanyId" });
            DropTable("dbo.CompanyClientTags");
        }
    }
}

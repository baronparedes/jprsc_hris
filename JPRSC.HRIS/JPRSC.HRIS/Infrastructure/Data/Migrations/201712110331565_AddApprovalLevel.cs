namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddApprovalLevel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApprovalLevels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        DeletedOn = c.DateTime(),
                        Level = c.Int(),
                        ModifiedOn = c.DateTime(),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApprovalLevels", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.ApprovalLevels", new[] { "UserId" });
            DropTable("dbo.ApprovalLevels");
        }
    }
}

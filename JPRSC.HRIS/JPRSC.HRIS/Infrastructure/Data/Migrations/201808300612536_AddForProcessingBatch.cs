namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForProcessingBatch : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ForProcessingBatches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        ClientId = c.Int(),
                        DateFormatted = c.String(),
                        DeletedOn = c.DateTime(),
                        EmployeeIds = c.String(),
                        ModifiedOn = c.DateTime(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .Index(t => t.ClientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ForProcessingBatches", "ClientId", "dbo.Clients");
            DropIndex("dbo.ForProcessingBatches", new[] { "ClientId" });
            DropTable("dbo.ForProcessingBatches");
        }
    }
}

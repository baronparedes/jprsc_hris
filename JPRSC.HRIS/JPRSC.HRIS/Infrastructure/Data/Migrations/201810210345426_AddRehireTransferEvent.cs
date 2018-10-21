namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRehireTransferEvent : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RehireTransferEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        RehireTransferDateLocal = c.DateTime(nullable: false),
                        EmployeeId = c.Int(),
                        ClientId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .Index(t => t.EmployeeId)
                .Index(t => t.ClientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RehireTransferEvents", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.RehireTransferEvents", "ClientId", "dbo.Clients");
            DropIndex("dbo.RehireTransferEvents", new[] { "ClientId" });
            DropIndex("dbo.RehireTransferEvents", new[] { "EmployeeId" });
            DropTable("dbo.RehireTransferEvents");
        }
    }
}

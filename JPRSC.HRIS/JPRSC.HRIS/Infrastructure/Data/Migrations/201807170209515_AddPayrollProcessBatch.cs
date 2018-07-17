namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPayrollProcessBatch : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PayrollProcessBatches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        DeletedOn = c.DateTime(),
                        ModifiedOn = c.DateTime(),
                        ClientId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .Index(t => t.ClientId);
            
            AddColumn("dbo.PayrollRecords", "PayrollProcessBatchId", c => c.Int());
            CreateIndex("dbo.PayrollRecords", "PayrollProcessBatchId");
            AddForeignKey("dbo.PayrollRecords", "PayrollProcessBatchId", "dbo.PayrollProcessBatches", "Id");

            DropColumn("dbo.PayrollRecords", "PayrollPeriodFrom");
            DropColumn("dbo.PayrollRecords", "PayrollPeriodTo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PayrollRecords", "PayrollPeriodTo", c => c.DateTime());
            AddColumn("dbo.PayrollRecords", "PayrollPeriodFrom", c => c.DateTime());
            DropForeignKey("dbo.PayrollRecords", "PayrollProcessBatchId", "dbo.PayrollProcessBatches");
            DropForeignKey("dbo.PayrollProcessBatches", "ClientId", "dbo.Clients");
            DropIndex("dbo.PayrollRecords", new[] { "PayrollProcessBatchId" });
            DropIndex("dbo.PayrollProcessBatches", new[] { "ClientId" });
            DropColumn("dbo.PayrollRecords", "PayrollProcessBatchId");
            DropTable("dbo.PayrollProcessBatches");
        }
    }
}

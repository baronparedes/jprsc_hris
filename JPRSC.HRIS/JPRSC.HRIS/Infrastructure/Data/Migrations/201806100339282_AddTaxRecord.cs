namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTaxRecord : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TaxRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        Code = c.String(),
                        DeletedOn = c.DateTime(),
                        Exemption = c.Double(),
                        ModifiedOn = c.DateTime(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TaxRanges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        DeletedOn = c.DateTime(),
                        ModifiedOn = c.DateTime(),
                        Percentage = c.Double(),
                        Plus = c.Decimal(precision: 18, scale: 2),
                        Range = c.Decimal(precision: 18, scale: 2),
                        TaxRecordId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TaxRecords", t => t.TaxRecordId)
                .Index(t => t.TaxRecordId);
            
            AddColumn("dbo.AspNetUsers", "TaxRecordId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "TaxRecordId");
            AddForeignKey("dbo.AspNetUsers", "TaxRecordId", "dbo.TaxRecords", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "TaxRecordId", "dbo.TaxRecords");
            DropForeignKey("dbo.TaxRanges", "TaxRecordId", "dbo.TaxRecords");
            DropIndex("dbo.TaxRanges", new[] { "TaxRecordId" });
            DropIndex("dbo.AspNetUsers", new[] { "TaxRecordId" });
            DropColumn("dbo.AspNetUsers", "TaxRecordId");
            DropTable("dbo.TaxRanges");
            DropTable("dbo.TaxRecords");
        }
    }
}

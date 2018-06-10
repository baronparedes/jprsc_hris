namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MergeTaxRecordIntoTaxStatus : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TaxRanges", "TaxRecordId", "dbo.TaxRecords");
            DropForeignKey("dbo.Employees", "TaxRecordId", "dbo.TaxRecords");
            DropIndex("dbo.Employees", new[] { "TaxRecordId" });
            DropIndex("dbo.TaxRanges", new[] { "TaxRecordId" });
            AddColumn("dbo.TaxRanges", "TaxStatusId", c => c.Int());
            AddColumn("dbo.TaxStatus", "Code", c => c.String());
            AddColumn("dbo.TaxStatus", "Exemption", c => c.Double());
            CreateIndex("dbo.TaxRanges", "TaxStatusId");
            AddForeignKey("dbo.TaxRanges", "TaxStatusId", "dbo.TaxStatus", "Id");
            DropColumn("dbo.Employees", "TaxRecordId");
            DropColumn("dbo.TaxRanges", "TaxRecordId");
            DropTable("dbo.TaxRecords");
        }
        
        public override void Down()
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
            
            AddColumn("dbo.TaxRanges", "TaxRecordId", c => c.Int());
            AddColumn("dbo.Employees", "TaxRecordId", c => c.Int());
            DropForeignKey("dbo.TaxRanges", "TaxStatusId", "dbo.TaxStatus");
            DropIndex("dbo.TaxRanges", new[] { "TaxStatusId" });
            DropColumn("dbo.TaxStatus", "Exemption");
            DropColumn("dbo.TaxStatus", "Code");
            DropColumn("dbo.TaxRanges", "TaxStatusId");
            CreateIndex("dbo.TaxRanges", "TaxRecordId");
            CreateIndex("dbo.Employees", "TaxRecordId");
            AddForeignKey("dbo.Employees", "TaxRecordId", "dbo.TaxRecords", "Id");
            AddForeignKey("dbo.TaxRanges", "TaxRecordId", "dbo.TaxRecords", "Id");
        }
    }
}

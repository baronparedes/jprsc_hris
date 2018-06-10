namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoveTaxRecordRelationship : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "TaxRecordId", "dbo.TaxRecords");
            DropIndex("dbo.AspNetUsers", new[] { "TaxRecordId" });
            AddColumn("dbo.Employees", "TaxRecordId", c => c.Int());
            CreateIndex("dbo.Employees", "TaxRecordId");
            AddForeignKey("dbo.Employees", "TaxRecordId", "dbo.TaxRecords", "Id");
            DropColumn("dbo.AspNetUsers", "TaxRecordId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "TaxRecordId", c => c.Int());
            DropForeignKey("dbo.Employees", "TaxRecordId", "dbo.TaxRecords");
            DropIndex("dbo.Employees", new[] { "TaxRecordId" });
            DropColumn("dbo.Employees", "TaxRecordId");
            CreateIndex("dbo.AspNetUsers", "TaxRecordId");
            AddForeignKey("dbo.AspNetUsers", "TaxRecordId", "dbo.TaxRecords", "Id");
        }
    }
}

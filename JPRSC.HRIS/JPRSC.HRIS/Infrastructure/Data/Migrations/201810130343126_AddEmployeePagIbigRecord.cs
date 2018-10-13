namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmployeePagIbigRecord : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "PagIbigRecordId", c => c.Int());
            CreateIndex("dbo.Employees", "PagIbigRecordId");
            AddForeignKey("dbo.Employees", "PagIbigRecordId", "dbo.PagIbigRecords", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Employees", "PagIbigRecordId", "dbo.PagIbigRecords");
            DropIndex("dbo.Employees", new[] { "PagIbigRecordId" });
            DropColumn("dbo.Employees", "PagIbigRecordId");
        }
    }
}

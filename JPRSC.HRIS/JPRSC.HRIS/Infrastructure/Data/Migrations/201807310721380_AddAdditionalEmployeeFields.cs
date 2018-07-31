namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAdditionalEmployeeFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "JobTitleId", c => c.Int());
            AddColumn("dbo.Employees", "CompanyIdNumber", c => c.String());
            AddColumn("dbo.Employees", "PermanentAddress", c => c.String());
            AddColumn("dbo.Employees", "PlaceOfBirth", c => c.String());
            CreateIndex("dbo.Employees", "JobTitleId");
            AddForeignKey("dbo.Employees", "JobTitleId", "dbo.JobTitles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Employees", "JobTitleId", "dbo.JobTitles");
            DropIndex("dbo.Employees", new[] { "JobTitleId" });
            DropColumn("dbo.Employees", "PlaceOfBirth");
            DropColumn("dbo.Employees", "PermanentAddress");
            DropColumn("dbo.Employees", "CompanyIdNumber");
            DropColumn("dbo.Employees", "JobTitleId");
        }
    }
}

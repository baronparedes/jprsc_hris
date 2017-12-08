namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserJobTitleRelation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "JobTitleId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "JobTitleId");
            AddForeignKey("dbo.AspNetUsers", "JobTitleId", "dbo.JobTitles", "Id");
            DropColumn("dbo.AspNetUsers", "JobTitle");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "JobTitle", c => c.String());
            DropForeignKey("dbo.AspNetUsers", "JobTitleId", "dbo.JobTitles");
            DropIndex("dbo.AspNetUsers", new[] { "JobTitleId" });
            DropColumn("dbo.AspNetUsers", "JobTitleId");
        }
    }
}

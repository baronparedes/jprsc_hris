namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "AddedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "DeletedOn", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "JobTitle", c => c.String());
            AddColumn("dbo.AspNetUsers", "ModifiedOn", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Name");
            DropColumn("dbo.AspNetUsers", "ModifiedOn");
            DropColumn("dbo.AspNetUsers", "JobTitle");
            DropColumn("dbo.AspNetUsers", "DeletedOn");
            DropColumn("dbo.AspNetUsers", "AddedOn");
        }
    }
}

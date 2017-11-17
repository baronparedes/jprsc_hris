namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateLogEntry : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomRoles", "DeletedOn", c => c.DateTime());
            AddColumn("dbo.LogEntries", "Level", c => c.String());
            AddColumn("dbo.LogEntries", "Request", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LogEntries", "Request");
            DropColumn("dbo.LogEntries", "Level");
            DropColumn("dbo.CustomRoles", "DeletedOn");
        }
    }
}

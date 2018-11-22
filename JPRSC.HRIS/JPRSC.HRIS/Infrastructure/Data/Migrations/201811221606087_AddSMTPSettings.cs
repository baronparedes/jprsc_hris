namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSMTPSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SystemSettings", "EmailAddress", c => c.String());
            AddColumn("dbo.SystemSettings", "Password", c => c.String());
            AddColumn("dbo.SystemSettings", "Port", c => c.String());
            AddColumn("dbo.SystemSettings", "Host", c => c.String());
            AddColumn("dbo.SystemSettings", "TestEmailAddress", c => c.String());
            AddColumn("dbo.SystemSettings", "EnableSendingEmails", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SystemSettings", "EnableSendingEmails");
            DropColumn("dbo.SystemSettings", "TestEmailAddress");
            DropColumn("dbo.SystemSettings", "Host");
            DropColumn("dbo.SystemSettings", "Port");
            DropColumn("dbo.SystemSettings", "Password");
            DropColumn("dbo.SystemSettings", "EmailAddress");
        }
    }
}

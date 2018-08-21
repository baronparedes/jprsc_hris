namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuditTrail : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditTrailEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Action = c.String(),
                        AddedOn = c.DateTime(nullable: false),
                        Module = c.String(),
                        RecordId = c.Int(),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AuditTrailEntries");
        }
    }
}

namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveSystemSettings : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.SystemSettings");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SystemSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SSSRate = c.Double(),
                        PHICRate = c.Double(),
                        ModifiedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}

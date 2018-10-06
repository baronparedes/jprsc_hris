namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSystemSettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SystemSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MinimumNetPay = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SystemSettings");
        }
    }
}

namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPayPercentage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PayPercentages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ModifiedOn = c.DateTime(),
                        Name = c.String(),
                        Percentage = c.Double(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PayPercentages");
        }
    }
}

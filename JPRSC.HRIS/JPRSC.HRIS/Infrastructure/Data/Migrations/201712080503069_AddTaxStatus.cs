namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTaxStatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TaxStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        DeletedOn = c.DateTime(),
                        ModifiedOn = c.DateTime(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TaxStatus");
        }
    }
}

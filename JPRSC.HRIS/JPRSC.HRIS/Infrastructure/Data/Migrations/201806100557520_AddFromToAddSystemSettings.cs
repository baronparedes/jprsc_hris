namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFromToAddSystemSettings : DbMigration
    {
        public override void Up()
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
            
            AddColumn("dbo.TaxRanges", "From", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.TaxRanges", "To", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.TaxRanges", "Range");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TaxRanges", "Range", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.TaxRanges", "To");
            DropColumn("dbo.TaxRanges", "From");
            DropTable("dbo.SystemSettings");
        }
    }
}

namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveSSSPhilHealthEmployeeEmployerShare : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SSSRecords", "PhilHealthEmployee");
            DropColumn("dbo.SSSRecords", "PhilHealthEmployer");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SSSRecords", "PhilHealthEmployer", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SSSRecords", "PhilHealthEmployee", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}

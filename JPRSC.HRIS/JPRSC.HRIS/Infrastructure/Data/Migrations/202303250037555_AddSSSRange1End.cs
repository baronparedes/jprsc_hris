namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSSSRange1End : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SSSRecords", "Range1End", c => c.Decimal(precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SSSRecords", "Range1End");
        }
    }
}

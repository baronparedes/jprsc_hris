namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPHICMinMaxDeduction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PhicRecords", "MaximumDeduction", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.PhicRecords", "MinimumDeduction", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PhicRecords", "MinimumDeduction");
            DropColumn("dbo.PhicRecords", "MaximumDeduction");
        }
    }
}

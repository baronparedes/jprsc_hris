namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMinimumDeductionOfContribution : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SystemSettings", "MinimumDeductionOfContribution", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SystemSettings", "MinimumDeductionOfContribution");
        }
    }
}

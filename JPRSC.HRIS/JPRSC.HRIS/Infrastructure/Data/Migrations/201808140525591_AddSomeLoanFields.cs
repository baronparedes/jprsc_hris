namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSomeLoanFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loans", "MonthsPayable", c => c.Int());
            AddColumn("dbo.Loans", "PrincipalAndInterestAmount", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Loans", "PrincipalAndInterestAmount");
            DropColumn("dbo.Loans", "MonthsPayable");
        }
    }
}

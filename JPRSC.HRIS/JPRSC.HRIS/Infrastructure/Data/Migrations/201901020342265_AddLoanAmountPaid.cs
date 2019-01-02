namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLoanAmountPaid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loans", "AmountPaid", c => c.Decimal(precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Loans", "AmountPaid");
        }
    }
}

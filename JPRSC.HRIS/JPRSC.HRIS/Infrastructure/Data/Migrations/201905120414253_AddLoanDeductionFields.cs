namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLoanDeductionFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoanDeductions", "NewAmountPaid", c => c.Decimal(precision: 18, scale: 4));
            AddColumn("dbo.LoanDeductions", "NewRemainingBalance", c => c.Decimal(precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoanDeductions", "NewRemainingBalance");
            DropColumn("dbo.LoanDeductions", "NewAmountPaid");
        }
    }
}

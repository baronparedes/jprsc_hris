namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveLoanInterestRateMonthsPayable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loans", "LoanPayrollPeriod", c => c.String());
            AddColumn("dbo.Loans", "StartDeduction", c => c.DateTime());
            DropColumn("dbo.Loans", "InterestRate");
            DropColumn("dbo.Loans", "MonthsPayable");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Loans", "MonthsPayable", c => c.Int());
            AddColumn("dbo.Loans", "InterestRate", c => c.Double());
            DropColumn("dbo.Loans", "StartDeduction");
            DropColumn("dbo.Loans", "LoanPayrollPeriod");
        }
    }
}

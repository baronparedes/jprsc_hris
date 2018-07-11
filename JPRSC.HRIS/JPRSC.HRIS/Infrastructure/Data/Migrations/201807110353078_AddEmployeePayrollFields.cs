namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmployeePayrollFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "ResignStatus", c => c.String());
            AddColumn("dbo.Employees", "TaxExempt", c => c.Boolean());
            AddColumn("dbo.Employees", "PagIbigExempt", c => c.Boolean());
            AddColumn("dbo.Employees", "ThirteenthMonthExempt", c => c.Boolean());
            AddColumn("dbo.Employees", "PhilHealthExempt", c => c.Boolean());
            AddColumn("dbo.Employees", "SalaryStatus", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "SalaryStatus");
            DropColumn("dbo.Employees", "PhilHealthExempt");
            DropColumn("dbo.Employees", "ThirteenthMonthExempt");
            DropColumn("dbo.Employees", "PagIbigExempt");
            DropColumn("dbo.Employees", "TaxExempt");
            DropColumn("dbo.Employees", "ResignStatus");
        }
    }
}

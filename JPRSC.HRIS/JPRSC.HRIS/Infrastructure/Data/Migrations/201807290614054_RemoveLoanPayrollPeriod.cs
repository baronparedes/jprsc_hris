namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveLoanPayrollPeriod : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Loans", "PayrollPeriod");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Loans", "PayrollPeriod", c => c.Int());
        }
    }
}

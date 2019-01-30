namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLoanExempt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "LoanExempt", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "LoanExempt");
        }
    }
}

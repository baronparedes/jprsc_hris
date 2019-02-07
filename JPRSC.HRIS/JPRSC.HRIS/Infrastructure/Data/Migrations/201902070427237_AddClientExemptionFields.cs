namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClientExemptionFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "LoanExempt", c => c.Boolean());
            AddColumn("dbo.Clients", "PagIbigExempt", c => c.Boolean());
            AddColumn("dbo.Clients", "PHICExempt", c => c.Boolean());
            AddColumn("dbo.Clients", "SSSExempt", c => c.Boolean());
            AddColumn("dbo.Clients", "TaxExempt", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "TaxExempt");
            DropColumn("dbo.Clients", "SSSExempt");
            DropColumn("dbo.Clients", "PHICExempt");
            DropColumn("dbo.Clients", "PagIbigExempt");
            DropColumn("dbo.Clients", "LoanExempt");
        }
    }
}

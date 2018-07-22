namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAdditionalClientPayrollContributionSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "PagIbigEarnings", c => c.Boolean());
            AddColumn("dbo.Clients", "PagIbigDeductions", c => c.Boolean());
            AddColumn("dbo.Clients", "PagIbigUndertime", c => c.Boolean());
            AddColumn("dbo.Clients", "PHICEarnings", c => c.Boolean());
            AddColumn("dbo.Clients", "PHICDeductions", c => c.Boolean());
            AddColumn("dbo.Clients", "PHICUndertime", c => c.Boolean());
            AddColumn("dbo.Clients", "SSSEarnings", c => c.Boolean());
            AddColumn("dbo.Clients", "SSSDeductions", c => c.Boolean());
            AddColumn("dbo.Clients", "SSSUndertime", c => c.Boolean());
            AddColumn("dbo.Clients", "TaxEarnings", c => c.Boolean());
            AddColumn("dbo.Clients", "TaxDeductions", c => c.Boolean());
            AddColumn("dbo.Clients", "TaxUndertime", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "TaxUndertime");
            DropColumn("dbo.Clients", "TaxDeductions");
            DropColumn("dbo.Clients", "TaxEarnings");
            DropColumn("dbo.Clients", "SSSUndertime");
            DropColumn("dbo.Clients", "SSSDeductions");
            DropColumn("dbo.Clients", "SSSEarnings");
            DropColumn("dbo.Clients", "PHICUndertime");
            DropColumn("dbo.Clients", "PHICDeductions");
            DropColumn("dbo.Clients", "PHICEarnings");
            DropColumn("dbo.Clients", "PagIbigUndertime");
            DropColumn("dbo.Clients", "PagIbigDeductions");
            DropColumn("dbo.Clients", "PagIbigEarnings");
        }
    }
}

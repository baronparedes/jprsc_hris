namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClientPayrollFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "SSSPayrollPeriod", c => c.String());
            AddColumn("dbo.Clients", "SSSBasic", c => c.Boolean());
            AddColumn("dbo.Clients", "SSSOvertime", c => c.Boolean());
            AddColumn("dbo.Clients", "SSSCola", c => c.Boolean());
            AddColumn("dbo.Clients", "PHICPayrollPeriod", c => c.String());
            AddColumn("dbo.Clients", "PHICBasic", c => c.Boolean());
            AddColumn("dbo.Clients", "PHICOvertime", c => c.Boolean());
            AddColumn("dbo.Clients", "PHICCola", c => c.Boolean());
            AddColumn("dbo.Clients", "PagIbigPayrollPeriod", c => c.String());
            AddColumn("dbo.Clients", "PagIbigBasic", c => c.Boolean());
            AddColumn("dbo.Clients", "PagIbigOvertime", c => c.Boolean());
            AddColumn("dbo.Clients", "PagIbigCola", c => c.Boolean());
            AddColumn("dbo.Clients", "TaxPayrollPeriod", c => c.String());
            AddColumn("dbo.Clients", "TaxBasic", c => c.Boolean());
            AddColumn("dbo.Clients", "TaxOvertime", c => c.Boolean());
            AddColumn("dbo.Clients", "TaxCola", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "TaxCola");
            DropColumn("dbo.Clients", "TaxOvertime");
            DropColumn("dbo.Clients", "TaxBasic");
            DropColumn("dbo.Clients", "TaxPayrollPeriod");
            DropColumn("dbo.Clients", "PagIbigCola");
            DropColumn("dbo.Clients", "PagIbigOvertime");
            DropColumn("dbo.Clients", "PagIbigBasic");
            DropColumn("dbo.Clients", "PagIbigPayrollPeriod");
            DropColumn("dbo.Clients", "PHICCola");
            DropColumn("dbo.Clients", "PHICOvertime");
            DropColumn("dbo.Clients", "PHICBasic");
            DropColumn("dbo.Clients", "PHICPayrollPeriod");
            DropColumn("dbo.Clients", "SSSCola");
            DropColumn("dbo.Clients", "SSSOvertime");
            DropColumn("dbo.Clients", "SSSBasic");
            DropColumn("dbo.Clients", "SSSPayrollPeriod");
        }
    }
}

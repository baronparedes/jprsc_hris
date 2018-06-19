namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClientFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "Code", c => c.String());
            AddColumn("dbo.Clients", "Description", c => c.String());
            AddColumn("dbo.Clients", "TaxTable", c => c.Int());
            AddColumn("dbo.Clients", "PayrollCode", c => c.Int());
            AddColumn("dbo.Clients", "NumberOfWorkingDaysForThisPayrollPeriod", c => c.Int());
            AddColumn("dbo.Clients", "NumberOfHoursInADay", c => c.Int());
            AddColumn("dbo.Clients", "NumberOfPayrollPeriodsAMonth", c => c.Int());
            AddColumn("dbo.Clients", "PayrollPeriodFrom", c => c.DateTime());
            AddColumn("dbo.Clients", "PayrollPeriodTo", c => c.DateTime());
            AddColumn("dbo.Clients", "PayrollPeriodMonth", c => c.Int());
            AddColumn("dbo.Clients", "CurrentPayrollPeriod", c => c.Int());
            AddColumn("dbo.Clients", "ZeroBasic", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "ZeroBasic");
            DropColumn("dbo.Clients", "CurrentPayrollPeriod");
            DropColumn("dbo.Clients", "PayrollPeriodMonth");
            DropColumn("dbo.Clients", "PayrollPeriodTo");
            DropColumn("dbo.Clients", "PayrollPeriodFrom");
            DropColumn("dbo.Clients", "NumberOfPayrollPeriodsAMonth");
            DropColumn("dbo.Clients", "NumberOfHoursInADay");
            DropColumn("dbo.Clients", "NumberOfWorkingDaysForThisPayrollPeriod");
            DropColumn("dbo.Clients", "PayrollCode");
            DropColumn("dbo.Clients", "TaxTable");
            DropColumn("dbo.Clients", "Description");
            DropColumn("dbo.Clients", "Code");
        }
    }
}

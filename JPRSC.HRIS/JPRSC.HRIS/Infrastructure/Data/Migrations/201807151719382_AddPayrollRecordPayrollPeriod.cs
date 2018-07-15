namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPayrollRecordPayrollPeriod : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayrollRecords", "PayrollPeriod", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PayrollRecords", "PayrollPeriod");
        }
    }
}

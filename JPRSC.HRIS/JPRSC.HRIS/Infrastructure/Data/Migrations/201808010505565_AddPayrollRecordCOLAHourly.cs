namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPayrollRecordCOLAHourly : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayrollRecords", "COLAHourlyValue", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PayrollRecords", "COLAHourlyValue");
        }
    }
}

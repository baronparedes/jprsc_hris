namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPayrollRecordNetPayValue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayrollRecords", "NetPayValue", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PayrollRecords", "NetPayValue");
        }
    }
}

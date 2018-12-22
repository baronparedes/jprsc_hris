namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPayrollRecordDeductionBasis : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayrollRecords", "DeductionBasis", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PayrollRecords", "DeductionBasis");
        }
    }
}

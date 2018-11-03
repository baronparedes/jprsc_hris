namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPayrollRecordDeductionFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayrollRecords", "SSSDeductionBasis", c => c.Decimal(precision: 18, scale: 4));
            AddColumn("dbo.PayrollRecords", "PHICDeductionBasis", c => c.Decimal(precision: 18, scale: 4));
            AddColumn("dbo.PayrollRecords", "PagIbigDeductionBasis", c => c.Decimal(precision: 18, scale: 4));
            AddColumn("dbo.PayrollRecords", "GovDeductionsDeducted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PayrollRecords", "LoansDeducted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PayrollRecords", "AnythingDeducted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PayrollRecords", "AnythingDeducted");
            DropColumn("dbo.PayrollRecords", "LoansDeducted");
            DropColumn("dbo.PayrollRecords", "GovDeductionsDeducted");
            DropColumn("dbo.PayrollRecords", "PagIbigDeductionBasis");
            DropColumn("dbo.PayrollRecords", "PHICDeductionBasis");
            DropColumn("dbo.PayrollRecords", "SSSDeductionBasis");
        }
    }
}

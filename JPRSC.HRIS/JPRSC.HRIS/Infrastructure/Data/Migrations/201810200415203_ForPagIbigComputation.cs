namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForPagIbigComputation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayrollRecords", "PagIbigValueEmployer", c => c.Decimal(precision: 18, scale: 4));

            RenameColumn("dbo.PagIbigRecords", "EmployeeAmount", "DeductionAmount");
            RenameColumn("dbo.PagIbigRecords", "EmployerAmount", "MinimumDeduction");
            RenameColumn("dbo.PayrollRecords", "PagIbigValue", "PagIbigValueEmployee");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.PayrollRecords", "PagIbigValueEmployee", "PagIbigValue");
            RenameColumn("dbo.PagIbigRecords", "MinimumDeduction", "EmployerAmount");
            RenameColumn("dbo.PagIbigRecords", "DeductionAmount", "EmployeeAmount");
            
            DropColumn("dbo.PayrollRecords", "PagIbigValueEmployer");
        }
    }
}

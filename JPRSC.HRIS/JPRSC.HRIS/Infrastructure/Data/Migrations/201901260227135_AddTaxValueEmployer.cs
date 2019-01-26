namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTaxValueEmployer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayrollRecords", "TaxValueEmployer", c => c.Decimal(precision: 18, scale: 4));
            RenameColumn("dbo.PayrollRecords", "TaxValue", "TaxValueEmployee");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.PayrollRecords", "TaxValueEmployee", "TaxValue");
            DropColumn("dbo.PayrollRecords", "TaxValueEmployer");
        }
    }
}

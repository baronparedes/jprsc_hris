namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPHICValueEmployerField : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.PayrollRecords", "PHICValue", "PHICValueEmployee");
            AddColumn("dbo.PayrollRecords", "PHICValueEmployer", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PayrollRecords", "PHICValueEmployer");
            RenameColumn("dbo.PayrollRecords", "PHICValueEmployee", "PHICValue");
        }
    }
}

namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMonthlyRate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "MonthlyRate", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "MonthlyRate");
        }
    }
}

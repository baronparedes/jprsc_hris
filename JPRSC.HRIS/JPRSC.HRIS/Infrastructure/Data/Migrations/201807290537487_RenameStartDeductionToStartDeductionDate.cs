namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameStartDeductionToStartDeductionDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loans", "StartDeductionDate", c => c.DateTime());
            DropColumn("dbo.Loans", "StartDeduction");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Loans", "StartDeduction", c => c.DateTime());
            DropColumn("dbo.Loans", "StartDeductionDate");
        }
    }
}

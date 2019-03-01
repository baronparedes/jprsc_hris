namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLoanLastDeductedOn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loans", "LastDeductedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Loans", "LastDeductedOn");
        }
    }
}

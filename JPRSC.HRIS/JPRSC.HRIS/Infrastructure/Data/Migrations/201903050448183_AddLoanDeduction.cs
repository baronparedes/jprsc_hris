namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLoanDeduction : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LoanDeductions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeductionAmount = c.Decimal(precision: 18, scale: 4),
                        LoanId = c.Int(),
                        PayrollRecordId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Loans", t => t.LoanId)
                .ForeignKey("dbo.PayrollRecords", t => t.PayrollRecordId)
                .Index(t => t.LoanId)
                .Index(t => t.PayrollRecordId);
            
            DropColumn("dbo.Loans", "LastDeductedOn");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Loans", "LastDeductedOn", c => c.DateTime());
            DropForeignKey("dbo.LoanDeductions", "PayrollRecordId", "dbo.PayrollRecords");
            DropForeignKey("dbo.LoanDeductions", "LoanId", "dbo.Loans");
            DropIndex("dbo.LoanDeductions", new[] { "PayrollRecordId" });
            DropIndex("dbo.LoanDeductions", new[] { "LoanId" });
            DropTable("dbo.LoanDeductions");
        }
    }
}

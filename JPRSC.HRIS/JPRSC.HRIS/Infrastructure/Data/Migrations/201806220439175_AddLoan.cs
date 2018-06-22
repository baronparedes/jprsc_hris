namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLoan : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Loans",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        DeletedOn = c.DateTime(),
                        EmployeeId = c.Int(),
                        InterestRate = c.Double(),
                        LoanDate = c.DateTime(),
                        LoanTypeId = c.Int(),
                        ModifiedOn = c.DateTime(),
                        MonthsPayable = c.Int(),
                        PayrollPeriod = c.Int(),
                        PrincipalAmount = c.Decimal(precision: 18, scale: 2),
                        TransactionNumber = c.String(),
                        ZeroedOutOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.LoanTypes", t => t.LoanTypeId)
                .Index(t => t.EmployeeId)
                .Index(t => t.LoanTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Loans", "LoanTypeId", "dbo.LoanTypes");
            DropForeignKey("dbo.Loans", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.Loans", new[] { "LoanTypeId" });
            DropIndex("dbo.Loans", new[] { "EmployeeId" });
            DropTable("dbo.Loans");
        }
    }
}

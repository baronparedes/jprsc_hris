namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEarningDeductionRecord : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EarningDeductionRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        Amount = c.Decimal(precision: 18, scale: 2),
                        DeletedOn = c.DateTime(),
                        EarningDeductionId = c.Int(),
                        EmployeeId = c.Int(),
                        ModifiedOn = c.DateTime(),
                        PayrollPeriodFrom = c.DateTime(),
                        PayrollPeriodTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EarningDeductions", t => t.EarningDeductionId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .Index(t => t.EarningDeductionId)
                .Index(t => t.EmployeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EarningDeductionRecords", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.EarningDeductionRecords", "EarningDeductionId", "dbo.EarningDeductions");
            DropIndex("dbo.EarningDeductionRecords", new[] { "EmployeeId" });
            DropIndex("dbo.EarningDeductionRecords", new[] { "EarningDeductionId" });
            DropTable("dbo.EarningDeductionRecords");
        }
    }
}

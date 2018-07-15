namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPayrollRecord : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PayrollRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        DeletedOn = c.DateTime(),
                        ModifiedOn = c.DateTime(),
                        EmployeeId = c.Int(),
                        PayrollPeriodFrom = c.DateTime(),
                        PayrollPeriodTo = c.DateTime(),
                        DaysWorkedValue = c.Decimal(precision: 18, scale: 2),
                        HoursWorkedValue = c.Decimal(precision: 18, scale: 2),
                        OvertimeValue = c.Decimal(precision: 18, scale: 2),
                        HoursLateValue = c.Decimal(precision: 18, scale: 2),
                        HoursUndertimeValue = c.Decimal(precision: 18, scale: 2),
                        COLADailyValue = c.Decimal(precision: 18, scale: 2),
                        EarningsValue = c.Decimal(precision: 18, scale: 2),
                        DeductionsValue = c.Decimal(precision: 18, scale: 2),
                        DeductedSSS = c.Boolean(),
                        DeductedPHIC = c.Boolean(),
                        DeductedPagIbig = c.Boolean(),
                        DeductedTax = c.Boolean(),
                        SSSValueEmployee = c.Decimal(precision: 18, scale: 2),
                        SSSValueEmployer = c.Decimal(precision: 18, scale: 2),
                        PHICValue = c.Decimal(precision: 18, scale: 2),
                        PagIbigValue = c.Decimal(precision: 18, scale: 2),
                        TaxValue = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .Index(t => t.EmployeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PayrollRecords", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.PayrollRecords", new[] { "EmployeeId" });
            DropTable("dbo.PayrollRecords");
        }
    }
}

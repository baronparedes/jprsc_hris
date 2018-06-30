namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDailyTimeRecord : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DailyTimeRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        ModifiedOn = c.DateTime(),
                        DeletedOn = c.DateTime(),
                        EmployeeId = c.Int(),
                        PayrollPeriodFrom = c.DateTime(),
                        PayrollPeriodTo = c.DateTime(),
                        DaysWorked = c.Double(),
                        HoursWorked = c.Double(),
                        HoursLate = c.Double(),
                        HoursUndertime = c.Double(),
                        DaysWorkedValue = c.Decimal(precision: 18, scale: 2),
                        HoursWorkedValue = c.Decimal(precision: 18, scale: 2),
                        HoursLateValue = c.Decimal(precision: 18, scale: 2),
                        HoursUndertimeValue = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .Index(t => t.EmployeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DailyTimeRecords", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.DailyTimeRecords", new[] { "EmployeeId" });
            DropTable("dbo.DailyTimeRecords");
        }
    }
}

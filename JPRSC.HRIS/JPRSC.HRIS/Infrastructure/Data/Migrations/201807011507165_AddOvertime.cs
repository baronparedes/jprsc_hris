namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOvertime : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Overtimes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        DeletedOn = c.DateTime(),
                        EmployeeId = c.Int(),
                        From = c.DateTime(),
                        ModifiedOn = c.DateTime(),
                        NumberOfHours = c.Double(),
                        PayPercentageName = c.String(),
                        PayPercentagePercentage = c.Double(),
                        PayrollPeriodFrom = c.DateTime(),
                        PayrollPeriodTo = c.DateTime(),
                        Reference = c.String(),
                        To = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .Index(t => t.EmployeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Overtimes", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.Overtimes", new[] { "EmployeeId" });
            DropTable("dbo.Overtimes");
        }
    }
}

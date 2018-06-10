namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPagIbigRecord : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PagIbigRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        ApplyToSalary = c.Boolean(),
                        Code = c.String(),
                        DeletedOn = c.DateTime(),
                        Description = c.String(),
                        EmployeeAmount = c.Decimal(precision: 18, scale: 2),
                        EmployeePercentage = c.Double(),
                        EmployerAmount = c.Decimal(precision: 18, scale: 2),
                        EmployerPercentage = c.Double(),
                        ModifiedOn = c.DateTime(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PagIbigRecords");
        }
    }
}

namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmployee : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountType = c.Int(),
                        AddedOn = c.DateTime(nullable: false),
                        ATMAccountNumber = c.String(),
                        CelNo = c.String(),
                        Citizenship = c.Int(),
                        CityAddress = c.String(),
                        CivilStatus = c.Int(),
                        ClientId = c.Int(),
                        CompanyProfileId = c.Int(),
                        DateHired = c.DateTime(),
                        DateOfBirth = c.DateTime(),
                        DateResigned = c.DateTime(),
                        DeletedOn = c.DateTime(),
                        DepartmentId = c.Int(),
                        Email = c.String(),
                        EmployeeCode = c.String(),
                        EmployeeStatus = c.String(),
                        FirstName = c.String(),
                        Gender = c.Int(),
                        LastName = c.String(),
                        MiddleName = c.String(),
                        ModifiedOn = c.DateTime(),
                        Nickname = c.String(),
                        PagIbig = c.String(),
                        PhilHealth = c.String(),
                        Position = c.String(),
                        ReligionId = c.Int(),
                        SSS = c.String(),
                        TaxStatusId = c.Int(),
                        TelNo = c.String(),
                        TIN = c.String(),
                        ZipCode = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .ForeignKey("dbo.CompanyProfiles", t => t.CompanyProfileId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .ForeignKey("dbo.Religions", t => t.ReligionId)
                .ForeignKey("dbo.TaxStatus", t => t.TaxStatusId)
                .Index(t => t.ClientId)
                .Index(t => t.CompanyProfileId)
                .Index(t => t.DepartmentId)
                .Index(t => t.ReligionId)
                .Index(t => t.TaxStatusId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Employees", "TaxStatusId", "dbo.TaxStatus");
            DropForeignKey("dbo.Employees", "ReligionId", "dbo.Religions");
            DropForeignKey("dbo.Employees", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Employees", "CompanyProfileId", "dbo.CompanyProfiles");
            DropForeignKey("dbo.Employees", "ClientId", "dbo.Clients");
            DropIndex("dbo.Employees", new[] { "TaxStatusId" });
            DropIndex("dbo.Employees", new[] { "ReligionId" });
            DropIndex("dbo.Employees", new[] { "DepartmentId" });
            DropIndex("dbo.Employees", new[] { "CompanyProfileId" });
            DropIndex("dbo.Employees", new[] { "ClientId" });
            DropTable("dbo.Employees");
        }
    }
}

namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApprovalLevels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        DeletedOn = c.DateTime(),
                        Level = c.Int(),
                        ModifiedOn = c.DateTime(),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        AddedOn = c.DateTime(nullable: false),
                        CompanyId = c.Int(),
                        DeletedOn = c.DateTime(),
                        JobTitleId = c.Int(),
                        ModifiedOn = c.DateTime(),
                        Name = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.JobTitles", t => t.JobTitleId)
                .Index(t => t.CompanyId)
                .Index(t => t.JobTitleId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        Address = c.String(),
                        BOI = c.String(),
                        DateIssued = c.DateTime(),
                        DeletedOn = c.DateTime(),
                        DTI = c.String(),
                        Email = c.String(),
                        ModifiedOn = c.DateTime(),
                        Name = c.String(),
                        PagIbig = c.String(),
                        PERAA = c.String(),
                        PhilHealth = c.String(),
                        Phone = c.String(),
                        PlaceIssued = c.String(),
                        Registration = c.String(),
                        SEC = c.String(),
                        SSS = c.String(),
                        VAT = c.String(),
                        ZipCode = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.CustomRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        DeletedOn = c.DateTime(),
                        ModifiedOn = c.DateTime(),
                        Name = c.String(),
                        PermissionsString = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.JobTitles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        DeletedOn = c.DateTime(),
                        ModifiedOn = c.DateTime(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        CutOffPeriod = c.Int(),
                        DaysPerWeek = c.Int(),
                        DeletedOn = c.DateTime(),
                        HoursPerDay = c.Int(),
                        ModifiedOn = c.DateTime(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        DeletedOn = c.DateTime(),
                        ModifiedOn = c.DateTime(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EarningDeductions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        Code = c.String(),
                        DeletedOn = c.DateTime(),
                        Description = c.String(),
                        EarningDeductionType = c.Int(),
                        ModifiedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
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
                        CompanyId = c.Int(),
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
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .ForeignKey("dbo.Religions", t => t.ReligionId)
                .ForeignKey("dbo.TaxStatus", t => t.TaxStatusId)
                .Index(t => t.ClientId)
                .Index(t => t.CompanyId)
                .Index(t => t.DepartmentId)
                .Index(t => t.ReligionId)
                .Index(t => t.TaxStatusId);
            
            CreateTable(
                "dbo.Religions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        Code = c.String(),
                        DeletedOn = c.DateTime(),
                        Description = c.String(),
                        ModifiedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TaxStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        DeletedOn = c.DateTime(),
                        ModifiedOn = c.DateTime(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LogEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Action = c.String(),
                        Controller = c.String(),
                        Level = c.String(),
                        LoggedOn = c.DateTime(),
                        Message = c.String(),
                        Request = c.String(),
                        StackTrace = c.String(),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AllowedUserCompanies",
                c => new
                    {
                        CompanyId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.CompanyId, t.UserId })
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.CompanyId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserCustomRoles",
                c => new
                    {
                        CustomRoleId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.CustomRoleId, t.UserId })
                .ForeignKey("dbo.CustomRoles", t => t.CustomRoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.CustomRoleId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Employees", "TaxStatusId", "dbo.TaxStatus");
            DropForeignKey("dbo.Employees", "ReligionId", "dbo.Religions");
            DropForeignKey("dbo.Employees", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Employees", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Employees", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.ApprovalLevels", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "JobTitleId", "dbo.JobTitles");
            DropForeignKey("dbo.UserCustomRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserCustomRoles", "CustomRoleId", "dbo.CustomRoles");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AllowedUserCompanies", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AllowedUserCompanies", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.AspNetUsers", "CompanyId", "dbo.Companies");
            DropIndex("dbo.UserCustomRoles", new[] { "UserId" });
            DropIndex("dbo.UserCustomRoles", new[] { "CustomRoleId" });
            DropIndex("dbo.AllowedUserCompanies", new[] { "UserId" });
            DropIndex("dbo.AllowedUserCompanies", new[] { "CompanyId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Employees", new[] { "TaxStatusId" });
            DropIndex("dbo.Employees", new[] { "ReligionId" });
            DropIndex("dbo.Employees", new[] { "DepartmentId" });
            DropIndex("dbo.Employees", new[] { "CompanyId" });
            DropIndex("dbo.Employees", new[] { "ClientId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "JobTitleId" });
            DropIndex("dbo.AspNetUsers", new[] { "CompanyId" });
            DropIndex("dbo.ApprovalLevels", new[] { "UserId" });
            DropTable("dbo.UserCustomRoles");
            DropTable("dbo.AllowedUserCompanies");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.LogEntries");
            DropTable("dbo.TaxStatus");
            DropTable("dbo.Religions");
            DropTable("dbo.Employees");
            DropTable("dbo.EarningDeductions");
            DropTable("dbo.Departments");
            DropTable("dbo.Clients");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.JobTitles");
            DropTable("dbo.CustomRoles");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.Companies");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.ApprovalLevels");
        }
    }
}

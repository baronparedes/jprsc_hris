namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBank : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Banks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountNumber = c.String(),
                        AddedOn = c.DateTime(nullable: false),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        BatchNumber = c.String(),
                        BranchCode = c.String(),
                        Code = c.String(),
                        CompanyCode = c.String(),
                        ContactPerson = c.String(),
                        DeletedOn = c.DateTime(),
                        Description = c.String(),
                        ModifiedOn = c.DateTime(),
                        Name = c.String(),
                        OtherBankInfo = c.String(),
                        Position = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Banks");
        }
    }
}

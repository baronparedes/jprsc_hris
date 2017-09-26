namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompanyProfile : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompanyProfiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        Address = c.String(),
                        Code = c.String(),
                        Email = c.String(),
                        ModifiedOn = c.DateTime(),
                        Name = c.String(),
                        Phone = c.String(),
                        Position = c.String(),
                        Signatory = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CompanyProfiles");
        }
    }
}

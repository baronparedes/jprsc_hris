namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSSSRecord : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SSSRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        DeletedOn = c.DateTime(),
                        ECC = c.Decimal(precision: 18, scale: 2),
                        Employee = c.Decimal(precision: 18, scale: 2),
                        Employer = c.Decimal(precision: 18, scale: 2),
                        ModifiedOn = c.DateTime(),
                        Number = c.Int(),
                        PhilHealthEmployee = c.Decimal(precision: 18, scale: 2),
                        PhilHealthEmployer = c.Decimal(precision: 18, scale: 2),
                        Range1 = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SSSRecords");
        }
    }
}

namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEarningDeductions : DbMigration
    {
        public override void Up()
        {
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EarningDeductions");
        }
    }
}

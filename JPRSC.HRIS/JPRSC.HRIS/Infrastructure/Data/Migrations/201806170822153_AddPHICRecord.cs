namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPHICRecord : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PhicRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedOn = c.DateTime(nullable: false),
                        DeletedOn = c.DateTime(),
                        EmployeePercentageShare = c.Double(),
                        ModifiedOn = c.DateTime(),
                        Percentage = c.Double(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PhicRecords");
        }
    }
}

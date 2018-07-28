namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOvertimePayPercentageRelation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Overtimes", "PayPercentageId", c => c.Int());
            CreateIndex("dbo.Overtimes", "PayPercentageId");
            AddForeignKey("dbo.Overtimes", "PayPercentageId", "dbo.PayPercentages", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Overtimes", "PayPercentageId", "dbo.PayPercentages");
            DropIndex("dbo.Overtimes", new[] { "PayPercentageId" });
            DropColumn("dbo.Overtimes", "PayPercentageId");
        }
    }
}

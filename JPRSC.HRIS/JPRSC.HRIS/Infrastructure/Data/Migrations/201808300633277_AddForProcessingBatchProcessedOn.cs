namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForProcessingBatchProcessedOn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ForProcessingBatches", "ProcessedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ForProcessingBatches", "ProcessedOn");
        }
    }
}

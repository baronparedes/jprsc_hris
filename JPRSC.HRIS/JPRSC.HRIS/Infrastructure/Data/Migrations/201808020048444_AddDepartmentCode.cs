namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDepartmentCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Departments", "Code", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Departments", "Code");
        }
    }
}

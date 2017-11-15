namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCompanyFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompanyProfiles", "BOI", c => c.String());
            AddColumn("dbo.CompanyProfiles", "DateIssued", c => c.DateTime());
            AddColumn("dbo.CompanyProfiles", "DeletedOn", c => c.DateTime());
            AddColumn("dbo.CompanyProfiles", "DTI", c => c.String());
            AddColumn("dbo.CompanyProfiles", "PagIbig", c => c.String());
            AddColumn("dbo.CompanyProfiles", "PERAA", c => c.String());
            AddColumn("dbo.CompanyProfiles", "PhilHealth", c => c.String());
            AddColumn("dbo.CompanyProfiles", "PlaceIssued", c => c.String());
            AddColumn("dbo.CompanyProfiles", "Registration", c => c.String());
            AddColumn("dbo.CompanyProfiles", "SEC", c => c.String());
            AddColumn("dbo.CompanyProfiles", "SSS", c => c.String());
            AddColumn("dbo.CompanyProfiles", "VAT", c => c.String());
            AddColumn("dbo.CompanyProfiles", "ZipCode", c => c.String());
            DropColumn("dbo.CompanyProfiles", "Code");
            DropColumn("dbo.CompanyProfiles", "Position");
            DropColumn("dbo.CompanyProfiles", "Signatory");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CompanyProfiles", "Signatory", c => c.String());
            AddColumn("dbo.CompanyProfiles", "Position", c => c.String());
            AddColumn("dbo.CompanyProfiles", "Code", c => c.String());
            DropColumn("dbo.CompanyProfiles", "ZipCode");
            DropColumn("dbo.CompanyProfiles", "VAT");
            DropColumn("dbo.CompanyProfiles", "SSS");
            DropColumn("dbo.CompanyProfiles", "SEC");
            DropColumn("dbo.CompanyProfiles", "Registration");
            DropColumn("dbo.CompanyProfiles", "PlaceIssued");
            DropColumn("dbo.CompanyProfiles", "PhilHealth");
            DropColumn("dbo.CompanyProfiles", "PERAA");
            DropColumn("dbo.CompanyProfiles", "PagIbig");
            DropColumn("dbo.CompanyProfiles", "DTI");
            DropColumn("dbo.CompanyProfiles", "DeletedOn");
            DropColumn("dbo.CompanyProfiles", "DateIssued");
            DropColumn("dbo.CompanyProfiles", "BOI");
        }
    }
}

namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmployeeSubmittedFiles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "SubmittedBiodata", c => c.Boolean());
            AddColumn("dbo.Employees", "SubmittedIdPictures", c => c.Boolean());
            AddColumn("dbo.Employees", "SubmittedNBIClearance", c => c.Boolean());
            AddColumn("dbo.Employees", "SubmittedPoliceClearance", c => c.Boolean());
            AddColumn("dbo.Employees", "SubmittedBarangayClearance", c => c.Boolean());
            AddColumn("dbo.Employees", "SubmittedSSSIdOrED1Form", c => c.Boolean());
            AddColumn("dbo.Employees", "SubmittedPhilHealthIdOrMDRForm", c => c.Boolean());
            AddColumn("dbo.Employees", "SubmittedPagIbigIdOrMIDNo", c => c.Boolean());
            AddColumn("dbo.Employees", "SubmittedTINIdOr1902Form", c => c.Boolean());
            AddColumn("dbo.Employees", "SubmittedBirthCertificate", c => c.Boolean());
            AddColumn("dbo.Employees", "SubmittedMarriageCertification", c => c.Boolean());
            AddColumn("dbo.Employees", "SubmittedBirthCertificateOfChildren", c => c.Boolean());
            AddColumn("dbo.Employees", "SubmittedDiplomaOrTCR", c => c.Boolean());
            AddColumn("dbo.Employees", "SubmittedPreEmploymentMedicalResult", c => c.Boolean());
            AddColumn("dbo.Employees", "SubmittedSSSLoanVerification", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "SubmittedSSSLoanVerification");
            DropColumn("dbo.Employees", "SubmittedPreEmploymentMedicalResult");
            DropColumn("dbo.Employees", "SubmittedDiplomaOrTCR");
            DropColumn("dbo.Employees", "SubmittedBirthCertificateOfChildren");
            DropColumn("dbo.Employees", "SubmittedMarriageCertification");
            DropColumn("dbo.Employees", "SubmittedBirthCertificate");
            DropColumn("dbo.Employees", "SubmittedTINIdOr1902Form");
            DropColumn("dbo.Employees", "SubmittedPagIbigIdOrMIDNo");
            DropColumn("dbo.Employees", "SubmittedPhilHealthIdOrMDRForm");
            DropColumn("dbo.Employees", "SubmittedSSSIdOrED1Form");
            DropColumn("dbo.Employees", "SubmittedBarangayClearance");
            DropColumn("dbo.Employees", "SubmittedPoliceClearance");
            DropColumn("dbo.Employees", "SubmittedNBIClearance");
            DropColumn("dbo.Employees", "SubmittedIdPictures");
            DropColumn("dbo.Employees", "SubmittedBiodata");
        }
    }
}

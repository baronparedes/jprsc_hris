using JPRSC.HRIS.Models;
using System.Data.Entity.ModelConfiguration;

namespace JPRSC.HRIS.Infrastructure.Data.EntityConfiguration
{
    public class CompanyProfileConfiguration : EntityTypeConfiguration<CompanyProfile>
    {
        public CompanyProfileConfiguration()
        {
            HasMany(cp => cp.UsersAllowed)
                .WithMany(u => u.AllowedCompanies)
                .Map(config =>
                {
                    config.ToTable("AllowedUserCompanies");
                    config.MapLeftKey("CompanyProfileId");
                    config.MapRightKey("UserId");
                });
        }
    }
}

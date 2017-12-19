using JPRSC.HRIS.Models;
using System.Data.Entity.ModelConfiguration;

namespace JPRSC.HRIS.Infrastructure.Data.EntityConfiguration
{
    public class CompanyConfiguration : EntityTypeConfiguration<Company>
    {
        public CompanyConfiguration()
        {
            HasMany(cp => cp.UsersAllowed)
                .WithMany(u => u.AllowedCompanies)
                .Map(config =>
                {
                    config.ToTable("AllowedUserCompanies");
                    config.MapLeftKey("CompanyId");
                    config.MapRightKey("UserId");
                });
        }
    }
}

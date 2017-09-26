using JPRSC.HRIS.Models;
using System.Data.Entity.ModelConfiguration;

namespace JPRSC.HRIS.Infrastructure.Data.EntityConfiguration
{
    public class CustomRoleConfiguration : EntityTypeConfiguration<CustomRole>
    {
        public CustomRoleConfiguration()
        {
            HasMany(u => u.Users)
                .WithMany(r => r.CustomRoles)
                .Map(config =>
                {
                    config.ToTable("UserCustomRoles");
                    config.MapLeftKey("CustomRoleId");
                    config.MapRightKey("UserId");
                });
        }
    }
}
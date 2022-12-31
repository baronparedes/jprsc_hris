using JPRSC.HRIS.Models;
using System.Data.Entity.ModelConfiguration;

namespace JPRSC.HRIS.Infrastructure.Data.EntityConfiguration
{
    public class EmployeeConfiguration : EntityTypeConfiguration<Employee>
    {
        public EmployeeConfiguration()
        {
            Ignore(e => e.FullName);
        }
    }
}

using JPRSC.HRIS.Models;
using System.Data.Entity.ModelConfiguration;

namespace JPRSC.HRIS.Infrastructure.Data.EntityConfiguration
{
    public class PayrollRecordConfiguration : EntityTypeConfiguration<PayrollRecord>
    {
        public PayrollRecordConfiguration()
        {
            Ignore(p => p.NetPayValue);
        }
    }
}
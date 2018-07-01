using System.Configuration;

namespace JPRSC.HRIS.Infrastructure.Configuration
{
    public class ConnectionStrings
    {
        public static string ApplicationDbContext => ConfigurationManager.ConnectionStrings["ApplicationDbContext"].ConnectionString;
    }
}
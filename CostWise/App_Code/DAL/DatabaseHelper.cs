using System.Configuration;
using System.Data.SqlClient;

namespace CostWise.App_Code.DAL
{
    public static class DatabaseHelper
    {
        private const string ConnectionStringName = "CostWiseConnectionString";
        public static SqlConnection GetConnection()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
                throw new ConfigurationErrorsException("Missing connection string: " + ConnectionStringName);
            return new SqlConnection(settings.ConnectionString);
        }
    }
}
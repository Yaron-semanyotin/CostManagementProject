using System.Data.SqlClient;

namespace CostWise.App_Code.DAL
{
    public static class DatabaseConnectionTestDAL
    {
        public static void TestConnection()
        {
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
            }
        }
    }
}
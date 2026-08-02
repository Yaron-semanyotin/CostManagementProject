using System.Data;
using System.Data.SqlClient;
namespace CostWise.App_Code.DAL
{
    public static class RegistrationDAL
    {
        public static bool UsernameExists(string username)
        {
            const string query = @"SELECT CASE WHEN EXISTS
            (
            SELECT 1
            FROM dbo.T_Users
            WHERE Username = @Username
            )
            THEN CAST(1 AS bit)
            ELSE CAST(0 AS bit)
            END;";
            bool usernameExists;
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
                    connection.Open();
                    usernameExists = (bool)command.ExecuteScalar();
                }
            }
            return usernameExists;
        }
        public static void CreateBusinessAndUser(string businessName, string username, string passwordHash)
        {
            const string insertBusinessQuery = @" DECLARE @UtcNow datetime2 = SYSUTCDATETIME();
            INSERT INTO dbo.T_Businesses(BusinessName,CreatedAtUtc,UpdatedAtUtc)
            OUTPUT INSERTED.BusinessId
            VALUES(@BusinessName,@UtcNow,@UtcNow);";

            const string insertUserQuery = @"INSERT INTO dbo.T_Users(BusinessId,Username,PasswordHash,CreatedAtUtc)
            VALUES(@BusinessId,@Username,@PasswordHash,SYSUTCDATETIME());";

            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int businessId;
                        using (SqlCommand businessCommand = new SqlCommand(insertBusinessQuery, connection, transaction))
                        {
                            businessCommand.Parameters.Add("@BusinessName", SqlDbType.NVarChar, 150).Value = businessName;
                            businessId = (int)businessCommand.ExecuteScalar();
                        }
                        using (SqlCommand userCommand = new SqlCommand(insertUserQuery, connection, transaction))
                        {
                            userCommand.Parameters.Add("@BusinessId", SqlDbType.Int).Value = businessId;
                            userCommand.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
                            userCommand.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = passwordHash;
                            userCommand.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
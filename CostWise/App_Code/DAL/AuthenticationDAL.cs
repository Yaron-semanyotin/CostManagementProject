using CostWise.App_Code.BLL;
using System.Data;
using System.Data.SqlClient;

namespace CostWise.App_Code.DAL
{
    public static class AuthenticationDAL
    {
        public static User GetUserByUsername(string username) // Get User by Username
        {
            // SELECT query for all the user info
            const string query = @"SELECT
            UserId,
            BusinessId,
            Username,
            PasswordHash,
            CreatedAtUtc
            FROM dbo.T_Users
            WHERE Username = @Username;";
            using (SqlConnection connection = DatabaseHelper.GetConnection()) // Creates the connection and guarantees its disposal
            {
                using (SqlCommand command = new SqlCommand(query, connection)) // Connects between the query and the connection
                {
                    command.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
                    connection.Open(); // Opens the connection
                    using (SqlDataReader reader = command.ExecuteReader()) // Execute the SELECT query and returns SqlDataReader
                    {
                        if (!reader.Read()) // if Read return false there's no user and we're returning null
                            return null;
                        return new User // Creating new user object with all the user fields
                        {
                            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                            BusinessId = reader.GetInt32(reader.GetOrdinal("BusinessId")),
                            Username = reader.GetString(reader.GetOrdinal("Username")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                            CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc"))
                        };
                    }
                }
            }
        }
    }
}
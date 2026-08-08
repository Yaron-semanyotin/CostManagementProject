using CostWise.App_Code.BLL;
using System.Data;
using System.Data.SqlClient;

namespace CostWise.App_Code.DAL
{
    public static class BusinessDAL
    {
        public static Business GetBusinessByUserId(int userId)
        {
            const string query = @"
            SELECT
                b.BusinessId,
                b.BusinessName,
                b.LogoPath,
                b.CreatedAtUtc,
                b.UpdatedAtUtc
            FROM dbo.T_Users AS u
            INNER JOIN dbo.T_Businesses AS b
                ON b.BusinessId = u.BusinessId
            WHERE u.UserId = @UserId;";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }
                        int logoPathOrdinal = reader.GetOrdinal("LogoPath");
                        return new Business
                        {
                            BusinessId = reader.GetInt32(reader.GetOrdinal("BusinessId")),
                            BusinessName = reader.GetString(reader.GetOrdinal("BusinessName")),
                            LogoPath = reader.IsDBNull(logoPathOrdinal) ? null : reader.GetString(logoPathOrdinal),
                            CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc")),
                            UpdatedAtUtc = reader.GetDateTime(reader.GetOrdinal("UpdatedAtUtc"))
                        };
                    }
                }
            }
        }
        public static bool UpdateBusinessName(int userId, string businessName)
        {
            const string query = @"
            UPDATE b
            SET
                b.BusinessName = @BusinessName,
                b.UpdatedAtUtc = SYSUTCDATETIME()
            FROM dbo.T_Businesses AS b
            INNER JOIN dbo.T_Users AS u
                ON u.BusinessId = b.BusinessId
            WHERE u.UserId = @UserId;";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@BusinessName", SqlDbType.NVarChar, 150).Value = businessName;
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows == 1;
                }
            }
        }
        public static bool UpdateBusinessLogoPath(int userId, string logoPath)
        {
            const string query = @"
            UPDATE b
            SET
                b.LogoPath = @LogoPath,
                b.UpdatedAtUtc = SYSUTCDATETIME()
            FROM dbo.T_Businesses AS b
            INNER JOIN dbo.T_Users AS u
                ON u.BusinessId = b.BusinessId
            WHERE u.UserId = @UserId;";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@LogoPath", SqlDbType.NVarChar, 260).Value = logoPath;
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows == 1;
                }
            }
        }
    }
}
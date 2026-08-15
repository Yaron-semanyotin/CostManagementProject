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
                b.ShowYieldUnitSelection,
                b.DefaultRecipeMeasurementUnitId,
                b.VatRatePercent,
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
                        int defaultRecipeMeasurementUnitIdOrdinal = reader.GetOrdinal("DefaultRecipeMeasurementUnitId");
                        return new Business
                        {
                            BusinessId = reader.GetInt32(reader.GetOrdinal("BusinessId")),
                            BusinessName = reader.GetString(reader.GetOrdinal("BusinessName")),
                            LogoPath = reader.IsDBNull(logoPathOrdinal) ? null : reader.GetString(logoPathOrdinal),
                            ShowYieldUnitSelection = reader.GetBoolean(reader.GetOrdinal("ShowYieldUnitSelection")),
                            DefaultRecipeMeasurementUnitId = reader.IsDBNull(defaultRecipeMeasurementUnitIdOrdinal) ? (int?)null : reader.GetInt32(defaultRecipeMeasurementUnitIdOrdinal),
                            VatRatePercent = reader.GetDecimal(reader.GetOrdinal("VatRatePercent")),
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
        public static bool UpdateBusinessSettings(
    int userId,
    bool showYieldUnitSelection,
    int? defaultRecipeMeasurementUnitId,
    decimal vatRatePercent)
        {
            const string query = @"
    UPDATE b
    SET
        b.ShowYieldUnitSelection = @ShowYieldUnitSelection,
        b.DefaultRecipeMeasurementUnitId = @DefaultRecipeMeasurementUnitId,
        b.VatRatePercent = @VatRatePercent,
        b.UpdatedAtUtc = SYSUTCDATETIME()
    FROM dbo.T_Businesses AS b
    INNER JOIN dbo.T_Users AS u
        ON u.BusinessId = b.BusinessId
    WHERE u.UserId = @UserId
        AND
        (
            @DefaultRecipeMeasurementUnitId IS NULL
            OR EXISTS
            (
                SELECT 1
                FROM dbo.T_MeasurementUnits AS mu
                WHERE mu.MeasurementUnitId =
                    @DefaultRecipeMeasurementUnitId
                    AND
                    (
                        mu.BusinessId IS NULL
                        OR mu.BusinessId = b.BusinessId
                    )
            )
        );";

            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@ShowYieldUnitSelection", SqlDbType.Bit).Value = showYieldUnitSelection;
                    SqlParameter defaultRecipeUnitParameter = command.Parameters.Add("@DefaultRecipeMeasurementUnitId", SqlDbType.Int);
                    defaultRecipeUnitParameter.Value = defaultRecipeMeasurementUnitId.HasValue ? (object)defaultRecipeMeasurementUnitId.Value : System.DBNull.Value;
                    SqlParameter vatRateParameter = command.Parameters.Add("@VatRatePercent", SqlDbType.Decimal);
                    vatRateParameter.Precision = 5;
                    vatRateParameter.Scale = 2;
                    vatRateParameter.Value = vatRatePercent;
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
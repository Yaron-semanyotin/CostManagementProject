using System.Data.SqlClient;
using System.Collections.Generic;
using CostWise.App_Code.BLL;
using System;
using System.Data;

namespace CostWise.App_Code.DAL
{
    public static class MeasurementUnitDAL
    {
        public static List<MeasurementUnit> GetSystemUnits()
        {
            List<MeasurementUnit> systemUnits = new List<MeasurementUnit>(); // Empty list to save the selected cols
            // Select query
            const string query = @"SELECT
            MeasurementUnitId,
            BusinessId,
            UnitName,
            UnitFamily,
            ConversionFactorToBase,
            CreateAtUtc AS CreatedAtUtc,
            UpdatedAtUtc
            FROM dbo.T_MeasurementUnits
            WHERE BusinessId IS NULL;";
            using (SqlConnection connection = DatabaseHelper.GetConnection()) // connection to DB
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open(); // Opens the connection
                    using (SqlDataReader reader = command.ExecuteReader()) // data reader to return the select query
                    {
                        while (reader.Read()) // loop that loops until Read() returns false
                        {
                            MeasurementUnit unit = new MeasurementUnit(); // creating MeasurementUnit object to hold the info
                            unit.MeasurementUnitId = reader.GetInt32(reader.GetOrdinal("MeasurementUnitId")); // finding the index by the name
                            int businessIdOrdinal = reader.GetOrdinal("BusinessId"); // reading from Int column
                            if (reader.IsDBNull(businessIdOrdinal))
                                unit.BusinessId = null;
                            else
                                unit.BusinessId = reader.GetInt32(businessIdOrdinal);
                            unit.UnitName = reader.GetString(reader.GetOrdinal("UnitName")); // finding the unitname
                            unit.UnitFamily = reader.GetString(reader.GetOrdinal("UnitFamily")); // finding the unitfamily
                            unit.ConversionFactorToBase = reader.GetDecimal(reader.GetOrdinal("ConversionFactorToBase")); // finding ConversionFactorToBase
                            unit.CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc")); // finding CreatedAtUtc
                            unit.UpdatedAtUtc = reader.GetDateTime(reader.GetOrdinal("UpdatedAtUtc")); // finding UpdatedAtUtc
                            systemUnits.Add(unit); // every time the loop enters it adds into the object the unit types
                        }
                    }
                }
            }
            return systemUnits; // return the object systemUnits
        }
        public static List<MeasurementUnit> GetAvailableUnits(int userId)
        {
            List<MeasurementUnit> availableUnits = new List<MeasurementUnit>();
            const string query = @"SELECT
            mu.MeasurementUnitId,
            mu.BusinessId,
            mu.UnitName,
            mu.UnitFamily,
            mu.ConversionFactorToBase,
            mu.CreateAtUtc AS CreatedAtUtc,
            mu.UpdatedAtUtc
            FROM dbo.T_Users AS u
            INNER JOIN dbo.T_MeasurementUnits AS mu
            ON mu.BusinessId IS NULL
            OR mu.BusinessId = u.BusinessId
            WHERE u.UserId = @UserId;";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MeasurementUnit unit = new MeasurementUnit();
                            unit.MeasurementUnitId = reader.GetInt32(reader.GetOrdinal("MeasurementUnitId"));
                            int businessIdOrdinal = reader.GetOrdinal("BusinessId");
                            if (reader.IsDBNull(businessIdOrdinal))
                            {
                                unit.BusinessId = null;
                            }
                            else
                            {
                                unit.BusinessId = reader.GetInt32(businessIdOrdinal);
                            }
                            unit.UnitName = reader.GetString(reader.GetOrdinal("UnitName"));
                            unit.UnitFamily = reader.GetString(reader.GetOrdinal("UnitFamily"));
                            unit.ConversionFactorToBase = reader.GetDecimal(reader.GetOrdinal("ConversionFactorToBase"));
                            unit.CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc"));
                            unit.UpdatedAtUtc = reader.GetDateTime(reader.GetOrdinal("UpdatedAtUtc"));
                            availableUnits.Add(unit);
                        }
                    }
                }
            }
            return availableUnits;
        }
        public static bool UnitNameExistsForUser(int userId, string unitName)
        {
            const string query = @"SELECT CASE WHEN EXISTS
            (
                SELECT 1
                FROM dbo.T_Users AS u
                INNER JOIN dbo.T_MeasurementUnits AS mu
                ON mu.BusinessId IS NULL
                OR mu.BusinessId = u.BusinessId
                WHERE u.UserId = @UserId
                AND mu.UnitName = @UnitName
            )
            THEN CAST(1 AS bit)
            ELSE CAST(0 AS bit)
            END;";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@UnitName", SqlDbType.NVarChar, 50).Value = unitName;
                    connection.Open();
                    return (bool)command.ExecuteScalar();
                }
            }
        }
        public static bool UnitNameExistsForUserExceptUnit(int userId, int measurementUnitId, string unitName)
        {
            const string query = @"SELECT CASE WHEN EXISTS
            (
                SELECT 1
                FROM dbo.T_Users AS u
                INNER JOIN dbo.T_MeasurementUnits AS mu
                    ON mu.BusinessId IS NULL
                    OR mu.BusinessId = u.BusinessId
                WHERE u.UserId = @UserId
                    AND mu.UnitName = @UnitName
                    AND mu.MeasurementUnitId <> @MeasurementUnitId
            )
            THEN CAST(1 AS bit)
            ELSE CAST(0 AS bit)
            END;";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@MeasurementUnitId", SqlDbType.Int).Value = measurementUnitId;
                    command.Parameters.Add("@UnitName", SqlDbType.NVarChar, 50).Value = unitName;
                    connection.Open();
                    return (bool)command.ExecuteScalar();
                }
            }
        }
        public static bool CreateCustomUnit(int userId, string unitName, string unitFamily, decimal conversionFactorToBase)
        {
            const string query = @"INSERT INTO dbo.T_MeasurementUnits
                (
                    BusinessId,
                    UnitName,
                    UnitFamily,
                    ConversionFactorToBase
                )
                SELECT
                    u.BusinessId,
                    @UnitName,
                    @UnitFamily,
                    @ConversionFactorToBase
                FROM dbo.T_Users AS u
                WHERE u.UserId = @UserId;";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@UnitName", SqlDbType.NVarChar, 50).Value = unitName;
                    command.Parameters.Add("@UnitFamily", SqlDbType.NVarChar, 20).Value = unitFamily;
                    SqlParameter conversionFactorParameter = command.Parameters.Add("@ConversionFactorToBase", SqlDbType.Decimal);
                    conversionFactorParameter.Precision = 18;
                    conversionFactorParameter.Scale = 6;
                    conversionFactorParameter.Value = conversionFactorToBase;
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows == 1;
                }
            }
        }
        public static bool UpdateCustomUnit(int userId, int measurementUnitId, string unitName, string unitFamily, decimal conversionFactorToBase)
        {
            const string query = @"UPDATE mu
            SET
                mu.UnitName = @UnitName,
                mu.UnitFamily = @UnitFamily,
                mu.ConversionFactorToBase = @ConversionFactorToBase,
                mu.UpdatedAtUtc = SYSUTCDATETIME()
            FROM dbo.T_MeasurementUnits AS mu
            INNER JOIN dbo.T_Users AS u
                ON u.BusinessId = mu.BusinessId
            WHERE u.UserId = @UserId
            AND mu.MeasurementUnitId = @MeasurementUnitId
            AND mu.BusinessId IS NOT NULL;";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@MeasurementUnitId", SqlDbType.Int).Value = measurementUnitId;
                    command.Parameters.Add("@UnitName", SqlDbType.NVarChar, 50).Value = unitName;
                    command.Parameters.Add("@UnitFamily", SqlDbType.NVarChar, 20).Value = unitFamily;
                    SqlParameter conversionFactorParameter = command.Parameters.Add("@ConversionFactorToBase", SqlDbType.Decimal);
                    conversionFactorParameter.Precision = 18;
                    conversionFactorParameter.Scale = 6;
                    conversionFactorParameter.Value = conversionFactorToBase;
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows == 1;
                }
            }
        }
        public static bool IsCustomUnitInUseForUser(int userId, int measurementUnitId)
        {
            const string query = @"SELECT CASE WHEN EXISTS
            (
                SELECT 1
                FROM dbo.T_Users AS u
                INNER JOIN dbo.T_MeasurementUnits AS  mu
                    ON mu.BusinessId = u.BusinessId
                WHERE u.UserId = @UserId
                    AND mu.MeasurementUnitId = @MeasurementUnitId
                    AND mu.BusinessId IS NOT NULL
                    AND
                    (
                        EXISTS
                        (
                            SELECT 1
                            FROM dbo.T_Ingredients AS i
                            WHERE i.PackageUnitId = mu.MeasurementUnitId
                        )
                        OR EXISTS
                        (
                            SELECT 1
                            FROM dbo.T_RecipeIngredients AS ri
                            WHERE ri.MeasurementUnitId = mu.MeasurementUnitId
                        )
                    )
            )
            THEN CAST(1 AS bit)
            ELSE CAST(0 AS bit)
            END;";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@MeasurementUnitId", SqlDbType.Int).Value = measurementUnitId;
                    connection.Open();
                    return (bool)command.ExecuteScalar();
                }
            }
        }
        public static bool DeleteCustomUnit(int userId, int measurementUnitId)
        {
            const string query = @"DELETE mu
            FROM dbo.T_MeasurementUnits AS mu
            INNER JOIN dbo.T_Users AS u
                ON u.BusinessId = mu.BusinessId
            WHERE u.UserId = @UserId
                AND mu.MeasurementUnitId = @MeasurementUnitId
                AND mu.BusinessId IS NOT NULL
                AND NOT EXISTS
                (
                    SELECT 1
                    FROM dbo.T_Ingredients AS i
                    WHERE i.PackageUnitId = mu.MeasurementUnitId
                )
                AND NOT EXISTS
                (
                    SELECT 1
                    FROM dbo.T_RecipeIngredients AS ri
                    WHERE ri.MeasurementUnitId = mu.MeasurementUnitId
                );";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@MeasurementUnitId", SqlDbType.Int).Value = measurementUnitId;
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows == 1;
                }
            }
        }
    }
}
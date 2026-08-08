using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CostWise.App_Code.BLL;

namespace CostWise.App_Code.DAL
{
    public static class IngredientDAL
    {
        public static List<Ingredient> GetIngredientsForUser(int userId)
        {
            List<Ingredient> ingredients = new List<Ingredient>();
            const string query = @"SELECT
            i.IngredientId,
            i.BusinessId,
            i.IngredientName,
            i.PackagePrice,
            i.PackageQuantity,
            i.PackageUnitId,
            i.IsActive,
            i.CreatedAtUtc,
            i.UpdatedAtUtc
            FROM dbo.T_Users AS u
            INNER JOIN dbo.T_Ingredients AS i
                ON i.BusinessId = u.BusinessId
            WHERE u.UserId = @UserId
            ORDER BY i.IngredientName;";
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
                            Ingredient ingredient = new Ingredient();
                            ingredient.IngredientId = reader.GetInt32(reader.GetOrdinal("IngredientId"));
                            ingredient.BusinessId = reader.GetInt32(reader.GetOrdinal("BusinessId"));
                            ingredient.IngredientName = reader.GetString(reader.GetOrdinal("IngredientName"));
                            ingredient.PackagePrice = reader.GetDecimal(reader.GetOrdinal("PackagePrice"));
                            ingredient.PackageQuantity = reader.GetDecimal(reader.GetOrdinal("PackageQuantity"));
                            ingredient.PackageUnitId = reader.GetInt32(reader.GetOrdinal("PackageUnitId"));
                            ingredient.IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
                            ingredient.CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc"));
                            ingredient.UpdatedAtUtc = reader.GetDateTime(reader.GetOrdinal("UpdatedAtUtc"));
                            ingredients.Add(ingredient);
                        }
                    }
                    return ingredients;
                }
            }
        }
        public static bool CreateIngredient(int userId, string ingredientName, decimal packagePrice, decimal packageQuantity, int packageUnitId)
        {
            const string query = @"INSERT INTO dbo.T_Ingredients
            (
                BusinessId,
                IngredientName,
                PackagePrice,
                PackageQuantity,
                PackageUnitId
            )
            SELECT
                u.BusinessId,
                @IngredientName,
                @PackagePrice,
                @PackageQuantity,
                mu.MeasurementUnitId
            FROM dbo.T_Users AS u
            INNER JOIN dbo.T_MeasurementUnits AS mu
                ON mu.MeasurementUnitId = @PackageUnitId
                AND
                (
                    mu.BusinessId IS NULL
                    OR mu.BusinessId = u.BusinessId
                )
            WHERE u.UserId = @UserId
            AND NOT EXISTS
            (
                SELECT 1
                FROM dbo.T_Ingredients AS existingIngredient
                    WITH(UPDLOCK, HOLDLOCK)
                WHERE existingIngredient.BusinessId = u.BusinessId
                    AND existingIngredient.IngredientName = @IngredientName
            );";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@IngredientName", SqlDbType.NVarChar, 150).Value = ingredientName;
                    SqlParameter packagePriceParameter = command.Parameters.Add("@PackagePrice", SqlDbType.Decimal);
                    packagePriceParameter.Precision = 18;
                    packagePriceParameter.Scale = 2;
                    packagePriceParameter.Value = packagePrice;
                    SqlParameter packageQuantityParameter = command.Parameters.Add("@PackageQuantity", SqlDbType.Decimal);
                    packageQuantityParameter.Precision = 18;
                    packageQuantityParameter.Scale = 6;
                    packageQuantityParameter.Value = packageQuantity;
                    command.Parameters.Add("@PackageUnitId", SqlDbType.Int).Value = packageUnitId;
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows == 1;
                }
            }
        }
        public static bool UpdateIngredient(int userId, int ingredientId, string ingredientName, decimal packagePrice, decimal packageQuantity, int packageUnitId)
        {
            const string query = @"UPDATE i
            SET
                i.IngredientName = @IngredientName,
                i.PackagePrice = @PackagePrice,
                i.PackageQuantity = @PackageQuantity,
                i.PackageUnitId = @PackageUnitId,
                i.UpdatedAtUtc = SYSUTCDATETIME()
            FROM dbo.T_Ingredients AS i
            INNER JOIN dbo.T_Users AS u
                ON u.BusinessId = i.BusinessId
            INNER JOIN dbo.T_MeasurementUnits AS mu
                ON mu.MeasurementUnitId = @PackageUnitId
                AND
                (
                    mu.BusinessId IS NULL
                    OR mu.BusinessId = u.BusinessId
                )
            WHERE u.UserId = @UserId
                AND i.IngredientId = @IngredientId
                AND i.IsActive = 1
                AND NOT EXISTS
                (
                    SELECT 1
                    FROM dbo.T_Ingredients AS otherIngredient
                        WITH (UPDLOCK, HOLDLOCK)
                    WHERE otherIngredient.BusinessId =
                            i.BusinessId
                        AND otherIngredient.IngredientName =
                            @IngredientName
                        AND otherIngredient.IngredientId <>
                            i.IngredientId
                );";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@IngredientId", SqlDbType.Int).Value = ingredientId;
                    command.Parameters.Add("@IngredientName", SqlDbType.NVarChar, 150).Value = ingredientName;
                    SqlParameter packagePriceParameter = command.Parameters.Add("@PackagePrice", SqlDbType.Decimal);
                    packagePriceParameter.Precision = 18;
                    packagePriceParameter.Scale = 2;
                    packagePriceParameter.Value = packagePrice;
                    SqlParameter packageQuantityParameter = command.Parameters.Add("@PackageQuantity", SqlDbType.Decimal);
                    packageQuantityParameter.Precision = 18;
                    packageQuantityParameter.Scale = 6;
                    packageQuantityParameter.Value = packageQuantity;
                    command.Parameters.Add("@PackageUnitId", SqlDbType.Int).Value = packageUnitId;
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows == 1;
                }
            }
        }
        public static string DeactivateIngredient(int userId, int ingredientId)
        {
            const string query = @"UPDATE i
            SET
                i.IsActive = 0,
                i.UpdatedAtUtc = SYSUTCDATETIME()
            OUTPUT INSERTED.IngredientName
            FROM dbo.T_Ingredients AS i
            INNER JOIN dbo.T_Users AS u
                ON u.BusinessId = i.BusinessId
            WHERE u.UserId = @UserId
                AND i.IngredientId = @IngredientId
                AND i.IsActive = 1;";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@IngredientId", SqlDbType.Int).Value = ingredientId;
                    connection.Open();
                    object deactivatedIngredientName = command.ExecuteScalar();
                    if (deactivatedIngredientName == null)
                    {
                        return null;
                    }
                    return Convert.ToString(deactivatedIngredientName);
                }
            }
        }
        public static Ingredient GetIngredientByNameForUser(int userId, string ingredientName)
        {
            const string query = @"SELECT TOP (1)
                i.IngredientId,
                i.BusinessId,
                i.IngredientName,
                i.PackagePrice,
                i.PackageQuantity,
                i.PackageUnitId,
                i.IsActive,
                i.CreatedAtUtc,
                i.UpdatedAtUtc
            FROM dbo.T_Users AS u
            INNER JOIN dbo.T_Ingredients AS i
                ON i.BusinessId = u.BusinessId
            WHERE u.UserId = @UserId
                AND i.IngredientName = @IngredientName
            ORDER BY
                i.IsActive DESC,
                i.IngredientId;";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@IngredientName", SqlDbType.NVarChar, 150).Value = ingredientName;
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }
                        Ingredient ingredient = new Ingredient();
                        ingredient.IngredientId = reader.GetInt32(reader.GetOrdinal("IngredientId"));
                        ingredient.BusinessId = reader.GetInt32(reader.GetOrdinal("BusinessId"));
                        ingredient.IngredientName = reader.GetString(reader.GetOrdinal("IngredientName"));
                        ingredient.PackagePrice = reader.GetDecimal(reader.GetOrdinal("PackagePrice"));
                        ingredient.PackageQuantity = reader.GetDecimal(reader.GetOrdinal("PackageQuantity"));
                        ingredient.PackageUnitId = reader.GetInt32(reader.GetOrdinal("PackageUnitId"));
                        ingredient.IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
                        ingredient.CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc"));
                        ingredient.UpdatedAtUtc = reader.GetDateTime(reader.GetOrdinal("UpdatedAtUtc"));
                        return ingredient;
                    }
                }
            }
        }
        public static bool ReactivateIngredient(int userId, int ingredientId, string ingredientName, decimal packagePrice, decimal packageQuantity, int packageUnitId)
        {
            const string query = @"UPDATE i
            SET
                i.IngredientName = @IngredientName,
                i.PackagePrice = @PackagePrice,
                i.PackageQuantity = @PackageQuantity,
                i.PackageUnitId = @PackageUnitId,
                i.IsActive = 1,
                i.UpdatedAtUtc = SYSUTCDATETIME()
            FROM dbo.T_Ingredients AS i
            INNER JOIN dbo.T_Users AS u
                ON u.BusinessId = i.BusinessId
            INNER JOIN dbo.T_MeasurementUnits AS mu
                ON mu.MeasurementUnitId = @PackageUnitId
                AND
                (
                    mu.BusinessId IS NULL
                    OR mu.BusinessId = u.BusinessId
                )
            WHERE u.UserId = @UserId
                AND i.IngredientId = @IngredientId
                AND i.IsActive = 0
                AND NOT EXISTS
                (
                    SELECT 1
                    FROM dbo.T_Ingredients AS otherIngredient
                        WITH (UPDLOCK, HOLDLOCK)
                    WHERE otherIngredient.BusinessId =
                            i.BusinessId
                        AND otherIngredient.IngredientName =
                            @IngredientName
                        AND otherIngredient.IngredientId <>
                            i.IngredientId
                );";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@IngredientId", SqlDbType.Int).Value = ingredientId;
                    command.Parameters.Add("@IngredientName", SqlDbType.NVarChar, 150).Value = ingredientName;
                    SqlParameter packagePriceParameter = command.Parameters.Add("@PackagePrice", SqlDbType.Decimal);
                    packagePriceParameter.Precision = 18;
                    packagePriceParameter.Scale = 2;
                    packagePriceParameter.Value = packagePrice;
                    SqlParameter packageQuantityParameter = command.Parameters.Add("@PackageQuantity", SqlDbType.Decimal);
                    packageQuantityParameter.Precision = 18;
                    packageQuantityParameter.Scale = 6;
                    packageQuantityParameter.Value = packageQuantity;
                    command.Parameters.Add("@PackageUnitId", SqlDbType.Int).Value = packageUnitId;
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows == 1;
                }
            }
        }
    }
}
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CostWise.App_Code.BLL;

namespace CostWise.App_Code.DAL
{
    public static class RecipeIngredientDAL
    {
        public static List<RecipeIngredient> GetRecipeIngredientsForProduct(int userId, int productId)
        {
            List<RecipeIngredient> recipeIngredients = new List<RecipeIngredient>();
            const string query = @"SELECT
                ri.RecipeIngredientId,
                ri.ProductId,
                ri.IngredientId,
                ri.Quantity,
                ri.MeasurementUnitId,
                ri.ManualIngredientCostOverride,
                ri.SortOrder
            FROM dbo.T_Users AS u
            INNER JOIN dbo.T_Products AS p
                ON p.BusinessId = u.BusinessId
            INNER JOIN dbo.T_RecipeIngredients AS ri
                ON ri.ProductId = p.ProductId
            WHERE u.UserId = @UserId
                AND p.ProductId = @ProductId
            ORDER BY
                ri.SortOrder,
                ri.RecipeIngredientId;";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RecipeIngredient recipeIngredient = new RecipeIngredient();
                            recipeIngredient.RecipeIngredientId = reader.GetInt32(reader.GetOrdinal("RecipeIngredientId"));
                            recipeIngredient.ProductId = reader.GetInt32(reader.GetOrdinal("ProductId"));
                            recipeIngredient.IngredientId = reader.GetInt32(reader.GetOrdinal("IngredientId"));
                            recipeIngredient.Quantity = reader.GetDecimal(reader.GetOrdinal("Quantity"));
                            recipeIngredient.MeasurementUnitId = reader.GetInt32(reader.GetOrdinal("MeasurementUnitId"));
                            int manualCostOrdinal = reader.GetOrdinal("ManualIngredientCostOverride");
                            recipeIngredient.ManualIngredientCostOverride = reader.IsDBNull(manualCostOrdinal) ? (decimal?)null : reader.GetDecimal(manualCostOrdinal);
                            recipeIngredient.SortOrder = reader.GetInt32(reader.GetOrdinal("SortOrder"));
                            recipeIngredients.Add(recipeIngredient);
                        }
                    }
                }
            }
            return recipeIngredients;
        }
        public static bool CreateRecipeIngredient(int userId, int productId, int ingredientId, decimal quantity, int measurementUnitId, int sortOrder, decimal? manualIngredientCostOverride = null)
        {
            const string query = @"INSERT INTO dbo.T_RecipeIngredients
            (
                ProductId,
                IngredientId,
                Quantity,
                MeasurementUnitId,
                ManualIngredientCostOverride,
                SortOrder
            )
            SELECT
                p.ProductId,
                i.IngredientId,
                @Quantity,
                mu.MeasurementUnitId,
                @ManualIngredientCostOverride,
                @SortOrder
            FROM dbo.T_Users AS u
            INNER JOIN dbo.T_Products AS p
                ON p.BusinessId = u.BusinessId
                AND p.ProductId = @ProductId
            INNER JOIN dbo.T_Ingredients AS i
                ON i.BusinessId = u.BusinessId
                AND i.IngredientId = @IngredientId
            INNER JOIN dbo.T_MeasurementUnits AS mu
                ON mu.MeasurementUnitId = @MeasurementUnitId
                AND
                (
                    mu.BusinessId IS NULL
                    OR mu.BusinessId = u.BusinessId
                )
            WHERE u.UserId = @UserId
                AND p.IsActive = 1
                AND i.IsActive = 1;";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                    command.Parameters.Add("@IngredientId", SqlDbType.Int).Value = ingredientId;
                    SqlParameter quantityParameter = command.Parameters.Add("@Quantity", SqlDbType.Decimal);
                    quantityParameter.Precision = 18;
                    quantityParameter.Scale = 6;
                    quantityParameter.Value = quantity;
                    command.Parameters.Add("@MeasurementUnitId", SqlDbType.Int).Value = measurementUnitId;
                    SqlParameter manualCostParameter = command.Parameters.Add("@ManualIngredientCostOverride", SqlDbType.Decimal);
                    manualCostParameter.Precision = 28;
                    manualCostParameter.Scale = 12;
                    manualCostParameter.Value = manualIngredientCostOverride.HasValue ? (object)manualIngredientCostOverride.Value : System.DBNull.Value;
                    command.Parameters.Add("@SortOrder", SqlDbType.Int).Value = sortOrder;
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows == 1;
                }
            }
        }
        public static bool UpdateRecipeIngredient(int userId, int productId, int recipeIngredientId, int ingredientId, decimal quantity, int measurementUnitId, int sortOrder, decimal? manualIngredientCostOverride = null)
        {
            const string query = @"UPDATE ri
            SET
                ri.IngredientId = i.IngredientId,
                ri.Quantity = @Quantity,
                ri.MeasurementUnitId = mu.MeasurementUnitId,
                ri.ManualIngredientCostOverride = @ManualIngredientCostOverride,
                ri.SortOrder = @SortOrder
            FROM dbo.T_RecipeIngredients AS ri
            INNER JOIN dbo.T_Products AS p
                ON p.ProductId = ri.ProductId
            INNER JOIN dbo.T_Users AS u
                ON u.BusinessId = p.BusinessId
            INNER JOIN dbo.T_Ingredients AS i
                ON i.BusinessId = u.BusinessId
                AND i.IngredientId = @IngredientId
            INNER JOIN dbo.T_MeasurementUnits AS mu
                ON mu.MeasurementUnitId = @MeasurementUnitId
                AND
                (
                    mu.BusinessId IS NULL
                    OR mu.BusinessId = u.BusinessId
                )
            WHERE u.UserId = @UserId
                AND p.ProductId = @ProductId
                AND ri.RecipeIngredientId = @RecipeIngredientId
                AND p.IsActive = 1
                AND i.IsActive = 1;";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                    command.Parameters.Add("@RecipeIngredientId", SqlDbType.Int).Value = recipeIngredientId;
                    command.Parameters.Add("@IngredientId", SqlDbType.Int).Value = ingredientId;
                    SqlParameter quantityParameter = command.Parameters.Add("@Quantity", SqlDbType.Decimal);
                    quantityParameter.Precision = 18;
                    quantityParameter.Scale = 6;
                    quantityParameter.Value = quantity;
                    command.Parameters.Add("@MeasurementUnitId", SqlDbType.Int).Value = measurementUnitId;
                    SqlParameter manualCostParameter = command.Parameters.Add("@ManualIngredientCostOverride", SqlDbType.Decimal);
                    manualCostParameter.Precision = 28;
                    manualCostParameter.Scale = 12;
                    manualCostParameter.Value = manualIngredientCostOverride.HasValue ? (object)manualIngredientCostOverride.Value : System.DBNull.Value;
                    command.Parameters.Add("@SortOrder", SqlDbType.Int).Value = sortOrder;
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows == 1;
                }
            }
        }
        public static bool DeleteRecipeIngredient(int userId, int productId, int recipeIngredientId)
        {
            const string query = @"DELETE ri
            FROM dbo.T_RecipeIngredients AS ri
            INNER JOIN dbo.T_Products AS p
                ON p.ProductId = ri.ProductId
            INNER JOIN dbo.T_Users AS u
                ON u.BusinessId = p.BusinessId
            WHERE u.UserId = @UserId
                AND p.ProductId = @ProductId
                AND ri.RecipeIngredientId = @RecipeIngredientId
                AND p.IsActive = 1;";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                    command.Parameters.Add("@RecipeIngredientId", SqlDbType.Int).Value = recipeIngredientId;
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows == 1;
                }
            }
        }
    }
}
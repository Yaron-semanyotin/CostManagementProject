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
        
        
        
    }
}
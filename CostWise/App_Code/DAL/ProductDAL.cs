using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CostWise.App_Code.BLL;
using System;
namespace CostWise.App_Code.DAL
{
    public static class ProductDAL
    {
        public static List<Product> GetProductsForUser(int userId)
        {
            List<Product> products = new List<Product>();

            const string query = @"SELECT
                p.ProductId,
                p.BusinessId,
                p.ProductName,
                p.YieldQuantity,
                p.YieldUnitLabel,
                p.InstructionsHtml,
                p.ImagePath,
                p.IsActive,
                p.CreatedAtUtc,
                p.UpdatedAtUtc
            FROM dbo.T_Users AS u
            INNER JOIN dbo.T_Products AS p
                ON p.BusinessId = u.BusinessId
            WHERE u.UserId = @UserId
            ORDER BY p.ProductName;";

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
                            Product product = new Product();
                            product.ProductId = reader.GetInt32(reader.GetOrdinal("ProductId"));
                            product.BusinessId = reader.GetInt32(reader.GetOrdinal("BusinessId"));
                            product.ProductName = reader.GetString(reader.GetOrdinal("ProductName"));
                            product.YieldQuantity = reader.GetDecimal(reader.GetOrdinal("YieldQuantity"));
                            product.YieldUnitLabel = reader.GetString(reader.GetOrdinal("YieldUnitLabel"));
                            int instructionsOrdinal = reader.GetOrdinal("InstructionsHtml");
                            product.InstructionsHtml = reader.IsDBNull(instructionsOrdinal) ? null : reader.GetString(instructionsOrdinal);
                            int imagePathOrdinal = reader.GetOrdinal("ImagePath");
                            product.ImagePath = reader.IsDBNull(imagePathOrdinal) ? null : reader.GetString(imagePathOrdinal);
                            product.IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
                            product.CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc"));
                            product.UpdatedAtUtc = reader.GetDateTime(reader.GetOrdinal("UpdatedAtUtc"));
                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }
        public static int CreateProduct(int userId, string productName, decimal yieldQuantity, string yieldUnitLabel)
        {
            const string query = @"INSERT INTO dbo.T_Products
            (
                BusinessId,
                ProductName,
                YieldQuantity,
                YieldUnitLabel
            )
            OUTPUT INSERTED.ProductId
            SELECT
                u.BusinessId,
                @ProductName,
                @YieldQuantity,
                @YieldUnitLabel
            FROM dbo.T_Users AS u
            WHERE u.UserId = @UserId;";

            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@ProductName", SqlDbType.NVarChar, 150).Value = productName;
                    SqlParameter yieldQuantityParameter = command.Parameters.Add("@YieldQuantity", SqlDbType.Decimal);
                    yieldQuantityParameter.Precision = 18;
                    yieldQuantityParameter.Scale = 6;
                    yieldQuantityParameter.Value = yieldQuantity;
                    command.Parameters.Add("@YieldUnitLabel", SqlDbType.NVarChar, 50).Value = yieldUnitLabel;
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result == null || result == DBNull.Value)
                    {
                        return 0;
                    }
                    return Convert.ToInt32(result);
                }
            }
        }
        public static int CreateProductWithRecipe(int userId, string productName, decimal yieldQuantity, string yieldUnitLabel, List<RecipeIngredientInput> recipeIngredients)
        {
            const string productQuery = @"INSERT INTO dbo.T_Products
            (
                BusinessId,
                ProductName,
                YieldQuantity,
                YieldUnitLabel
            )
            OUTPUT INSERTED.ProductId
            SELECT
                u.BusinessId,
                @ProductName,
                @YieldQuantity,
                @YieldUnitLabel
            FROM dbo.T_Users AS u
            WHERE u.UserId = @UserId;";
            const string recipeIngredientQuery =
                @"INSERT INTO dbo.T_RecipeIngredients
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
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        object productIdResult;
                        using (SqlCommand productCommand = new SqlCommand(productQuery, connection, transaction))
                        {
                            productCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                            productCommand.Parameters.Add("@ProductName", SqlDbType.NVarChar, 150).Value = productName;
                            SqlParameter yieldQuantityParameter = productCommand.Parameters.Add("@YieldQuantity", SqlDbType.Decimal);
                            yieldQuantityParameter.Precision = 18;
                            yieldQuantityParameter.Scale = 6;
                            yieldQuantityParameter.Value = yieldQuantity;
                            productCommand.Parameters.Add("@YieldUnitLabel", SqlDbType.NVarChar, 50).Value = yieldUnitLabel;
                            productIdResult = productCommand.ExecuteScalar();
                        }
                        if (productIdResult == null || productIdResult == DBNull.Value)
                        {
                            transaction.Rollback();
                            return 0;
                        }
                        int productId = Convert.ToInt32(productIdResult);
                        for (int index = 0; index < recipeIngredients.Count; index++)
                        {
                            RecipeIngredientInput recipeIngredient = recipeIngredients[index];
                            using (SqlCommand recipeCommand = new SqlCommand(recipeIngredientQuery, connection, transaction))
                            {
                                recipeCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                                recipeCommand.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                                recipeCommand.Parameters.Add("@IngredientId", SqlDbType.Int).Value = recipeIngredient.IngredientId;
                                SqlParameter quantityParameter = recipeCommand.Parameters.Add("@Quantity", SqlDbType.Decimal);
                                quantityParameter.Precision = 18;
                                quantityParameter.Scale = 6;
                                quantityParameter.Value = recipeIngredient.Quantity;
                                recipeCommand.Parameters.Add("@MeasurementUnitId", SqlDbType.Int).Value = recipeIngredient.MeasurementUnitId;
                                SqlParameter manualCostParameter = recipeCommand.Parameters.Add("@ManualIngredientCostOverride", SqlDbType.Decimal);
                                manualCostParameter.Precision = 28;
                                manualCostParameter.Scale = 12;
                                manualCostParameter.Value = recipeIngredient.ManualIngredientCostOverride.HasValue ? (object)recipeIngredient.ManualIngredientCostOverride.Value : DBNull.Value;
                                recipeCommand.Parameters.Add("@SortOrder", SqlDbType.Int).Value = index + 1;
                                int affectedRows = recipeCommand.ExecuteNonQuery();
                                if (affectedRows != 1)
                                {
                                    transaction.Rollback();
                                    return 0;
                                }
                            }
                        }
                        transaction.Commit();
                        return productId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public static bool UpdateProductWithRecipe(int userId, int productId, string productName, decimal yieldQuantity, string yieldUnitLabel, List<RecipeIngredientInput> recipeIngredients)
        {
            const string updateProductQuery = @"
            UPDATE p
            SET
                p.ProductName = @ProductName,
                p.YieldQuantity = @YieldQuantity,
                p.YieldUnitLabel = @YieldUnitLabel,
                p.UpdatedAtUtc = SYSUTCDATETIME()
            FROM dbo.T_Products AS p
            INNER JOIN dbo.T_Users AS u
                ON u.BusinessId = p.BusinessId
            WHERE u.UserId = @UserId
                AND p.ProductId = @ProductId
                AND p.IsActive = 1;";
            const string deleteRecipeQuery = @"
            DELETE ri
            FROM dbo.T_RecipeIngredients AS ri
            INNER JOIN dbo.T_Products AS p
                ON p.ProductId = ri.ProductId
            INNER JOIN dbo.T_Users AS u
                ON u.BusinessId = p.BusinessId
            WHERE u.UserId = @UserId
                AND p.ProductId = @ProductId
                AND p.IsActive = 1;";
            const string insertRecipeIngredientQuery = @"
            INSERT INTO dbo.T_RecipeIngredients
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
                AND p.IsActive = 1;";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int updatedProductRows;
                        using (SqlCommand productCommand = new SqlCommand(updateProductQuery, connection, transaction))
                        {
                            productCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                            productCommand.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                            productCommand.Parameters.Add("@ProductName", SqlDbType.NVarChar, 150).Value = productName;
                            SqlParameter yieldQuantityParameter = productCommand.Parameters.Add("@YieldQuantity", SqlDbType.Decimal);
                            yieldQuantityParameter.Precision = 18;
                            yieldQuantityParameter.Scale = 6;
                            yieldQuantityParameter.Value = yieldQuantity;
                            productCommand.Parameters.Add("@YieldUnitLabel", SqlDbType.NVarChar, 50).Value = yieldUnitLabel;
                            updatedProductRows = productCommand.ExecuteNonQuery();
                        }
                        if (updatedProductRows != 1)
                        {
                            transaction.Rollback();
                            return false;
                        }
                        using (SqlCommand deleteRecipeCommand = new SqlCommand(deleteRecipeQuery, connection, transaction))
                        {
                            deleteRecipeCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                            deleteRecipeCommand.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                            deleteRecipeCommand.ExecuteNonQuery();
                        }
                        for (int index = 0; index < recipeIngredients.Count; index++)
                        {
                            RecipeIngredientInput recipeIngredient = recipeIngredients[index];
                            using (SqlCommand recipeCommand = new SqlCommand(insertRecipeIngredientQuery, connection, transaction))
                            {
                                recipeCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                                recipeCommand.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                                recipeCommand.Parameters.Add("@IngredientId", SqlDbType.Int).Value = recipeIngredient.IngredientId;
                                SqlParameter quantityParameter = recipeCommand.Parameters.Add("@Quantity", SqlDbType.Decimal);
                                quantityParameter.Precision = 18;
                                quantityParameter.Scale = 6;
                                quantityParameter.Value = recipeIngredient.Quantity;
                                recipeCommand.Parameters.Add("@MeasurementUnitId", SqlDbType.Int).Value = recipeIngredient.MeasurementUnitId;
                                SqlParameter manualCostParameter = recipeCommand.Parameters.Add("@ManualIngredientCostOverride", SqlDbType.Decimal);
                                manualCostParameter.Precision = 28;
                                manualCostParameter.Scale = 12;
                                manualCostParameter.Value = recipeIngredient.ManualIngredientCostOverride.HasValue
                                    ? (object)recipeIngredient.ManualIngredientCostOverride.Value : DBNull.Value;
                                recipeCommand.Parameters.Add("@SortOrder", SqlDbType.Int).Value = index + 1;
                                int insertedRows = recipeCommand.ExecuteNonQuery();
                                if (insertedRows != 1)
                                {
                                    transaction.Rollback();
                                    return false;
                                }
                            }
                        }
                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public static bool DeactivateProduct(int userId, int productId)
        {
            const string query = @"UPDATE p
            SET
                p.IsActive = 0,
                p.UpdatedAtUtc = SYSUTCDATETIME()
            FROM dbo.T_Products AS p
            INNER JOIN dbo.T_Users AS u
                ON u.BusinessId = p.BusinessId
            WHERE u.UserId = @UserId
                AND p.ProductId = @ProductId
                AND p.IsActive = 1;";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows == 1;
                }
            }
        }
        public static bool ReactivateProduct(int userId, int productId, string productName, decimal yieldQuantity, string yieldUnitLabel)
        {
            const string query = @"UPDATE p
            SET
                p.ProductName = @ProductName,
                p.YieldQuantity = @YieldQuantity,
                p.YieldUnitLabel = @YieldUnitLabel,
                p.IsActive = 1,
                p.UpdatedAtUtc = SYSUTCDATETIME()
            FROM dbo.T_Products AS p
            INNER JOIN dbo.T_Users AS u
                ON u.BusinessId = p.BusinessId
            WHERE u.UserId = @UserId
                AND p.ProductId = @ProductId
                AND p.IsActive = 0;";
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                    command.Parameters.Add("@ProductName", SqlDbType.NVarChar, 150).Value = productName;
                    SqlParameter yieldQuantityParameter = command.Parameters.Add("@YieldQuantity", SqlDbType.Decimal);
                    yieldQuantityParameter.Precision = 18;
                    yieldQuantityParameter.Scale = 6;
                    yieldQuantityParameter.Value = yieldQuantity;
                    command.Parameters.Add("@YieldUnitLabel", SqlDbType.NVarChar, 50).Value = yieldUnitLabel;
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows == 1;
                }
            }
        }
    }
}
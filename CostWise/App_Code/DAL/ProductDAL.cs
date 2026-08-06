using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CostWise.App_Code.BLL;

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
        public static bool CreateProduct(int userId, string productName, decimal yieldQuantity, string yieldUnitLabel)
        {
            const string query = @"INSERT INTO dbo.T_Products
            (
                BusinessId,
                ProductName,
                YieldQuantity,
                YieldUnitLabel
            )
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
                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows == 1;
                }
            }
        }
        public static bool UpdateProduct(int userId, int productId, string productName, decimal yieldQuantity, string yieldUnitLabel)
        {
            const string query = @"UPDATE p
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
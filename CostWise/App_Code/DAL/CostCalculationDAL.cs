using CostWise.App_Code.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CostWise.App_Code.DAL
{
    public static class CostCalculationDAL
    {
        private const string InsertCalculationQuery = @"
        INSERT INTO dbo.T_CostCalculations
        (
            BusinessId,
            ProductId,
            ProductNameSnapshot,
            YieldQuantitySnapshot,
            YieldUnitLabelSnaphot,
            TotalIngredientCostSnapshot,
            CostPerYieldUnitSnapshot,
            CalculatedAtUtc
        )
        OUTPUT INSERTED.CostCalculationId
        SELECT
            u.BusinessId,
            p.ProductId,
            @ProductNameSnapshot,
            @YieldQuantitySnapshot,
            @YieldUnitLabelSnapshot,
            @TotalIngredientCostSnapshot,
            @CostPerYieldUnitSnapshot,
            @CalculatedAtUtc
        FROM dbo.T_Users AS u
        INNER JOIN dbo.T_Products AS p
            ON p.BusinessId = u.BusinessId
        WHERE u.UserId = @UserId
            AND p.ProductId = @ProductId;";
        private const string InsertCalculationItemQuery = @"
        INSERT INTO dbo.T_CostCalculationItems
        (
            CostCalculationId,
            IngredientId,
            IngredientNameSnapshot,
            PackagePriceSnapshot,
            PackageQuantitySnapshot,
            PackageUnitNameSnapshot,
            PackageUnitFamilySnapshot,
            PackageUnitConversionFactorSnapshot,
            RecipeQuantitySnapshot,
            RecipeUnitNameSnapshot,
            RecipeUnitFamilySnapshot,
            RecipeUnitConversionFactorSnapshot,
            BaseUnitNameSnapshot,
            PackageQuantityInBaseUnitSnapshot,
            RecipeQuantityInBaseUnitSnapshot,
            PricePerBaseUnitSnapshot,
            IngredientCostSnapshot,
            SortOrderSnapshot
        )
        SELECT
            @CostCalculationId,
            ingredient.IngredientId,
            @IngredientNameSnapshot,
            @PackagePriceSnapshot,
            @PackageQuantitySnapshot,
            @PackageUnitNameSnapshot,
            @PackageUnitFamilySnapshot,
            @PackageUnitConversionFactorSnapshot,
            @RecipeQuantitySnapshot,
            @RecipeUnitNameSnapshot,
            @RecipeUnitFamilySnapshot,
            @RecipeUnitConversionFactorSnapshot,
            @BaseUnitNameSnapshot,
            @PackageQuantityInBaseUnitSnapshot,
            @RecipeQuantityInBaseUnitSnapshot,
            @PricePerBaseUnitSnapshot,
            @IngredientCostSnapshot,
            @SortOrderSnapshot
        FROM dbo.T_Users AS u
        INNER JOIN dbo.T_CostCalculations AS calculation
            ON calculation.BusinessId = u.BusinessId
        INNER JOIN dbo.T_Ingredients AS ingredient
            ON ingredient.BusinessId = u.BusinessId
        WHERE u.UserId = @UserId
            AND calculation.CostCalculationId = @CostCalculationId
            AND ingredient.IngredientId = @IngredientId;";
        private const string SelectCalculationsQuery = @"
        SELECT
            calculation.CostCalculationId,
            calculation.BusinessId,
            calculation.ProductId,
            calculation.ProductNameSnapshot,
            calculation.YieldQuantitySnapshot,
            calculation.YieldUnitLabelSnaphot
                AS YieldUnitLabelSnapshot,
            calculation.TotalIngredientCostSnapshot,
            calculation.CostPerYieldUnitSnapshot,
            calculation.CalculatedAtUtc
        FROM dbo.T_Users AS u
        INNER JOIN dbo.T_CostCalculations AS calculation
            ON calculation.BusinessId = u.BusinessId
        WHERE u.UserId = @UserId
        ORDER BY
            calculation.CalculatedAtUtc DESC,
            calculation.CostCalculationId DESC;";
        private const string SelectCalculationByIdQuery = @"
        SELECT
            calculation.CostCalculationId,
            calculation.BusinessId,
            calculation.ProductId,
            calculation.ProductNameSnapshot,
            calculation.YieldQuantitySnapshot,
            calculation.YieldUnitLabelSnaphot
                AS YieldUnitLabelSnapshot,
            calculation.TotalIngredientCostSnapshot,
            calculation.CostPerYieldUnitSnapshot,
            calculation.CalculatedAtUtc
        FROM dbo.T_Users AS u
        INNER JOIN dbo.T_CostCalculations AS calculation
            ON calculation.BusinessId = u.BusinessId
        WHERE u.UserId = @UserId
            AND calculation.CostCalculationId =
                @CostCalculationId;";
        private const string SelectCalculationItemsQuery = @"
        SELECT
            item.CostCalculationItemId,
            item.CostCalculationId,
            item.IngredientId,
            item.IngredientNameSnapshot,
            item.PackagePriceSnapshot,
            item.PackageQuantitySnapshot,
            item.PackageUnitNameSnapshot,
            item.PackageUnitFamilySnapshot,
            item.PackageUnitConversionFactorSnapshot,
            item.RecipeQuantitySnapshot,
            item.RecipeUnitNameSnapshot,
            item.RecipeUnitFamilySnapshot,
            item.RecipeUnitConversionFactorSnapshot,
            item.BaseUnitNameSnapshot,
            item.PackageQuantityInBaseUnitSnapshot,
            item.RecipeQuantityInBaseUnitSnapshot,
            item.PricePerBaseUnitSnapshot,
            item.IngredientCostSnapshot,
            item.SortOrderSnapshot
        FROM dbo.T_Users AS u
        INNER JOIN dbo.T_CostCalculations AS calculation
            ON calculation.BusinessId = u.BusinessId
        INNER JOIN dbo.T_CostCalculationItems AS item
            ON item.CostCalculationId =
                calculation.CostCalculationId
        WHERE u.UserId = @UserId
            AND calculation.CostCalculationId =
                @CostCalculationId
        ORDER BY
            item.SortOrderSnapshot,
            item.CostCalculationItemId;";
        private static CostCalculation MapCalculation(SqlDataReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            return new CostCalculation
            {
                CostCalculationId = reader.GetInt32(reader.GetOrdinal("CostCalculationId")),
                BusinessId = reader.GetInt32(reader.GetOrdinal("BusinessId")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                ProductNameSnapshot = reader.GetString(reader.GetOrdinal("ProductNameSnapshot")),
                YieldQuantitySnapshot = reader.GetDecimal(reader.GetOrdinal("YieldQuantitySnapshot")),
                YieldUnitLabelSnapshot = reader.GetString(reader.GetOrdinal("YieldUnitLabelSnapshot")),
                TotalIngredientCostSnapshot = reader.GetDecimal(reader.GetOrdinal("TotalIngredientCostSnapshot")),
                CostPerYieldUnitSnapshot = reader.GetDecimal(reader.GetOrdinal("CostPerYieldUnitSnapshot")),
                CalculatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CalculatedAtUtc"))
            };
        }
        private static CostCalculationItem MapCalculationItem(SqlDataReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            return new CostCalculationItem
            {
                CostCalculationItemId = reader.GetInt32(reader.GetOrdinal("CostCalculationItemId")),
                CostCalculationId = reader.GetInt32(reader.GetOrdinal("CostCalculationId")),
                IngredientId = reader.GetInt32(reader.GetOrdinal("IngredientId")),
                IngredientNameSnapshot = reader.GetString(reader.GetOrdinal("IngredientNameSnapshot")),
                PackagePriceSnapshot = reader.GetDecimal(reader.GetOrdinal("PackagePriceSnapshot")),
                PackageQuantitySnapshot = reader.GetDecimal(reader.GetOrdinal("PackageQuantitySnapshot")),
                PackageUnitNameSnapshot = reader.GetString(reader.GetOrdinal("PackageUnitNameSnapshot")),
                PackageUnitFamilySnapshot = reader.GetString(reader.GetOrdinal("PackageUnitFamilySnapshot")),
                PackageUnitConversionFactorSnapshot = reader.GetDecimal(reader.GetOrdinal("PackageUnitConversionFactorSnapshot")),
                RecipeQuantitySnapshot = reader.GetDecimal(reader.GetOrdinal("RecipeQuantitySnapshot")),
                RecipeUnitNameSnapshot = reader.GetString(reader.GetOrdinal("RecipeUnitNameSnapshot")),
                RecipeUnitFamilySnapshot = reader.GetString(reader.GetOrdinal("RecipeUnitFamilySnapshot")),
                RecipeUnitConversionFactorSnapshot = reader.GetDecimal(reader.GetOrdinal("RecipeUnitConversionFactorSnapshot")),
                BaseUnitNameSnapshot = reader.GetString(reader.GetOrdinal("BaseUnitNameSnapshot")),
                PackageQuantityInBaseUnitSnapshot = reader.GetDecimal(reader.GetOrdinal("PackageQuantityInBaseUnitSnapshot")),
                RecipeQuantityInBaseUnitSnapshot = reader.GetDecimal(reader.GetOrdinal("RecipeQuantityInBaseUnitSnapshot")),
                PricePerBaseUnitSnapshot = reader.GetDecimal(reader.GetOrdinal("PricePerBaseUnitSnapshot")),
                IngredientCostSnapshot = reader.GetDecimal(reader.GetOrdinal("IngredientCostSnapshot")),
                SortOrderSnapshot = reader.GetInt32(reader.GetOrdinal("SortOrderSnapshot"))
            };
        }
        public static List<CostCalculation> GetCalculationsForUser(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            List<CostCalculation> calculations = new List<CostCalculation>();
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(SelectCalculationsQuery, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            calculations.Add(MapCalculation(reader));
                        }
                    }
                }
            }
            return calculations;
        }
        public static CostCalculationResult GetCalculationDetailsForUser(int userId, int costCalculationId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (costCalculationId <= 0)
            {
                throw new ArgumentException("מזהה החישוב אינו תקין.");
            }
            CostCalculation calculation = null;
            List<CostCalculationItem> items = new List<CostCalculationItem>();
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (SqlCommand calculationCommand = new SqlCommand(SelectCalculationByIdQuery, connection))
                {
                    calculationCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    calculationCommand.Parameters.Add("@CostCalculationId", SqlDbType.Int).Value = costCalculationId;
                    using (SqlDataReader reader = calculationCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            calculation = MapCalculation(reader);
                        }
                    }
                }
                if (calculation == null)
                {
                    return null;
                }
                using (SqlCommand itemsCommand = new SqlCommand(SelectCalculationItemsQuery, connection))
                {
                    itemsCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    itemsCommand.Parameters.Add("@CostCalculationId", SqlDbType.Int).Value = costCalculationId;
                    using (SqlDataReader reader = itemsCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(MapCalculationItem(reader));
                        }
                    }
                }
            }
            return new CostCalculationResult { Calculation = calculation, Items = items };
        }
        private static void AddDecimalParameter(SqlCommand command, string parameterName, byte precision, byte scale, decimal value)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            SqlParameter parameter = command.Parameters.Add(parameterName, SqlDbType.Decimal);
            parameter.Precision = precision;
            parameter.Scale = scale;
            parameter.Value = value;
        }
        private static void AddCalculationParameters(SqlCommand command, int userId, CostCalculation calculation)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }
            command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            command.Parameters.Add("@ProductId", SqlDbType.Int).Value = calculation.ProductId;
            command.Parameters.Add("@ProductNameSnapshot", SqlDbType.NVarChar, 150).Value = calculation.ProductNameSnapshot;
            AddDecimalParameter(command, "@YieldQuantitySnapshot", 18, 6, calculation.YieldQuantitySnapshot);
            command.Parameters.Add("@YieldUnitLabelSnapshot", SqlDbType.NVarChar, 50).Value = calculation.YieldUnitLabelSnapshot;
            AddDecimalParameter(command, "@TotalIngredientCostSnapshot", 28, 12, calculation.TotalIngredientCostSnapshot);
            AddDecimalParameter(command, "@CostPerYieldUnitSnapshot", 28, 12, calculation.CostPerYieldUnitSnapshot);
            SqlParameter calculatedAtUtcParameter = command.Parameters.Add("@CalculatedAtUtc", SqlDbType.DateTime2);
            calculatedAtUtcParameter.Scale = 7;
            calculatedAtUtcParameter.Value = calculation.CalculatedAtUtc;
        }
        private static void AddCalculationItemParameters(SqlCommand command, int userId, int costCalculationId, CostCalculationItem item)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            command.Parameters.Add("@CostCalculationId", SqlDbType.Int).Value = costCalculationId;
            command.Parameters.Add("@IngredientId", SqlDbType.Int).Value = item.IngredientId;
            command.Parameters.Add("@IngredientNameSnapshot", SqlDbType.NVarChar, 150).Value = item.IngredientNameSnapshot;
            AddDecimalParameter(command, "@PackagePriceSnapshot", 18, 2, item.PackagePriceSnapshot);
            AddDecimalParameter(command, "@PackageQuantitySnapshot", 18, 6, item.PackageQuantitySnapshot);
            command.Parameters.Add("@PackageUnitNameSnapshot", SqlDbType.NVarChar, 50).Value = item.PackageUnitNameSnapshot;
            command.Parameters.Add("@PackageUnitFamilySnapshot", SqlDbType.NVarChar, 20).Value = item.PackageUnitFamilySnapshot;
            AddDecimalParameter(command, "@PackageUnitConversionFactorSnapshot", 18, 6, item.PackageUnitConversionFactorSnapshot);
            AddDecimalParameter(command, "@RecipeQuantitySnapshot", 18, 6, item.RecipeQuantitySnapshot);
            command.Parameters.Add("@RecipeUnitNameSnapshot", SqlDbType.NVarChar, 50).Value = item.RecipeUnitNameSnapshot;
            command.Parameters.Add("@RecipeUnitFamilySnapshot", SqlDbType.NVarChar, 20).Value = item.RecipeUnitFamilySnapshot;
            AddDecimalParameter(command, "@RecipeUnitConversionFactorSnapshot", 18, 6, item.RecipeUnitConversionFactorSnapshot);
            command.Parameters.Add("@BaseUnitNameSnapshot", SqlDbType.NVarChar, 50).Value = item.BaseUnitNameSnapshot;
            AddDecimalParameter(command, "@PackageQuantityInBaseUnitSnapshot", 28, 12, item.PackageQuantityInBaseUnitSnapshot);
            AddDecimalParameter(command, "@RecipeQuantityInBaseUnitSnapshot", 28, 12, item.RecipeQuantityInBaseUnitSnapshot);
            AddDecimalParameter(command, "@PricePerBaseUnitSnapshot", 28, 12, item.PricePerBaseUnitSnapshot);
            AddDecimalParameter(command, "@IngredientCostSnapshot", 28, 12, item.IngredientCostSnapshot);
            command.Parameters.Add("@SortOrderSnapshot", SqlDbType.Int).Value = item.SortOrderSnapshot;
        }
        public static int SaveCalculation(int userId, CostCalculationResult result)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }
            if (result.Calculation == null)
            {
                throw new InvalidOperationException("כותרת החישוב חסרה.");
            }
            if (result.Items == null || result.Items.Count == 0)
            {
                throw new InvalidOperationException("פירוט החישוב חסר.");
            }
            using (SqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int costCalculationId;
                        using (SqlCommand calculationCommand = new SqlCommand(InsertCalculationQuery, connection, transaction))
                        {
                            AddCalculationParameters(calculationCommand, userId, result.Calculation);
                            object insertedId = calculationCommand.ExecuteScalar();
                            if (insertedId == null || insertedId == DBNull.Value)
                            {
                                throw new InvalidOperationException("לא ניתן לשמור את כותרת החישוב.");
                            }
                            costCalculationId = Convert.ToInt32(insertedId);
                        }
                        foreach (CostCalculationItem item in result.Items)
                        {
                            using (SqlCommand itemCommand = new SqlCommand(InsertCalculationItemQuery, connection, transaction))
                            {
                                AddCalculationItemParameters(itemCommand, userId, costCalculationId, item);
                                int affectedRows = itemCommand.ExecuteNonQuery();
                                if (affectedRows != 1)
                                {
                                    throw new InvalidOperationException("לא ניתן לשמור את פירוט החישוב.");
                                }
                            }
                        }
                        transaction.Commit();
                        return costCalculationId;
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
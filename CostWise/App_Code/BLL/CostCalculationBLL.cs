using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CostWise.App_Code.DAL;
namespace CostWise.App_Code.BLL
{
    public static class CostCalculationBLL
    {
        private static Product GetProductForCalculation(int userId, int productId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (productId <= 0)
            {
                throw new ArgumentException("מזהה המוצר אינו תקין.");
            }
            List<Product> products = ProductBLL.GetProductsForUser(userId);
            Product product = products.Find(item => item.ProductId == productId);
            if (product == null || !product.IsActive)
            {
                throw new InvalidOperationException("המוצר לא נמצא, אינו פעיל או אינו שייך לעסק שלך.");
            }
            return product;
        }
        private static List<RecipeIngredient> GetRecipeForCalculation(int userId, int productId)
        {
            List<RecipeIngredient> recipeIngredients = RecipeIngredientBLL.GetRecipeIngredientsForProduct(userId, productId);
            if (recipeIngredients == null || recipeIngredients.Count == 0)
            {
                throw new InvalidOperationException("לא ניתן לחשב עלות למוצר שאין לו מתכון.");
            }
            return recipeIngredients;
        }
        private static Ingredient GetIngredientForCalculation(List<Ingredient> ingredients, int ingredientId)
        {
            if (ingredients == null)
            {
                throw new ArgumentNullException(nameof(ingredients));
            }
            if (ingredientId <= 0)
            {
                throw new ArgumentException("מזהה הרכיב אינו תקין.");
            }
            Ingredient ingredient = ingredients.Find(item => item.IngredientId == ingredientId);
            if (ingredient == null)
            {
                throw new InvalidOperationException("הרכיב לא נמצא או אינו שייך לעסק שלך.");
            }
            if (ingredient.PackageQuantity <= 0)
            {
                throw new InvalidOperationException("כמות האריזה של הרכיב חייבת להיות גדולה מאפס.");
            }
            if (ingredient.PackagePrice < 0)
            {
                throw new InvalidOperationException("מחיר האריזה של הרכיב אינו יכול להיות שלילי.");
            }
            return ingredient;
        }
        private static MeasurementUnit GetUnitForCalculation(List<MeasurementUnit> availableUnits, int measurementUnitId)
        {
            if (availableUnits == null)
            {
                throw new ArgumentNullException(nameof(availableUnits));
            }
            if (measurementUnitId <= 0)
            {
                throw new ArgumentException("מזהה יחידת המידה אינו תקין.");
            }
            MeasurementUnit unit = availableUnits.Find(item => item.MeasurementUnitId == measurementUnitId);
            if (unit == null)
            {
                throw new InvalidOperationException("יחידת המידה לא נמצאה או אינה זמינה לעסק שלך.");
            }
            return unit;
        }
        private static MeasurementUnit GetBaseUnitForCalculation(List<MeasurementUnit> availableUnits, string unitFamily)
        {
            if (availableUnits == null)
            {
                throw new ArgumentNullException(nameof(availableUnits));
            }
            if (string.IsNullOrWhiteSpace(unitFamily))
            {
                throw new ArgumentException("משפחת יחידת המידה חסרה.");
            }
            MeasurementUnit baseUnit = availableUnits.Find(item => !item.BusinessId.HasValue && item.ConversionFactorToBase == 1m && string.Equals(item.UnitFamily, unitFamily, StringComparison.OrdinalIgnoreCase));
            if (baseUnit == null)
            {
                throw new InvalidOperationException("לא נמצאה יחידת בסיס מתאימה למשפחת המידה.");
            }
            return baseUnit;
        }
        private static decimal CalculateIngredientCost(decimal recipeQuantityInBaseUnit, decimal packageQuantityInBaseUnit, decimal packagePrice)
        {
            if (recipeQuantityInBaseUnit <= 0)
            {
                throw new ArgumentException("כמות הרכיב במתכון חייבת להיות גדולה מאפס.");
            }
            if (packageQuantityInBaseUnit <= 0)
            {
                throw new ArgumentException("כמות האריזה ביחידת הבסיס חייבת להיות גדולה מאפס.");
            }
            if (packagePrice < 0)
            {
                throw new ArgumentException("מחיר האריזה אינו יכול להיות שלילי.");
            }
            return recipeQuantityInBaseUnit / packageQuantityInBaseUnit * packagePrice;
        }
        public static decimal CalculateIngredientCostPreview(int userId, int ingredientId, decimal quantity, int measurementUnitId, int? productId = null)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (ingredientId <= 0)
            {
                throw new ArgumentException("מזהה הרכיב אינו תקין.");
            }
            if (quantity <= 0)
            {
                throw new ArgumentException("כמות הרכיב חייבת להיות גדולה מאפס.");
            }
            if (quantity > 999999999999.999999m)
            {
                throw new ArgumentException("כמות הרכיב גדולה מדי.");
            }
            if (decimal.Round(quantity, 6) != quantity)
            {
                throw new ArgumentException("כמות הרכיב יכולה להכיל עד 6 ספרות אחרי הנקודה.");
            }
            if (measurementUnitId <= 0)
            {
                throw new ArgumentException("מזהה יחידת המידה אינו תקין.");
            }
            List<Ingredient> ingredients = ProductBLL.GetIngredientsForProductBuilder(userId, productId);
            Ingredient ingredient = GetIngredientForCalculation(ingredients, ingredientId);
            List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
            MeasurementUnit packageUnit = GetUnitForCalculation(availableUnits, ingredient.PackageUnitId);
            MeasurementUnit recipeUnit = GetUnitForCalculation(availableUnits, measurementUnitId);
            UnitConversionBLL.ValidateCompatibleUnits(packageUnit, recipeUnit);
            decimal packageQuantityInBaseUnit = UnitConversionBLL.ConvertToBaseUnit(ingredient.PackageQuantity, packageUnit);
            decimal recipeQuantityInBaseUnit = UnitConversionBLL.ConvertToBaseUnit(quantity, recipeUnit);
            return CalculateIngredientCost(recipeQuantityInBaseUnit, packageQuantityInBaseUnit, ingredient.PackagePrice);
        }
        private static CostCalculationItem CreateCalculationItem(RecipeIngredient recipeIngredient, Ingredient ingredient, MeasurementUnit packageUnit, MeasurementUnit recipeUnit, MeasurementUnit baseUnit)
        {
            if (recipeIngredient == null)
            {
                throw new ArgumentNullException(nameof(recipeIngredient));
            }
            if (ingredient == null)
            {
                throw new ArgumentNullException(nameof(ingredient));
            }
            if (packageUnit == null)
            {
                throw new ArgumentNullException(nameof(packageUnit));
            }
            if (recipeUnit == null)
            {
                throw new ArgumentNullException(nameof(recipeUnit));
            }
            if (baseUnit == null)
            {
                throw new ArgumentNullException(nameof(baseUnit));
            }
            UnitConversionBLL.ValidateCompatibleUnits(packageUnit, recipeUnit);
            UnitConversionBLL.ValidateCompatibleUnits(packageUnit, baseUnit);
            decimal packageQuantityInBaseUnit = UnitConversionBLL.ConvertToBaseUnit(ingredient.PackageQuantity, packageUnit);
            decimal recipeQuantityInBaseUnit = UnitConversionBLL.ConvertToBaseUnit(recipeIngredient.Quantity, recipeUnit);
            decimal pricePerBaseUnit = ingredient.PackagePrice / packageQuantityInBaseUnit;
            decimal calculatedIngredientCost = CalculateIngredientCost(recipeQuantityInBaseUnit, packageQuantityInBaseUnit, ingredient.PackagePrice);
            if (recipeIngredient.ManualIngredientCostOverride.HasValue && recipeIngredient.ManualIngredientCostOverride.Value < 0)
            {
                throw new InvalidOperationException("מחיר ידני שנשמר במתכון אינו תקין.");
            }
            decimal ingredientCost = recipeIngredient.ManualIngredientCostOverride.HasValue ? recipeIngredient.ManualIngredientCostOverride.Value : calculatedIngredientCost;
            return new CostCalculationItem
            {
                IngredientId = ingredient.IngredientId,
                IngredientNameSnapshot = ingredient.IngredientName,
                PackagePriceSnapshot = ingredient.PackagePrice,
                PackageQuantitySnapshot = ingredient.PackageQuantity,
                PackageUnitNameSnapshot = packageUnit.UnitName,
                PackageUnitFamilySnapshot = packageUnit.UnitFamily,
                PackageUnitConversionFactorSnapshot = packageUnit.ConversionFactorToBase,
                RecipeQuantitySnapshot = recipeIngredient.Quantity,
                RecipeUnitNameSnapshot = recipeUnit.UnitName,
                RecipeUnitFamilySnapshot = recipeUnit.UnitFamily,
                RecipeUnitConversionFactorSnapshot = recipeUnit.ConversionFactorToBase,
                BaseUnitNameSnapshot = baseUnit.UnitName,
                PackageQuantityInBaseUnitSnapshot = packageQuantityInBaseUnit,
                RecipeQuantityInBaseUnitSnapshot = recipeQuantityInBaseUnit,
                PricePerBaseUnitSnapshot = pricePerBaseUnit,
                ManualIngredientCostOverrideSnapshot = recipeIngredient.ManualIngredientCostOverride,
                IngredientCostSnapshot = ingredientCost,
                SortOrderSnapshot = recipeIngredient.SortOrder
            };
        }
        private static CostCalculation CreateCalculationSummary(Product product, decimal totalIngredientCost)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            if (product.YieldQuantity <= 0)
            {
                throw new InvalidOperationException("כמות התוצר חייבת להיות גדולה מאפס.");
            }
            if (string.IsNullOrWhiteSpace(product.YieldUnitLabel))
            {
                throw new InvalidOperationException("יחידת התוצר של המוצר חסרה.");
            }
            if (totalIngredientCost < 0)
            {
                throw new ArgumentException("העלות הכוללת אינה יכולה להיות שלילית.");
            }
            decimal costPerYieldUnit = totalIngredientCost / product.YieldQuantity;
            return new CostCalculation
            {
                BusinessId = product.BusinessId,
                ProductId = product.ProductId,
                ProductNameSnapshot = product.ProductName,
                YieldQuantitySnapshot = product.YieldQuantity,
                YieldUnitLabelSnapshot = product.YieldUnitLabel,
                TotalIngredientCostSnapshot = totalIngredientCost,
                CostPerYieldUnitSnapshot = costPerYieldUnit,
                CalculatedAtUtc = DateTime.UtcNow
            };
        }
        public static CostCalculationResult CalculateProductCost(int userId, int productId)
        {
            Product product = GetProductForCalculation(userId, productId);
            List<RecipeIngredient> recipeIngredients = GetRecipeForCalculation(userId, productId);
            List<Ingredient> ingredients = IngredientBLL.GetIngredientsForUser(userId);
            List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
            List<CostCalculationItem> calculationItems = new List<CostCalculationItem>();
            decimal totalIngredientCost = 0m;
            foreach (RecipeIngredient recipeIngredient in recipeIngredients)
            {
                Ingredient ingredient = GetIngredientForCalculation(ingredients, recipeIngredient.IngredientId);
                MeasurementUnit packageUnit = GetUnitForCalculation(availableUnits, ingredient.PackageUnitId);
                MeasurementUnit recipeUnit = GetUnitForCalculation(availableUnits, recipeIngredient.MeasurementUnitId);
                MeasurementUnit baseUnit = GetBaseUnitForCalculation(availableUnits, packageUnit.UnitFamily);
                CostCalculationItem calculationItem = CreateCalculationItem(recipeIngredient, ingredient, packageUnit, recipeUnit, baseUnit);
                calculationItems.Add(calculationItem);
                totalIngredientCost += calculationItem.IngredientCostSnapshot;
            }
            CostCalculation calculation = CreateCalculationSummary(product, totalIngredientCost);
            return new CostCalculationResult { Calculation = calculation, Items = calculationItems };
        }
        public static List<Product> GetActiveProductsWithCurrentCosts(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            List<Product> products = ProductBLL.GetActiveProductsForUser(userId);
            foreach (Product product in products)
            {
                try
                {
                    CostCalculationResult result = CalculateProductCost(userId, product.ProductId);
                    if (result == null || result.Calculation == null)
                    {
                        product.CurrentTotalCost = null;
                        continue;
                    }
                    product.CurrentTotalCost = result.Calculation.TotalIngredientCostSnapshot;
                }
                catch (ArgumentException)
                {
                    product.CurrentTotalCost = null;
                }
                catch (InvalidOperationException)
                {
                    product.CurrentTotalCost = null;
                }
            }
            return products;
        }
        public static int CalculateAndSaveProductCost(int userId, int productId)
        {
            CostCalculationResult result = CalculateProductCost(userId, productId);
            return CostCalculationDAL.SaveCalculation(userId, result);
        }
        public static List<CostCalculation> GetCalculationHistory(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            return CostCalculationDAL.GetCalculationsForUser(userId);
        }
        public static CostCalculationResult GetCalculationDetails(int userId, int costCalculationId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (costCalculationId <= 0)
            {
                throw new ArgumentException("מזהה החישוב אינו תקין.");
            }
            CostCalculationResult result = CostCalculationDAL.GetCalculationDetailsForUser(userId, costCalculationId);
            if (result == null || result.Calculation == null)
            {
                throw new InvalidOperationException("החישוב לא נמצא או אינו שייך לעסק שלך.");
            }
            if (result.Items == null || result.Items.Count == 0)
            {
                throw new InvalidOperationException("פירוט החישוב ההיסטורי חסר.");
            }
            return result;
        }
    }
}
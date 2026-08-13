using System;
using System.Collections.Generic;
using CostWise.App_Code.DAL;
namespace CostWise.App_Code.BLL
{
    public static class ProductBLL
    {
        private static MeasurementUnit
    ValidateProductWithRecipeInput(int userId, string productName, decimal yieldQuantity, int yieldUnitId,
        List<RecipeIngredientInput> recipeIngredients, HashSet<int> allowedInactiveIngredientIds = null)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (string.IsNullOrWhiteSpace(productName))
            {
                throw new ArgumentException("יש להזין שם מוצר.");
            }
            if (productName.Trim().Length > 150)
            {
                throw new ArgumentException("שם המוצר יכול להכיל עד 150 תווים.");
            }
            if (yieldQuantity <= 0)
            {
                throw new ArgumentException("כמות התוצר חייבת להיות גדולה מאפס.");
            }
            if (yieldQuantity > 999999999999.999999m)
            {
                throw new ArgumentException("כמות התוצר גדולה מדי.");
            }
            if (decimal.Round(yieldQuantity, 6) != yieldQuantity)
            {
                throw new ArgumentException("כמות התוצר יכולה להכיל עד 6 ספרות אחרי הנקודה.");
            }
            if (yieldUnitId <= 0)
            {
                throw new ArgumentException("יש לבחור יחידת תוצר.");
            }
            if (recipeIngredients == null || recipeIngredients.Count == 0)
            {
                throw new ArgumentException("יש להוסיף לפחות רכיב אחד למתכון.");
            }
            List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
            MeasurementUnit yieldUnit = availableUnits.Find(unit => unit.MeasurementUnitId == yieldUnitId);
            if (yieldUnit == null)
            {
                throw new ArgumentException("יחידת התוצר שנבחרה אינה זמינה לעסק.");
            }
            List<Ingredient> ingredients = IngredientBLL.GetIngredientsForUser(userId);
            HashSet<int> selectedIngredientIds = new HashSet<int>();
            foreach (RecipeIngredientInput recipeIngredient in recipeIngredients)
            {
                if (recipeIngredient == null)
                {
                    throw new ArgumentException("אחת משורות המתכון אינה תקינה.");
                }
                if (recipeIngredient.IngredientId <= 0)
                {
                    throw new ArgumentException("יש לבחור רכיב תקין בכל שורת מתכון.");
                }
                if (!selectedIngredientIds.Add(recipeIngredient.IngredientId))
                {
                    throw new ArgumentException("לא ניתן להוסיף את אותו הרכיב יותר מפעם אחת.");
                }
                Ingredient ingredient = ingredients.Find(item => item.IngredientId == recipeIngredient.IngredientId);
                if (ingredient == null)
                {
                    throw new InvalidOperationException("אחד הרכיבים אינו שייך לעסק שלך.");
                }
                bool inactiveIngredientIsAllowed = !ingredient.IsActive && allowedInactiveIngredientIds != null && allowedInactiveIngredientIds.Contains(ingredient.IngredientId);
                if (!ingredient.IsActive && !inactiveIngredientIsAllowed)
                {
                    throw new InvalidOperationException("לא ניתן להוסיף רכיב מושבת למתכון.");
                }
                if (recipeIngredient.Quantity <= 0)
                {
                    throw new ArgumentException("הכמות בכל שורת מתכון חייבת להיות גדולה מאפס.");
                }
                if (recipeIngredient.Quantity > 999999999999.999999m)
                {
                    throw new ArgumentException("אחת מכמויות המתכון גדולה מדי.");
                }
                if (decimal.Round(recipeIngredient.Quantity, 6) != recipeIngredient.Quantity)
                {
                    throw new ArgumentException("כמות במתכון יכולה להכיל עד 6 ספרות אחרי הנקודה.");
                }
                if (recipeIngredient.ManualIngredientCostOverride.HasValue)
                {
                    decimal manualIngredientCost = recipeIngredient.ManualIngredientCostOverride.Value;
                    if (manualIngredientCost < 0)
                    {
                        throw new ArgumentException("מחיר הרכיב במתכון אינו יכול להיות שלילי.");
                    }
                    if (manualIngredientCost > 9999999999999999.999999999999m)
                    {
                        throw new ArgumentException("מחיר הרכיב במתכון גדול מדי.");
                    }
                    if (decimal.Round(manualIngredientCost, 12) != manualIngredientCost)
                    {
                        throw new ArgumentException("מחיר הרכיב במתכון יכול להכיל עד 12 ספרות אחרי הנקודה.");
                    }
                }
                if (recipeIngredient.MeasurementUnitId <= 0)
                {
                    throw new ArgumentException("יש לבחור יחידת מידה בכל שורת מתכון.");
                }
                MeasurementUnit recipeUnit = availableUnits.Find(unit => unit.MeasurementUnitId == recipeIngredient.MeasurementUnitId);
                if (recipeUnit == null)
                {
                    throw new ArgumentException("אחת מיחידות המידה אינה זמינה לעסק.");
                }
                MeasurementUnit packageUnit = availableUnits.Find(unit => unit.MeasurementUnitId == ingredient.PackageUnitId);
                if (packageUnit == null)
                {
                    throw new InvalidOperationException("יחידת האריזה של אחד הרכיבים אינה זמינה.");
                }
                if (!string.Equals(packageUnit.UnitFamily, recipeUnit.UnitFamily, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("יחידת המידה של אחד הרכיבים אינה תואמת למשפחת יחידת האריזה שלו.");
                }
            }
            return yieldUnit;
        }
        public static List<Product> GetProductsForUser(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            return ProductDAL.GetProductsForUser(userId);
        }
        public static List<Product> GetActiveProductsForUser(int userId)
        {
            List<Product> products = GetProductsForUser(userId);
            return products.FindAll(product => product.IsActive);
        }
        public static Product GetActiveProductForUser(int userId, int productId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (productId <= 0)
            {
                throw new ArgumentException("מזהה המוצר אינו תקין.");
            }
            List<Product> products = GetProductsForUser(userId);
            Product product = products.Find(item => item.ProductId == productId);
            if (product == null || !product.IsActive)
            {
                throw new InvalidOperationException("המוצר לא נמצא, אינו פעיל או אינו שייך לעסק שלך.");
            }
            return product;
        }
        public static List<Ingredient> GetIngredientsForProductBuilder(int userId, int? productId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            List<Ingredient> ingredients = IngredientBLL.GetIngredientsForUser(userId);
            if (!productId.HasValue)
            {
                return ingredients.FindAll(ingredient => ingredient.IsActive);
            }
            if (productId.Value <= 0)
            {
                throw new ArgumentException("מזהה המוצר אינו תקין.");
            }
            GetActiveProductForUser(userId, productId.Value);
            List<RecipeIngredient> existingRecipe = RecipeIngredientBLL.GetRecipeIngredientsForProduct(userId, productId.Value);
            HashSet<int> existingRecipeIngredientIds = new HashSet<int>();
            foreach (RecipeIngredient recipeIngredient in existingRecipe)
            {
                existingRecipeIngredientIds.Add(recipeIngredient.IngredientId);
            }
            return ingredients.FindAll(ingredient => ingredient.IsActive || existingRecipeIngredientIds.Contains(ingredient.IngredientId));
        }
        public static List<Product> GetInactiveProductsForUser(int userId)
        {
            List<Product> products = GetProductsForUser(userId);
            return products.FindAll(product => !product.IsActive);
        }
        public static int CreateProduct(int userId, string productName, decimal yieldQuantity, int yieldUnitId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (string.IsNullOrWhiteSpace(productName))
            {
                throw new ArgumentException("יש להזין שם מוצר.");
            }
            productName = productName.Trim();
            if (productName.Length > 150)
            {
                throw new ArgumentException("שם המוצר יכול להכיל עד 150 תווים.");
            }
            if (yieldQuantity <= 0)
            {
                throw new ArgumentException("כמות התוצר חייבת להיות גדולה מאפס.");
            }
            if (yieldQuantity > 999999999999.999999m)
            {
                throw new ArgumentException("כמות התוצר גדולה מדי.");
            }
            if (decimal.Round(yieldQuantity, 6) != yieldQuantity)
            {
                throw new ArgumentException("כמות התוצר יכולה להכיל עד 6 ספרות אחרי הנקודה.");
            }
            if (yieldUnitId <= 0)
            {
                throw new ArgumentException("יש לבחור יחידת תוצר.");
            }
            List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
            MeasurementUnit yieldUnit = availableUnits.Find(unit => unit.MeasurementUnitId == yieldUnitId);
            if (yieldUnit == null)
            {
                throw new ArgumentException("יחידת התוצר שנבחרה אינה זמינה לעסק.");
            }
            int createdProductId = ProductDAL.CreateProduct(userId, productName, yieldQuantity, yieldUnit.UnitName);
            if (createdProductId <= 0)
            {
                throw new InvalidOperationException("לא ניתן להוסיף את המוצר עבור המשתמש הנוכחי.");
            }
            return createdProductId;
        }
        public static int CreateProductWithRecipe(int userId, string productName, decimal yieldQuantity, int yieldUnitId, List<RecipeIngredientInput> recipeIngredients)
        {
            MeasurementUnit yieldUnit = ValidateProductWithRecipeInput(userId, productName, yieldQuantity, yieldUnitId, recipeIngredients);
            productName = productName.Trim();
            int createdProductId = ProductDAL.CreateProductWithRecipe(userId, productName, yieldQuantity, yieldUnit.UnitName, recipeIngredients);
            if (createdProductId <= 0)
            {
                throw new InvalidOperationException("לא ניתן ליצור את המוצר והמתכון.");
            }
            CostCalculationBLL.CalculateAndSaveProductCost(userId, createdProductId);
            return createdProductId;
        }
        public static void UpdateProductWithRecipe(int userId, int productId, string productName, decimal yieldQuantity, int yieldUnitId, List<RecipeIngredientInput> recipeIngredients)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("מזהה המוצר אינו תקין.");
            }
            GetActiveProductForUser(userId, productId);
            List<RecipeIngredient> existingRecipe = RecipeIngredientBLL.GetRecipeIngredientsForProduct(userId, productId);
            HashSet<int> allowedInactiveIngredientIds = new HashSet<int>();
            foreach (RecipeIngredient recipeIngredient in existingRecipe)
            {
                allowedInactiveIngredientIds.Add(recipeIngredient.IngredientId);
            }
            MeasurementUnit yieldUnit = ValidateProductWithRecipeInput(userId, productName, yieldQuantity, yieldUnitId, recipeIngredients, allowedInactiveIngredientIds);
            productName = productName.Trim();
            bool updated = ProductDAL.UpdateProductWithRecipe(userId, productId, productName, yieldQuantity, yieldUnit.UnitName, recipeIngredients);
            if (!updated)
            {
                throw new InvalidOperationException("לא ניתן לעדכן את המוצר והמתכון.");
            }
        }
        public static void DeactivateProduct(int userId, int productId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (productId <= 0)
            {
                throw new ArgumentException("מזהה המוצר אינו תקין.");
            }
            bool wasDeactivated =
                ProductDAL.DeactivateProduct(userId, productId);
            if (!wasDeactivated)
            {
                throw new InvalidOperationException("לא ניתן להשבית את המוצר.");
            }
        }
        public static void RestoreProduct(int userId, int productId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (productId <= 0)
            {
                throw new ArgumentException("מזהה המוצר אינו תקין.");
            }
            List<Product> inactiveProducts = GetInactiveProductsForUser(userId);
            Product productToRestore = inactiveProducts.Find(product => product.ProductId == productId);
            if (productToRestore == null)
            {
                throw new InvalidOperationException("לא ניתן לשחזר את המוצר.");
            }
            List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
            MeasurementUnit yieldUnit = availableUnits.Find(unit => string.Equals(unit.UnitName, productToRestore.YieldUnitLabel, StringComparison.OrdinalIgnoreCase));
            if (yieldUnit == null)
            {
                throw new InvalidOperationException("יחידת התוצר של המוצר אינה זמינה.");
            }
            ReactivateProduct(userId, productToRestore.ProductId, productToRestore.ProductName, productToRestore.YieldQuantity, yieldUnit.MeasurementUnitId);
        }
        public static void ReactivateProduct(int userId, int productId, string productName, decimal yieldQuantity, int yieldUnitId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (productId <= 0)
            {
                throw new ArgumentException("מזהה המוצר אינו תקין.");
            }
            if (string.IsNullOrWhiteSpace(productName))
            {
                throw new ArgumentException("יש להזין שם מוצר.");
            }
            productName = productName.Trim();
            if (productName.Length > 150)
            {
                throw new ArgumentException("שם המוצר יכול להכיל עד 150 תווים.");
            }
            if (yieldQuantity <= 0)
            {
                throw new ArgumentException("כמות התוצר חייבת להיות גדולה מאפס.");
            }
            if (yieldQuantity > 999999999999.999999m)
            {
                throw new ArgumentException("כמות התוצר גדולה מדי.");
            }
            if (decimal.Round(yieldQuantity, 6) != yieldQuantity)
            {
                throw new ArgumentException("כמות התוצר יכולה להכיל עד 6 ספרות אחרי הנקודה.");
            }
            if (yieldUnitId <= 0)
            {
                throw new ArgumentException("יש לבחור יחידת תוצר.");
            }
            List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
            MeasurementUnit yieldUnit = availableUnits.Find(unit => unit.MeasurementUnitId == yieldUnitId);
            if (yieldUnit == null)
            {
                throw new ArgumentException("יחידת התוצר שנבחרה אינה זמינה לעסק.");
            }
            bool wasReactivated = ProductDAL.ReactivateProduct(userId, productId, productName, yieldQuantity, yieldUnit.UnitName);
            if (!wasReactivated)
            {
                throw new InvalidOperationException("לא ניתן להפעיל מחדש את המוצר.");
            }
        }
    }
}
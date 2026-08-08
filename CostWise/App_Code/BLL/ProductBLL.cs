using System;
using System.Collections.Generic;
using CostWise.App_Code.DAL;
namespace CostWise.App_Code.BLL
{
    public static class ProductBLL
    {
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
            List<Ingredient> activeIngredients = IngredientBLL.GetActiveIngredientsForUser(userId);
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
                Ingredient ingredient = activeIngredients.Find(item => item.IngredientId == recipeIngredient.IngredientId);
                if (ingredient == null)
                {
                    throw new InvalidOperationException("אחד הרכיבים אינו פעיל או אינו שייך לעסק שלך.");
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
            int createdProductId = ProductDAL.CreateProductWithRecipe(userId, productName, yieldQuantity, yieldUnit.UnitName, recipeIngredients);
            if (createdProductId <= 0)
            {
                throw new InvalidOperationException("לא ניתן ליצור את המוצר והמתכון.");
            }
            return createdProductId;
        }
        public static void UpdateProduct(int userId, int productId, string productName, decimal yieldQuantity, int yieldUnitId)
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
            bool wasUpdated = ProductDAL.UpdateProduct(userId, productId, productName, yieldQuantity, yieldUnit.UnitName);
            if (!wasUpdated)
            {
                throw new InvalidOperationException("לא ניתן לעדכן את המוצר.");
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
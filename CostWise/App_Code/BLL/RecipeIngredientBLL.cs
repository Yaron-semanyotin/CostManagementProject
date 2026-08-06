using System;
using System.Collections.Generic;
using CostWise.App_Code.DAL;

namespace CostWise.App_Code.BLL
{
    public static class RecipeIngredientBLL
    {
        public static List<RecipeIngredient> GetRecipeIngredientsForProduct(int userId, int productId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (productId <= 0)
            {
                throw new ArgumentException("המוצר שנבחר אינו תקין.");
            }
            List<Product> products = ProductBLL.GetProductsForUser(userId);
            Product product = products.Find(item => item.ProductId == productId);
            if (product == null || !product.IsActive)
            {
                throw new InvalidOperationException("המוצר לא נמצא, אינו פעיל או אינו שייך לעסק שלך.");
            }
            return RecipeIngredientDAL.GetRecipeIngredientsForProduct(userId, productId);
        }
        public static List<MeasurementUnit> GetCompatibleUnitsForIngredient(int userId, int ingredientId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (ingredientId <= 0)
            {
                throw new ArgumentException("הרכיב שנבחר אינו תקין.");
            }
            List<Ingredient> ingredients = IngredientBLL.GetActiveIngredientsForUser(userId);
            Ingredient ingredient = ingredients.Find(item => item.IngredientId == ingredientId);
            if (ingredient == null)
            {
                throw new InvalidOperationException("הרכיב לא נמצא, אינו פעיל או אינו שייך לעסק שלך.");
            }
            List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
            MeasurementUnit packageUnit = availableUnits.Find(unit => unit.MeasurementUnitId == ingredient.PackageUnitId);
            if (packageUnit == null)
            {
                throw new InvalidOperationException("יחידת האריזה של הרכיב אינה זמינה לעסק.");
            }
            return availableUnits.FindAll(unit => string.Equals(unit.UnitFamily, packageUnit.UnitFamily, StringComparison.OrdinalIgnoreCase));
        }
        public static void CreateRecipeIngredient(int userId, int productId, int ingredientId, decimal quantity, int measurementUnitId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (productId <= 0)
            {
                throw new ArgumentException("המוצר שנבחר אינו תקין.");
            }
            if (ingredientId <= 0)
            {
                throw new ArgumentException("הרכיב שנבחר אינו תקין.");
            }
            if (quantity <= 0)
            {
                throw new ArgumentException("הכמות במתכון חייבת להיות גדולה מאפס.");
            }
            if (quantity > 999999999999.999999m)
            {
                throw new ArgumentException("הכמות במתכון גדולה מדי.");
            }
            if (decimal.Round(quantity, 6) != quantity)
            {
                throw new ArgumentException("הכמות במתכון יכולה להכיל עד 6 ספרות אחרי הנקודה.");
            }
            if (measurementUnitId <= 0)
            {
                throw new ArgumentException("יש לבחור יחידת מידה למתכון.");
            }
            List<Product> products = ProductBLL.GetProductsForUser(userId);
            Product product = products.Find(item => item.ProductId == productId);
            if (product == null || !product.IsActive)
            {
                throw new InvalidOperationException("המוצר לא נמצא, אינו פעיל או אינו שייך לעסק שלך.");
            }
            List<Ingredient> ingredients = IngredientBLL.GetIngredientsForUser(userId);
            Ingredient ingredient = ingredients.Find(item => item.IngredientId == ingredientId);
            if (ingredient == null || !ingredient.IsActive)
            {
                throw new InvalidOperationException("הרכיב לא נמצא, אינו פעיל או אינו שייך לעסק שלך.");
            }
            List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
            MeasurementUnit recipeUnit = availableUnits.Find(unit => unit.MeasurementUnitId == measurementUnitId);
            if (recipeUnit == null)
            {
                throw new ArgumentException("יחידת המידה שנבחרה אינה זמינה לעסק.");
            }
            MeasurementUnit packageUnit = availableUnits.Find(unit => unit.MeasurementUnitId == ingredient.PackageUnitId);
            if (packageUnit == null)
            {
                throw new InvalidOperationException("יחידת האריזה של הרכיב אינה זמינה לעסק.");
            }
            if (!string.Equals(packageUnit.UnitFamily, recipeUnit.UnitFamily, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("יחידת המידה במתכון אינה תואמת למשפחת יחידת האריזה של הרכיב.");
            }
            List<RecipeIngredient> existingRecipeIngredients = RecipeIngredientDAL.GetRecipeIngredientsForProduct(userId, productId);
            int sortOrder = 1;
            foreach (RecipeIngredient existingRecipeIngredient in existingRecipeIngredients)
            {
                if (existingRecipeIngredient.SortOrder >= sortOrder)
                {
                    sortOrder = existingRecipeIngredient.SortOrder + 1;
                }
            }
            bool created = RecipeIngredientDAL.CreateRecipeIngredient(userId, productId, ingredientId, quantity, measurementUnitId, sortOrder);
            if (!created)
            {
                throw new InvalidOperationException("לא ניתן היה להוסיף את הרכיב למתכון.");
            }
        }
        public static void UpdateRecipeIngredient(int userId, int productId, int recipeIngredientId, int ingredientId, decimal quantity, int measurementUnitId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (productId <= 0)
            {
                throw new ArgumentException("המוצר שנבחר אינו תקין.");
            }
            if (recipeIngredientId <= 0)
            {
                throw new ArgumentException("שורת המתכון שנבחרה אינה תקינה.");
            }
            if (ingredientId <= 0)
            {
                throw new ArgumentException("הרכיב שנבחר אינו תקין.");
            }
            if (quantity <= 0)
            {
                throw new ArgumentException("הכמות במתכון חייבת להיות גדולה מאפס.");
            }
            if (quantity > 999999999999.999999m)
            {
                throw new ArgumentException("הכמות במתכון גדולה מדי.");
            }
            if (decimal.Round(quantity, 6) != quantity)
            {
                throw new ArgumentException("הכמות במתכון יכולה להכיל עד 6 ספרות אחרי הנקודה.");
            }
            if (measurementUnitId <= 0)
            {
                throw new ArgumentException("יש לבחור יחידת מידה למתכון.");
            }
            List<Product> products = ProductBLL.GetProductsForUser(userId);
            Product product = products.Find(item => item.ProductId == productId);
            if (product == null || !product.IsActive)
            {
                throw new InvalidOperationException("המוצר לא נמצא, אינו פעיל או אינו שייך לעסק שלך.");
            }
            List<RecipeIngredient> recipeIngredients = RecipeIngredientDAL.GetRecipeIngredientsForProduct(userId, productId);
            RecipeIngredient existingRecipeIngredient = recipeIngredients.Find(item => item.RecipeIngredientId == recipeIngredientId);
            if (existingRecipeIngredient == null)
            {
                throw new InvalidOperationException("שורת המתכון לא נמצאה או אינה שייכת למוצר ולעסק שלך.");
            }
            List<Ingredient> ingredients = IngredientBLL.GetIngredientsForUser(userId);
            Ingredient ingredient = ingredients.Find(item => item.IngredientId == ingredientId);
            if (ingredient == null || !ingredient.IsActive)
            {
                throw new InvalidOperationException("הרכיב לא נמצא, אינו פעיל או אינו שייך לעסק שלך.");
            }
            List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
            MeasurementUnit recipeUnit = availableUnits.Find(unit => unit.MeasurementUnitId == measurementUnitId);
            if (recipeUnit == null)
            {
                throw new ArgumentException("יחידת המידה שנבחרה אינה זמינה לעסק.");
            }
            MeasurementUnit packageUnit = availableUnits.Find(unit => unit.MeasurementUnitId == ingredient.PackageUnitId);
            if (packageUnit == null)
            {
                throw new InvalidOperationException("יחידת האריזה של הרכיב אינה זמינה לעסק.");
            }
            if (!string.Equals(packageUnit.UnitFamily, recipeUnit.UnitFamily, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("יחידת המידה במתכון אינה תואמת למשפחת יחידת האריזה של הרכיב.");
            }
            bool updated = RecipeIngredientDAL.UpdateRecipeIngredient(userId, productId, recipeIngredientId, ingredientId, quantity, measurementUnitId, existingRecipeIngredient.SortOrder);
            if (!updated)
            {
                throw new InvalidOperationException("לא ניתן היה לעדכן את שורת המתכון.");
            }
        }
        public static void DeleteRecipeIngredient(int userId, int productId, int recipeIngredientId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (productId <= 0)
            {
                throw new ArgumentException("המוצר שנבחר אינו תקין.");
            }
            if (recipeIngredientId <= 0)
            {
                throw new ArgumentException("שורת המתכון שנבחרה אינה תקינה.");
            }
            List<Product> products = ProductBLL.GetProductsForUser(userId);
            Product product = products.Find(item => item.ProductId == productId);
            if (product == null || !product.IsActive)
            {
                throw new InvalidOperationException("המוצר לא נמצא, אינו פעיל או אינו שייך לעסק שלך.");
            }
            List<RecipeIngredient> recipeIngredients = RecipeIngredientDAL.GetRecipeIngredientsForProduct(userId, productId);
            RecipeIngredient existingRecipeIngredient = recipeIngredients.Find(item => item.RecipeIngredientId == recipeIngredientId);
            if (existingRecipeIngredient == null)
            {
                throw new InvalidOperationException("שורת המתכון לא נמצאה או אינה שייכת למוצר ולעסק שלך.");
            }
            bool deleted = RecipeIngredientDAL.DeleteRecipeIngredient(userId, productId, recipeIngredientId);
            if (!deleted)
            {
                throw new InvalidOperationException("לא ניתן היה למחוק את שורת המתכון.");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using CostWise.App_Code.DAL;

namespace CostWise.App_Code.BLL
{
    public static class IngredientBLL
    {
        public static List<Ingredient> GetIngredientsForUser(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            return IngredientDAL.GetIngredientsForUser(userId);
        }
        public static List<Ingredient> GetActiveIngredientsForUser(int userId)
        {
            List<Ingredient> ingredients = GetIngredientsForUser(userId);
            return ingredients.FindAll(ingredient => ingredient.IsActive);
        }
        public static void CreateIngredient(int userId, string ingredientName, decimal packagePrice, decimal packageQuantity, int packageUnitId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (string.IsNullOrWhiteSpace(ingredientName))
            {
                throw new ArgumentException("יש להזין שם רכיב.");
            }
            ingredientName = ingredientName.Trim();
            if (ingredientName.Length > 150)
            {
                throw new ArgumentException("שם הרכיב יכול להכיל עד 150 תווים.");
            }
            if (packagePrice < 0)
            {
                throw new ArgumentException("מחיר האריזה אינו יכול להיות שלילי.");
            }
            if (packagePrice > 9999999999999999.99m)
            {
                throw new ArgumentException("מחיר האריזה גדול מדי.");
            }
            if (decimal.Round(packagePrice, 2) != packagePrice)
            {
                throw new ArgumentException("מחיר האריזה יכול להכיל עד 2 ספרות אחרי הנקודה.");
            }
            if (packageQuantity <= 0)
            {
                throw new ArgumentException("כמות האריזה חייבת להיות גדולה מאפס.");
            }
            if (packageQuantity > 999999999999.999999m)
            {
                throw new ArgumentException("כמות האריזה גדולה מדי.");
            }
            if (decimal.Round(packageQuantity, 6) != packageQuantity)
            {
                throw new ArgumentException("כמות האריזה יכולה להכיל עד 6 ספרות אחרי הנקודה.");
            }
            if (packageUnitId <= 0)
            {
                throw new ArgumentException("יש לבחור יחידת מידה לאריזה.");
            }
            List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
            bool isPackageUnitAvailable = availableUnits.Exists(unit => unit.MeasurementUnitId == packageUnitId);
            if (!isPackageUnitAvailable)
            {
                throw new ArgumentException("יחידת המידה שנבחרה אינה זמינה לעסק.");
            }
            Ingredient existingIngredient = IngredientDAL.GetIngredientByNameForUser(userId, ingredientName);
            if (existingIngredient != null)
            {
                if (existingIngredient.IsActive)
                {
                    throw new InvalidOperationException("רכיב בשם זה כבר קיים.");
                }
                throw new InvalidOperationException("קיים רכיב מושבת בשם זה. יש להפעיל אותו מחדש.");
            }
            bool wasCreated = IngredientDAL.CreateIngredient(userId, ingredientName, packagePrice, packageQuantity, packageUnitId);
            if (!wasCreated)
            {
                throw new InvalidOperationException("לא ניתן להוסיף את הרכיב עבור המשתמש הנוכחי.");
            }
        }
        public static void UpdateIngredient(int userId, int ingredientId, string ingredientName, decimal packagePrice, decimal packageQuantity, int packageUnitId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (ingredientId <= 0)
            {
                throw new ArgumentException("מזהה הרכיב אינו תקין.");
            }
            if (string.IsNullOrWhiteSpace(ingredientName))
            {
                throw new ArgumentException("יש להזין שם רכיב.");
            }
            ingredientName = ingredientName.Trim();
            if (ingredientName.Length > 150)
            {
                throw new ArgumentException("שם הרכיב יכול להכיל עד 150 תווים.");
            }
            if (packagePrice < 0)
            {
                throw new ArgumentException("מחיר האריזה אינו יכול להיות שלילי.");
            }
            if (packagePrice > 9999999999999999.99m)
            {
                throw new ArgumentException("מחיר האריזה גדול מדי.");
            }
            if (decimal.Round(packagePrice, 2) != packagePrice)
            {
                throw new ArgumentException("מחיר האריזה יכול להכיל עד 2 ספרות אחרי הנקודה.");
            }
            if (packageQuantity <= 0)
            {
                throw new ArgumentException("כמות האריזה חייבת להיות גדולה מאפס.");
            }
            if (packageQuantity > 999999999999.999999m)
            {
                throw new ArgumentException("כמות האריזה גדולה מדי.");
            }
            if (decimal.Round(packageQuantity, 6) != packageQuantity)
            {
                throw new ArgumentException("כמות האריזה יכולה להכיל עד 6 ספרות אחרי הנקודה.");
            }
            if (packageUnitId <= 0)
            {
                throw new ArgumentException("יש לבחור יחידת מידה לאריזה.");
            }
            List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
            bool isPackageUnitAvailable = availableUnits.Exists(unit => unit.MeasurementUnitId == packageUnitId);
            if (!isPackageUnitAvailable)
            {
                throw new ArgumentException("יחידת המידה שנבחרה אינה זמינה לעסק.");
            }
            Ingredient existingIngredient = IngredientDAL.GetIngredientByNameForUser(userId, ingredientName);
            if (existingIngredient != null && existingIngredient.IngredientId != ingredientId)
            {
                throw new InvalidOperationException("רכיב אחר בשם זה כבר קיים.");
            }
            bool wasUpdated = IngredientDAL.UpdateIngredient(userId, ingredientId, ingredientName, packagePrice, packageQuantity, packageUnitId);
            if (!wasUpdated)
            {
                throw new InvalidOperationException("לא ניתן לעדכן את הרכיב.");
            }
        }
        public static void DeactivateIngredient(int userId, int ingredientId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (ingredientId <= 0)
            {
                throw new ArgumentException("מזהה הרכיב אינו תקין.");
            }
            bool wasDeactivated = IngredientDAL.DeactivateIngredient(userId, ingredientId);
            if (!wasDeactivated)
            {
                throw new InvalidOperationException("לא ניתן להשבית את הרכיב.");
            }
        }
        public static void ReactivateIngredient(int userId, int ingredientId, string ingredientName, decimal packagePrice, decimal packageQuantity, int packageUnitId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (ingredientId <= 0)
            {
                throw new ArgumentException("מזהה הרכיב אינו תקין.");
            }
            if (string.IsNullOrWhiteSpace(ingredientName))
            {
                throw new ArgumentException("יש להזין שם רכיב.");
            }
            ingredientName = ingredientName.Trim();
            if (ingredientName.Length > 150)
            {
                throw new ArgumentException("שם הרכיב יכול להכיל עד 150 תווים.");
            }
            if (packagePrice < 0)
            {
                throw new ArgumentException("מחיר האריזה אינו יכול להיות שלילי.");
            }
            if (packagePrice > 9999999999999999.99m)
            {
                throw new ArgumentException("מחיר האריזה גדול מדי.");
            }
            if (decimal.Round(packagePrice, 2) != packagePrice)
            {
                throw new ArgumentException("מחיר האריזה יכול להכיל עד 2 ספרות אחרי הנקודה.");
            }
            if (packageQuantity <= 0)
            {
                throw new ArgumentException("כמות האריזה חייבת להיות גדולה מאפס.");
            }
            if (packageQuantity > 999999999999.999999m)
            {
                throw new ArgumentException("כמות האריזה גדולה מדי.");
            }
            if (decimal.Round(packageQuantity, 6) != packageQuantity)
            {
                throw new ArgumentException("כמות האריזה יכולה להכיל עד 6 ספרות אחרי הנקודה.");
            }
            if (packageUnitId <= 0)
            {
                throw new ArgumentException("יש לבחור יחידת מידה לאריזה.");
            }
            List<MeasurementUnit> availableUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
            bool isPackageUnitAvailable = availableUnits.Exists(unit => unit.MeasurementUnitId == packageUnitId);
            if (!isPackageUnitAvailable)
            {
                throw new ArgumentException("יחידת המידה שנבחרה אינה זמינה לעסק.");
            }
            Ingredient existingIngredient = IngredientDAL.GetIngredientByNameForUser(userId, ingredientName);
            if (existingIngredient != null && existingIngredient.IngredientId != ingredientId)
            {
                throw new InvalidOperationException("רכיב אחר בשם זה כבר קיים.");
            }
            bool wasReactivated = IngredientDAL.ReactivateIngredient(userId, ingredientId, ingredientName, packagePrice, packageQuantity, packageUnitId);
            if (!wasReactivated)
            {
                throw new InvalidOperationException("לא ניתן להפעיל מחדש את הרכיב.");
            }
        }
    }
}
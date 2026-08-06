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
        public static void CreateProduct(int userId, string productName, decimal yieldQuantity, int yieldUnitId)
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
            bool wasCreated = ProductDAL.CreateProduct(userId, productName, yieldQuantity, yieldUnit.UnitName);
            if (!wasCreated)
            {
                throw new InvalidOperationException("לא ניתן להוסיף את המוצר עבור המשתמש הנוכחי.");
            }
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
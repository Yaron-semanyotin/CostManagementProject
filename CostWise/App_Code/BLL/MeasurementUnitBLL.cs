using CostWise.App_Code.DAL;
using System.Collections.Generic;
using System;
namespace CostWise.App_Code.BLL
{
    public static class MeasurementUnitBLL
    {
        public static List<MeasurementUnit> GetSystemUnits()
        {
            return MeasurementUnitDAL.GetSystemUnits();
        }
        public static List<MeasurementUnit> GetAvailableUnits(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            return MeasurementUnitDAL.GetAvailableUnits(userId);
        }
        public static List<MeasurementUnit> GetCustomUnits(int userId)
        {
            List<MeasurementUnit> availableUnits = GetAvailableUnits(userId);
            return availableUnits.FindAll(unit => unit.BusinessId.HasValue);
        }
        public static void CreateCustomUnit(int userId, string unitName, string unitFamily, decimal conversionFactorToBase)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            unitName = unitName?.Trim();
            if (string.IsNullOrWhiteSpace(unitName))
            {
                throw new ArgumentException("שם היחידה הוא שדה חובה.");
            }
            if (unitName.Length > 50)
            {
                throw new ArgumentException("שם היחידה יכול להכיל עד 50 תווים.");
            }
            unitFamily = unitFamily?.Trim();
            bool isValidFamily = string.Equals(unitFamily, "Weight", StringComparison.Ordinal) ||
                string.Equals(unitFamily, "Volume", StringComparison.Ordinal) ||
                string.Equals(unitFamily, "Quantity", StringComparison.Ordinal);
            if (!isValidFamily)
            {
                throw new ArgumentException("משפחת היחידה אינה תקינה.");
            }
            if (conversionFactorToBase <= 0)
            {
                throw new ArgumentException("מקדם ההמרה חייב להיות גדול מאפס.");
            }
            if (conversionFactorToBase > 999999999999.999999m)
            {
                throw new ArgumentException("מקדם ההמרה גדול מדי.");
            }
            if (decimal.Round(conversionFactorToBase, 6) != conversionFactorToBase)
            {
                throw new ArgumentException("מקדם ההמרה יכול להכיל עד 6 ספרות אחרי הנקודה.");
            }
            if (MeasurementUnitDAL.UnitNameExistsForUser(userId, unitName))
            {
                throw new InvalidOperationException("יחידה בשם זה כבר קיימת.");
            }
            bool wasCreated = MeasurementUnitDAL.CreateCustomUnit(userId, unitName, unitFamily, conversionFactorToBase);
            if (!wasCreated)
            {
                throw new InvalidOperationException("לא ניתן ליצור את היחידה עבור המשתמש הנוכחי.");
            }
        }
        public static void UpdateCustomUnit(int userId, int measurementUnitId, string unitName, string unitFamily, decimal conversionFactorToBase)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (measurementUnitId <= 0)
            {
                throw new ArgumentException("מזהה יחידת המידה אינו תקין.");
            }
            unitName = unitName?.Trim();
            if (string.IsNullOrWhiteSpace(unitName))
            {
                throw new ArgumentException("שם היחידה הוא שדה חובה.");
            }
            if (unitName.Length > 50)
            {
                throw new ArgumentException("שם היחידה יכול להכיל עד 50 תווים.");
            }
            unitFamily = unitFamily?.Trim();
            bool isValidFamily = string.Equals(unitFamily, "Weight", StringComparison.Ordinal) ||
                string.Equals(unitFamily, "Volume", StringComparison.Ordinal) ||
                string.Equals(unitFamily, "Quantity", StringComparison.Ordinal);
            if (!isValidFamily)
            {
                throw new ArgumentException("משפחת היחידה אינה תקינה.");
            }
            if (conversionFactorToBase <= 0)
            {
                throw new ArgumentException("מקדם ההמרה חייב להיות גדול מאפס.");
            }
            if (conversionFactorToBase > 999999999999.999999m)
            {
                throw new ArgumentException("מקדם ההמרה גדול מדי.");
            }
            if (decimal.Round(conversionFactorToBase, 6) != conversionFactorToBase)
            {
                throw new ArgumentException("מקדם ההמרה יכול להכיל עד 6 ספרות אחרי הנקודה.");
            }
            if (MeasurementUnitDAL.UnitNameExistsForUserExceptUnit(userId, measurementUnitId, unitName))
            {
                throw new InvalidOperationException("יחידה בשם זה כבר קיימת.");
            }
            bool wasUpdated = MeasurementUnitDAL.UpdateCustomUnit(userId, measurementUnitId, unitName, unitFamily, conversionFactorToBase);
            if (!wasUpdated)
            {
                throw new InvalidOperationException("לא ניתן לעדכן את יחידת המידה.");
            }
        }
        public static void DeleteCustomUnit(int userId, int measurementUnitId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (measurementUnitId <= 0)
            {
                throw new ArgumentException("מזהה יחידת המידה אינו תקין.");
            }
            bool isInUse = MeasurementUnitDAL.IsCustomUnitInUseForUser(userId, measurementUnitId);
            if (isInUse)
            {
                throw new InvalidOperationException("לא ניתן למחוק יחידת מידה שנמצאת בשימוש.");
            }
            bool wasDeleted = MeasurementUnitDAL.DeleteCustomUnit(userId, measurementUnitId);
            if (!wasDeleted)
            {
                throw new InvalidOperationException("לא ניתן למחוק את יחידת המידה.");
            }
        }
    }
}
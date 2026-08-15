using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CostWise.App_Code.BLL
{
    public static class UnitConversionBLL
    {
        public static decimal ConvertToBaseUnit(decimal quantity, MeasurementUnit unit)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("הכמות חייבת להיות גדולה מאפס.");
            }
            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }
            if (unit.ConversionFactorToBase <= 0)
            {
                throw new ArgumentException("מקדם ההמרה חייב להיות גדול מאפס.");
            }
            return quantity * unit.ConversionFactorToBase;
        }
        public static bool AreCompatibleUnits(MeasurementUnit sourceUnit, MeasurementUnit targetUnit)
        {
            if (sourceUnit == null || targetUnit == null)
            {
                return false;
            }
            bool sameFamily = string.Equals(sourceUnit.UnitFamily, targetUnit.UnitFamily, StringComparison.OrdinalIgnoreCase);
            bool liquidMeasuredByWeight = string.Equals(sourceUnit.UnitFamily, "Volume", StringComparison.OrdinalIgnoreCase)
                &&
                string.Equals(targetUnit.UnitFamily, "Weight", StringComparison.OrdinalIgnoreCase);
            return sameFamily || liquidMeasuredByWeight;
        }
        public static void ValidateCompatibleUnits(MeasurementUnit sourceUnit, MeasurementUnit targetUnit)
        {
            if (sourceUnit == null)
            {
                throw new ArgumentNullException(nameof(sourceUnit));
            }
            if (targetUnit == null)
            {
                throw new ArgumentNullException(nameof(targetUnit));
            }
            if (!AreCompatibleUnits(sourceUnit, targetUnit))
            {
                throw new ArgumentException("לא ניתן להמיר בין יחידות ממשפחות שונות.");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CostWise.App_Code.BLL
{
    public class MeasurementUnit
    {
        public int MeasurementUnitId { get; set; }
        public int? BusinessId { get; set; }
        public string UnitName { get; set; }
        public string UnitFamily { get; set; }
        public decimal ConversionFactorToBase { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
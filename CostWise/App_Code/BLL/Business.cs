using System;

namespace CostWise.App_Code.BLL
{
    public class Business
    {
        public int BusinessId { get; set; }
        public string BusinessName { get; set; }
        public string LogoPath { get; set; }
        public bool ShowYieldUnitSelection { get; set; }
        public int? DefaultRecipeMeasurementUnitId { get; set; }
        public decimal VatRatePercent { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
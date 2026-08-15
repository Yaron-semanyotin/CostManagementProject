using System;

namespace CostWise.App_Code.BLL
{
    public class CostCalculation
    {
        public int CostCalculationId { get; set; }
        public int BusinessId { get; set; }
        public int ProductId { get; set; }
        public string ProductNameSnapshot { get; set; }
        public decimal YieldQuantitySnapshot { get; set; }
        public string YieldUnitLabelSnapshot { get; set; }
        public decimal TotalIngredientCostSnapshot { get; set; }
        public decimal CostPerYieldUnitSnapshot { get; set; }
        public DateTime CalculatedAtUtc { get; set; }
        public DateTime? ValidUntilUtc { get; set; }
    }
}